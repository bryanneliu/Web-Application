using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecalculateLDCG
{
    public class MetricCalculator
    {
        static public double[] IdcgArray = GetIdcgArray();

        static public double CalculateMetric(string metricName, List<QueryMetadata> querySet, Dictionary<string, List<ResultMetadata>> resultSet)
        {
            // go through each result to calcualte in result level, then aggregate to query level, then overall level
            double sum = 0;
            int queryCount = 0;
            foreach (var queryMetadata in querySet)
            {
                if (resultSet.ContainsKey(queryMetadata.EntityGuid))
                {
                    var resultList = resultSet[queryMetadata.EntityGuid];
                    sum += CalculateQueryLevelMetric(metricName, queryMetadata, resultList);
                    queryCount++;
                }
                else if (queryMetadata.IsLocal == 1 && metricName.Contains("@"))
                {
                    queryMetadata.metric.Add(metricName, 0);
                    queryCount++;
                }

                queryMetadata.MetricSchema.Add(metricName);
            }

            return sum / queryCount;
        }

        private static double CalculateQueryLevelMetric(string metricName, QueryMetadata queryMetadata, List<ResultMetadata> resultList)
        {
            foreach (var result in resultList)
            {
                CalculateResultLevelMetric(metricName, queryMetadata, result);
            }

            return AggreatedOnQueryLevel(metricName, resultList, queryMetadata);
        }

        private static double AggreatedOnQueryLevel(string metricName, List<ResultMetadata> resultList, QueryMetadata queryMetadata)
        {
            double metricValue = 0;

            if (metricName.Contains("dcg@"))
            {
                var parts = metricName.Split('@');
                int maxPosition = int.Parse(parts[1]);
                var metricNamePrefix = parts[0];

                if (queryMetadata.IsSingle == 1 && resultList.Count == 1)
                {
                    metricValue = resultList[0].metric[metricNamePrefix];
                }
                else 
                {
                    if (queryMetadata.IsSingle == 1)
                    {
                        maxPosition = Math.Min(resultList.Count, maxPosition);
                    }

                    for (int i = 0; i < Math.Min(maxPosition, resultList.Count); i++)
                    {
                        metricValue += resultList[i].metric[metricNamePrefix] * GetPositionWeight(i + 1);
                    }

                    metricValue /= IdcgArray[maxPosition];
                }
            }
            else
            {
                metricValue = resultList.Average(l => l.metric[metricName]);
            }

            queryMetadata.metric.Add(metricName, metricValue);

            return metricValue;
        }

        private static double GetPositionWeight(int position)
        {
            return (Math.Log(2) / Math.Log(position + 1));
        }

        private static double[] GetIdcgArray()
        {
            double[] idcgArray = new double[50];
            for (var position = 1; position < idcgArray.Length; position++)
            {
                idcgArray[position] = idcgArray[position - 1] + GetPositionWeight(position);
            }
            return idcgArray;
        }

        private static void CalculateResultLevelMetric(string metricName, QueryMetadata queryMetadata, ResultMetadata result)
        {

            if (result.metric.ContainsKey(metricName))
                return;

            double metricScore = 0;

            if (metricName == "matchscore")
            {
                metricScore = CalIntentMatchScore(result);
            }

            else if (metricName == "popularscore")
            {
                metricScore = result.Popularity / 100000.0;
            }

            else if (metricName == "distancescore")
            {
                metricScore = CalDistanceMatchScore(result, queryMetadata);
            }

            else if (metricName == "linedistance")
            {
                metricScore = CalLineDistanceScore(result, queryMetadata);
            }

            else if (metricName == "linedistance2")
            {
                metricScore = CalLineDistanceScore(result, queryMetadata);
                if (queryMetadata.ManualInnerRadius > 20)
                {
                    metricScore = -1; ;
                }
            }

            else if (metricName == "linedistance3")
            {
                metricScore = CalLineDistanceScore(result, queryMetadata);
                if (metricScore > 100)
                {
                    metricScore = -1; ;
                }
            }

            else if (metricName == "linedistanceinner")
            {
                metricScore = CalLineDistanceInnerScore(result, queryMetadata);
            }

            else if (metricName == "distanceinnerratio")
            {
                metricScore = CalLineDistanceInnerRatio(result, queryMetadata);
            }

            else if (metricName == "urlscore")
            {
                metricScore = string.IsNullOrWhiteSpace(result.Url) ? 0 : 1;
            }

            else if (metricName == "reviewscore")
            {
                metricScore = (result.ReviewCount > 3 || result.Rating > 0) ? 1 : 0;
            }

            else if (metricName == "nojunkscore")
            {
                metricScore = (result.ReviewCount > 3 || result.Rating > 0 || !string.IsNullOrWhiteSpace(result.Url)) ? 1 : 0;
            }

            else if (metricName == "l3dcgrelevance")
            {
                CalculateResultLevelMetric("matchscore", queryMetadata, result);
                CalculateResultLevelMetric("popularscore", queryMetadata, result);
                CalculateResultLevelMetric("urlscore", queryMetadata, result);
                CalculateResultLevelMetric("reviewscore", queryMetadata, result);
                metricScore = CalculateL3dcgIntent(result);
            }

            else if (metricName == "pldcgrelevance")
            {
                CalculateResultLevelMetric("l3dcgrelevance", queryMetadata, result);
                metricScore = CalculatePLdcgIntent(result);
            }

            else if (metricName.StartsWith("l3dcg"))
            {
                CalculateResultLevelMetric("l3dcgrelevance", queryMetadata, result);
                CalculateResultLevelMetric("distancescore", queryMetadata, result);

                metricScore = result.metric["l3dcgrelevance"] * result.metric["distancescore"] * 100;
                metricName = "l3dcg";
            }

            else if (metricName.StartsWith("pldcg"))
            {
                CalculateResultLevelMetric("pldcgrelevance", queryMetadata, result);
                CalculateResultLevelMetric("distancescore", queryMetadata, result);

                metricScore = result.metric["pldcgrelevance"] * result.metric["distancescore"] * 100;
                metricName = "pldcg";
            }

            if (!result.metric.ContainsKey(metricName))
            {
                result.metric.Add(metricName, metricScore);
            }
        }

        private static double CalculatePLdcgIntent(ResultMetadata result)
        {
            if (result.IsPopularityIntent == 1)
            {
                double score = 0;
                if (result.IntentJudgment == 1)
                {
                    score = 0.722328375279349;
                }
                else if (result.IntentJudgment == 2)
                {
                    score = 0.231394025352752;
                }

                if (score > 0)
                {
                    score += 0.1967974035372131 * result.metric["popularscore"];
                    score += 0.0178954605522934 * result.metric["reviewscore"];
                    score += 0.0629787606311446 * result.metric["urlscore"];
                }

                return score;
            }

            else return CalculateL3dcgIntent(result);
        }

        private static double CalculateL3dcgIntent(ResultMetadata result)
        {
            double score = result.metric["matchscore"];
            if (score > 0)
            {
                score += 0.0567974035372131 * result.metric["popularscore"];
                score += 0.0478954605522934 * result.metric["reviewscore"];
                score += 0.0729787606311446 * result.metric["urlscore"];
            }

            return score;
        }

        private static double CalLineDistanceScore(ResultMetadata result, QueryMetadata queryMetadata)
        {
            return CalDistance(result, queryMetadata);
        }

        private static double CalLineDistanceInnerScore(ResultMetadata result, QueryMetadata queryMetadata)
        {
            double distance = CalDistance(result, queryMetadata);

            return Math.Max(distance - result.ManualInnerRadius, 0);
        }

        private static double CalLineDistanceInnerRatio(ResultMetadata result, QueryMetadata queryMetadata)
        {
            double distance = CalDistance(result, queryMetadata);
            return distance / Math.Max(1, result.ManualInnerRadius);
        }

        private static double CalDistanceMatchScore(ResultMetadata result, QueryMetadata query)
        {
            double outterCircle = result.ManualOuterRadius;

            if (!string.IsNullOrWhiteSpace(result.FinalTargetLocationOption) && result.FinalTargetLocationOption == "UserLocation")
            {
                outterCircle = Math.Max(outterCircle, 45);
            }

            double distance = CalDistance(result, query);
            if (distance <= result.ManualInnerRadius)
            {
                return 1;
            }

            if (distance > outterCircle)
            {
                return 0;
            }
            
            distance -= result.ManualInnerRadius;
            double range = outterCircle - result.ManualInnerRadius;  // range > 0 ALWAYS due to above logic
            return 1 - Math.Sqrt(distance / range);
        }

        private static double CalDistance(ResultMetadata result, QueryMetadata query)
        {
            double lat1 = query.FinalLat;
            double long1 = query.FinalLong;
            double lat2 = result.Lat;
            double long2 = result.Long;

            return HaversineDistanceCalculator.GetHaversineDistanceInMiles(lat1, long1, lat2, long2);
        }

        private static double CalIntentMatchScore(ResultMetadata result)
        {
            if (result.IntentJudgment == 1)
            {
                return 0.822328375279349;
            }
            else if (result.IntentJudgment == 2)
            {
                return 0.311394025352752;
            }

            return 0;
        }
    }
}
