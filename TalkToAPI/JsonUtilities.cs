using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TalkToAPI
{
    public class Query
    {
        public string[] SearchTerms;
    }

    public class MSWDoc
    {
        public string Id { get; set; }
        public string ContentSource { get; set; }
        public Source source;
    }

    public class Source
    {
        public string Author { get; set; }
        public string FileType { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string HitHighlightedSummary { get; set; }
    }
    public class Doc
    {
        public string Author { get; set; }
        public string FileType { get; set; }
        public string Id { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public DateTime? LastModifiedByMeTime { get; set; }
        public DateTime? LastViewedByMeTime { get; set; }
        public string Path { get; set; }
        public string RedirectUrl { get; set; }
        public string HashedPath { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public double Views { get; set; }
        public long Size { get; set; }
        public long ViewLast7days { get; set; }
        public string PiSearchResultId { get; set; }
        public double GraphRankingScore { get; set; }
        public double SearchRankingScore { get; set; }
        public double MergedRankingScore { get; set; }
        public double ReUseScore { get; set; }
        public double TrendingAroundMeScore { get; set; }
        public double ModifiedByMeScore { get; set; }
        public double ViewedByMeScore { get; set; }
        public bool IsExternalContent { get; set; }
        public string ObjectEmbeddings { get; set; }
        public string SPWebUrl { get; set; }
        public double RankInSearchResult { get; set; }
        public string StatusKey { get; set; }
        public double FastRankRankingScore { get; set; }

        /*
        private string field;

        public string Author {
            get
            {
                return field;
             }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception();
                }
                field = value;
            }
        }
        */
    }

    static public class Utilities
    {
        //{"Items":null,"GraphSearchPageImpression":null,"SearchPageImpression":null,"GraphSearchCorrelationId":null,"SearchCorrelationId":null,"GraphSearchSourceId":null,"SearchSourceId":null,"GraphSearchQueryId":null,"SearchQueryId":null,"Tracking":null}
        static public List<Doc> JsonSerializer(string content)
        {
            List<Doc> docList = new List<Doc>();
            if (string.IsNullOrEmpty(content)) return docList;
            if (content.Contains("\"Items\":null")) return docList;
            JToken rss = JObject.Parse(content);
            if (rss["Items"] == null) return docList;
            JArray docs = (JArray)rss["Items"];
            foreach (JToken doc in docs)
            {
                docList.Add(doc.ToObject<Doc>());
            }
            return docList;
        }

        static public List<MSWDoc> JsonSerializerMSWDoc(string content)
        {
            List<MSWDoc> docList = new List<MSWDoc>();
            JToken rss = JObject.Parse(content);

            //if (!(rss["EntitySets"] && rss["EntitySets"]["0"] && rss["EntitySets"]["0"]["ResultSets"] && rss["EntitySets"]["0"]["ResultSets"]["0"] && rss["EntitySets"]["0"]["ResultSets"]["0"]["Results"])) return docList;

            JArray docs = (JArray)rss.SelectToken("EntitySets[0].ResultSets[0].Results");
            //JArray docs = (JArray)rss["EntitySets"]["0"]["ResultSets"]["0"]["Results"];
            if (docs == null) return docList;
            foreach (JToken doc in docs)
            {
                docList.Add(doc.ToObject<MSWDoc>());
            }
            return docList;
        }

        static public Query JsonSerailizerQuery(string content)
        {
            JToken rss = JObject.Parse(content);
            JToken query = rss.SelectToken("SearchTerms");
            return query.ToObject<Query>();
        }

        static public string GetQuery(string content)
        {
            string[] sep = { "\"SearchTerms\":[" };
            string[] parts = content.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            int index = parts[1].IndexOf(']');
            string query = parts[1].Substring(0, index).Replace("\"", "").Replace(",", " ");
            return query;
        }

        static public string NormalizeJson(string content)
        {
            string[] sep = { "\"Author\":" };
            string[] parts = content.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string newContent = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("["))
                {
                    int index = parts[i].IndexOf(']');
                    string author = parts[i].Substring(1, index - 1).Replace("\"", "");
                    newContent += "\"Author\":" + "\"" + author + "\"" + parts[i].Substring(index + 1);
                }
                else
                {
                    newContent += "\"Author\":" + parts[i];
                }
            }
            return newContent;
        }

        static public int isNullOrEmptyOrZero(string str)
        {
            if (string.IsNullOrEmpty(str) || str == "0")
            {
                return 1;
            }
            return 0;
        }

        static public int isNull(DateTime? dt)
        {
            if (dt == null) return 1;
            return 0;
        }

        static public int isFalse(bool value)
        {
            if (value == false) return 1;
            return 0;
        }

        static public int isZero(double score)
        {
            if (score == 0) return 1;
            return 0;
        }

        static public void setDictionaryValue(Dictionary<string, int> count, string key, int value)
        {
            if (count.ContainsKey(key))
            {
                count[key] += value;
            }
            else
            {
                count[key] = value;
            }
        }
    }
}
