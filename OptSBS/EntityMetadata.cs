using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RecalculateLDCG
{
    public class ResultMetadata
    {
        public string Line;

        private Dictionary<string, string> metadatas;

        public Dictionary<string, double> metric;

        public string Name;

        public string Street;

        public string City;

        public string State;

        public bool IsNoJunk { get { return ReviewCount > 3 || Rating > 0 || !string.IsNullOrWhiteSpace(Url);  } }

        public bool HasReview {  get { return ReviewCount > 3 || Rating > 0; } }

        public string EngineEntityId { set; get; }
        public string EntityGuid { set; get; }

        public int IntentJudgment { set; get; }

        public int DistanceJudgment { set; get; }

        public int Popularity { set; get; }

        public string Url { set; get; }

        public double LineDistance { set; get; }

        public string AnswerType { set; get; }

        public double ManualInnerRadius { set; get; }

        public double ManualOuterRadius { set; get; }

        public string FinalTargetLocationOption { set; get; }

        public int IsPopularityIntent { set; get; }

        public int ReviewCount { set; get; }

        public int Rating { set; get; }

        public int Position { set; get; }

        public double Lat { set; get; }

        public double Long { set; get; }

        //Add additinal attributes for popularity
        public int P_ReviewCount { set; get; }
        public double P_Rating { set; get; }
        public int P_FBLikesCount { set; get; }
        public int P_EntityRelatedClickCount { set; get; }
        //public int EntityRelatedUrlsClickCount { set; get; }
        //public int SlapiLog_EntityImpressionCount_1Month { set; get; }
        //public int SlapiLog_EntityClickCount_1Month { set; get; }

        public string[] Schema { set; get; }

        public ResultMetadata(string inputline, string[] schema)
        {
            Line = inputline;
            string[] parts = inputline.Split('\t');
            if (parts.Length != schema.Length)
            {
                throw new Exception("schema doesn't match");
            }

            Schema = schema;
            metadatas = new Dictionary<string, string>();
            for (int i = 0; i < schema.Length; i++)
            {
                metadatas.Add(schema[i], parts[i]);
            }

            Name = GetStringAttribute("EntityTitle");
            Street = GetStringAttribute("EntityStreet");
            City = GetStringAttribute("EntityCity");
            State = GetStringAttribute("EntityState");

            EntityGuid = GetStringAttribute("EntityGuid");

            var judgmentID = GetIntAttribute("JudgmentID");

            IntentJudgment = judgmentID / 10;
            DistanceJudgment = judgmentID % 10;
            Popularity = GetIntAttribute("PopularityScore", 0);
            Url = GetStringAttribute("EntityPrimaryWebsite");
            LineDistance = GetDoubleAttribute("LineDistance", 1000);

            AnswerType = GetStringAttribute("AnswerType");
            FinalTargetLocationOption = GetStringAttribute("FinalTargetLocationOption");
            EngineEntityId = GetStringAttribute("EngineEntityId");

            ManualInnerRadius = GetDoubleAttribute("ManualInnerRadius", -1);
            ManualOuterRadius = GetDoubleAttribute("ManualOuterRadius", -1);
            IsPopularityIntent = GetIntAttribute("IsPopularityIntent", 0);
            ReviewCount = GetIntAttribute("EntityReviewCount", -1);
            Rating = (int)(0.5 + GetDoubleAttribute("EntityRating", -1));
            Position = GetIntAttribute("Position", 0);
            Lat = GetDoubleAttribute("EntityLatitude", 0);
            Long = GetDoubleAttribute("EntityLongitude", 0);

            //Add for Popularity
            P_ReviewCount = GetIntAttribute("ReviewCount");
            P_Rating = GetDoubleAttribute("Rating");
            P_FBLikesCount = GetIntAttribute("FBLikesCount");
            P_EntityRelatedClickCount = GetIntAttribute("EntityRelatedClickCount");
            //EntityRelatedUrlsClickCount = GetIntAttribute("EntityRelatedUrlsClickCount");
            //SlapiLog_EntityImpressionCount_1Month = GetIntAttribute("SlapiLog_EntityImpressionCount");
            //SlapiLog_EntityClickCount_1Month = GetIntAttribute("SlapiLog_EntityClickCount");

            metric = new Dictionary<string, double>();
        }

        public void UpdatePostion(int newPosition)
        {
            if (newPosition != Position)
            {
                metadatas["Position"] = newPosition.ToString();
                Line = string.Join("\t", Schema.Select(s => metadatas[s]));
                /*
                int offset = 0;
                for(int i = 0; i < Schema.Length; i++)
                {
                    if (Schema[i] == "Position")
                    {
                        break;
                    }

                    offset += metadatas[Schema[i]].Length + 1;
                }

                offset--;
                */
            }
        }

        public string GetStringAttribute(string key)
        {
            return metadatas.ContainsKey(key) ? metadatas[key] : null;
        }

        public int GetIntAttribute(string key, int defaultValue = -1)
        {
            return metadatas.ContainsKey(key) && !string.IsNullOrWhiteSpace(metadatas[key]) ? int.Parse(metadatas[key]) : defaultValue;
        }

        public double GetDoubleAttribute(string key, double defaultValue = -1)
        {
            return metadatas.ContainsKey(key) && !string.IsNullOrWhiteSpace(metadatas[key]) ? double.Parse(metadatas[key]) : defaultValue;
        }

        public static Dictionary<string, List<ResultMetadata>> ReadResultMetadata(string fileName)
        {
            var resultSet = new Dictionary<string, List<ResultMetadata>>();

            var lines = File.ReadAllLines(fileName);
            var schema = lines[0].Split('\t');
            for (int i = 1; i < lines.Length; i++)
            {
                var result = new ResultMetadata(lines[i], schema);
                var entityGuid = result.EntityGuid;

                if (!resultSet.ContainsKey(entityGuid))
                {
                    resultSet.Add(entityGuid, new List<ResultMetadata> { result });
                }
                else
                {
                    resultSet[entityGuid].Add(result);
                }
            }

            foreach (var resultList in resultSet.Values)
            {
                var newList = resultList.OrderBy(s => s.Position).ToList();
                resultList.Clear();
                resultList.AddRange(newList);
            }

            return resultSet;
        }


        internal static Dictionary<string, List<ResultMetadata>> TrimResultSet(Dictionary<string, List<ResultMetadata>> resultSet, List<QueryMetadata> querySet)
        {
            var newResultSet = new Dictionary<string, List<ResultMetadata>>();
            foreach (var query in querySet)
            {
                if (resultSet.ContainsKey(query.EntityGuid))
                {
                    newResultSet.Add(query.EntityGuid, resultSet[query.EntityGuid]);
                }
            }

            return newResultSet;
        }
    }
}
