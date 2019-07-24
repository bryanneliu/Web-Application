using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace RecalculateLDCG
{
    public static class MetricLogger
    {
        public static void OutputQueryMetadata(string v, List<QueryMetadata> querySet, List<string> metricList)
        {
            using (StreamWriter sw = new StreamWriter(v))
            {
                OutputSchema(sw, querySet.First().Schema, metricList);
                foreach (var query in querySet)
                {
                    sw.Write(query.Line);
                    foreach( var metricName in metricList)
                    {
                        sw.Write("\t{0}", query.metric.ContainsKey(metricName) ? Math.Round(query.metric[metricName], 2).ToString() : string.Empty);
                    }

                    sw.WriteLine();
                }
            }
        }

        private static void OutputSchema(StreamWriter sw, string[] schema1, List<string> schema2)
        {
            string line = string.Empty;
            foreach (var s in schema1)
            {
                line += s + "\t";
            }
            foreach (var s in schema2)
            {
                line += s + "\t";
            }
            sw.WriteLine(line.TrimEnd('\t'));
        }
        internal static void GenerateAnalysisReport(string v, List<QueryMetadata> querySet, Dictionary<string, List<ResultMetadata>> resultSet, List<string> metricList)
        {
            using (StreamWriter sw = new StreamWriter(v))
            {
                sw.WriteLine("Overall:");
                GetnerateAnalysisReportForSlice(sw, querySet, resultSet.Values.ToList(), metricList);

                var segmentList = querySet.Select(s => s.Segment).Distinct().ToList();
                if (segmentList.Count > 1)
                {
                    foreach (var segment in segmentList)
                    {
                        var slicedQuerySet = querySet.Where(s => s.Segment == segment).ToList();
                        var slicedResultSet = new List<List<ResultMetadata>>();

                        foreach(var query in slicedQuerySet)
                        {
                            if (resultSet.ContainsKey(query.EntityGuid))
                            {
                                slicedResultSet.Add(resultSet[query.EntityGuid]);
                            }
                        }

                        sw.WriteLine("segment: {0}", segment);
                        GetnerateAnalysisReportForSlice(sw, slicedQuerySet, slicedResultSet, metricList);
                    }
                }
            }
        }

        internal static void StatisticJunk(string v, List<QueryMetadata> querySet, Dictionary<string, List<ResultMetadata>> resultSet)
        {
            var junkValueSet = resultSet.Values.Select(s => s.Select(i => i.IsNoJunk ? 1 : 0).ToList()).ToList();
            var reviewValueSet = resultSet.Values.Select(s => s.Select(i => i.HasReview ? 1 : 0).ToList()).ToList();

            Dictionary<string, int> stat = new Dictionary<string, int>();
            foreach (var set in junkValueSet)
            {
                int resultCount = set.Count;
                int nojunkCount = set.Sum();
                string key = string.Format("{0}-{1}", resultCount, nojunkCount);
                if (stat.ContainsKey(key))
                {
                    stat[key]++;
                }
                else
                {
                    stat.Add(key, 1);
                }
            }

            Dictionary<string, int> reviewStat = new Dictionary<string, int>();
            foreach (var set in reviewValueSet)
            {
                int resultCount = set.Count;
                int nojunkCount = set.Sum();
                string key = string.Format("{0}-{1}", resultCount, nojunkCount);
                if (reviewStat.ContainsKey(key))
                {
                    reviewStat[key]++;
                }
                else
                {
                    reviewStat.Add(key, 1);
                }
            }

            using (StreamWriter sw = new StreamWriter(v))
            {
                sw.WriteLine("nojunk pair stat:");
                sw.WriteLine(string.Join("\n", stat.OrderBy(s => s.Key).Select(s => string.Format("{0}: {1}", s.Key, s.Value))));

                sw.WriteLine();
                sw.WriteLine("review pair stat:");
                sw.WriteLine(string.Join("\n", reviewStat.OrderBy(s => s.Key).Select(s => string.Format("{0}: {1}", s.Key, s.Value))));
            }
        }

        internal static void GetnerateAnalysisReportForSlice(StreamWriter sw, List<QueryMetadata> querySet, List<List<ResultMetadata>> resultSet, List<string> metricList)
        {
            sw.WriteLine("\ttotalQueryCount:{0}", querySet.Count);
            sw.WriteLine("\ttotalEntityCount:{0}", resultSet.Sum(s => s.Count()));
            foreach (var metricName in metricList)
            {
                sw.WriteLine("\t{0}:{1}", metricName, Math.Round(querySet.Where(s => s.metric.ContainsKey(metricName)).Average(s => s.metric[metricName]), 4));
            }
            sw.WriteLine();

            sw.WriteLine();
            /*
            sw.WriteLine("ResultStat:");
            sw.WriteLine("\ttotalCount:{0}", resultSet.Sum(s => s.Count()));
            sw.WriteLine("\tintentExelCount:{0}", resultSet.Sum(s => s.Count(t => t.IntentJudgment == 1)));
            sw.WriteLine("\tintentGoodCount:{0}", resultSet.Sum(s => s.Count(t => t.IntentJudgment == 2)));
            sw.WriteLine("\tintentExelWithReviewCount:{0}", resultSet.Sum(s => s.Count(t => t.IntentJudgment == 1 && t.metric["reviewscore"] > 0)));
            sw.WriteLine("\tintentExelWithHtmlCount:{0}", resultSet.Sum(s => s.Count(t => t.IntentJudgment == 1 && t.metric["urlscore"] > 0)));
            sw.WriteLine("\tintentExelWithoutCount:{0}", resultSet.Sum(s => s.Count(t => t.IntentJudgment == 1 && t.metric["urlscore"] <= 0 && t.metric["reviewscore"] <= 0)));
            sw.WriteLine("\tintentGoodWithoutCount:{0}", resultSet.Sum(s => s.Count(t => t.IntentJudgment == 2 && t.metric["urlscore"] <= 0 && t.metric["reviewscore"] <= 0)));
            sw.WriteLine("\tintentNoneWithoutCount:{0}", resultSet.Sum(s => s.Count(t => t.IntentJudgment > 2 && t.metric["urlscore"] <= 0 && t.metric["reviewscore"] <= 0)));
            */
        }

        internal static void GenerateEntitySetAnalysis(Dictionary<string, List<ResultMetadata>> resultSet)
        {
            Console.WriteLine("allEntity: {0}", resultSet.Values.Sum(s => s.Count));
            Console.WriteLine("nojunkEntity: {0}", resultSet.Values.Sum(s => s.Count(t => t.IsNoJunk)));
        }

        internal static void GenerateQuerySetAnalysis(string v, List<QueryMetadata> querySet)
        {
            Dictionary<string, int> statistic = new Dictionary<string, int>();
            using (var sw = new StreamWriter(v + "location.tsv"))
            {
                foreach (var query in querySet)
                {
                    sw.Write(query.Line);
                    sw.Write("\t{0}", query.ExplictLocationType);
                    sw.Write("\t{0}", query.ExplictLocationGuid);
                    sw.Write("\t{0}", query.ExplictLocationLat);
                    sw.Write("\t{0}", query.ExplictLocationLong);
                    sw.WriteLine();


                    var finalOption = query.GetStringAttribute("FinalTargetLocationOption");

                    AddOneStatistic(statistic, "all");
                    AddOneStatistic(statistic, finalOption);
                    /*
                    if (!query.IsImplicitLocation)
                    {
                        AddOneStatistic(statistic, finalOption + "-" + query.ExplictLocationType);

                        if (query.ExplictLocationLat != query.FinalLat || query.ExplictLocationLong != query.FinalLong)
                        {
                            AddOneStatistic(statistic, finalOption + "-difflatlong");
                        }
                    }
                    */
                }

                foreach (var pari in statistic)
                {
                    Console.WriteLine("{0}: {1}", pari.Key, pari.Value);
                }
            }
        }

        private static void AddOneStatistic(Dictionary<string, int> statistic, string v)
        {
            if (!statistic.ContainsKey(v))
            {
                statistic.Add(v, 1);
            }
            else
            {
                statistic[v]++;
            }
        }
        
        internal static void OutputEntityMetadata(string v, List<QueryMetadata> querySet, Dictionary<string, List<ResultMetadata>> resultSet, List<string> metricList)
        {
            using (StreamWriter sw = new StreamWriter(v))
            {
                OutputSchema(sw, metricList.ToArray(), resultSet.First().Value.First().Schema.ToList());
                foreach (var query in querySet)
                {
                    if (!resultSet.ContainsKey(query.EntityGuid))
                        continue;

                    var resultList = resultSet[query.EntityGuid];
                    foreach (var result in resultList)
                    {
                        foreach (var metricName in metricList)
                        {
                            var metricNamePrefix = metricName;
                            if (metricName.Contains("dcg@"))
                            {
                                var parts = metricName.Split('@');
                                int maxPosition = int.Parse(parts[1]);
                                metricNamePrefix = parts[0];
                            }

                            sw.Write("{0}\t", Math.Round(result.metric[metricNamePrefix], 2));
                        }

                        sw.Write(result.Line);

                        sw.WriteLine();
                    }
                }
            }
        }
    }
}
