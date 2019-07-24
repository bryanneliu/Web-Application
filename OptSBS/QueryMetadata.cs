using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RecalculateLDCG
{
    public class QueryMetadata
    {
        private Dictionary<string, string> metadatas;

        public Dictionary<string, double> metric;

        public List<string> slice;

        public string[] Schema { set; get; }

        public List<string> MetricSchema { set; get; }

        public string Line { set; get; }

        public string Text { set; get; }
        public string EntityGuid { set; get; }

        public int Frequency { set; get; }

        public int IsLocal { set; get; }

        public int IsSingle { set; get; }

        public double ManualInnerRadius { set; get; }

        public double ManualOuterRadius { set; get; }

        public bool IsImplicitLocation { set; get; }

        public string ExplictLocationGuid { set; get; }

        public string ExplictLocationType { set; get; }

        public double ExplictLocationLat { set; get; }

        public double ExplictLocationLong { set; get; }

        public double FinalLat { set; get; }

        public double FinalLong { set; get; }

        public string Segment { set; get; }

        public QueryMetadata(string inputline, string[] schema)
        {
            Line = inputline;
            string[] parts = inputline.Split('\t');
            if (parts.Length > schema.Length)
            {
                throw new Exception("schema doesn't match");
            }

            if (parts.Length < schema.Length)
            {
                var oldparts = parts;
                parts = new string[schema.Length];
                for (int i = 0; i < schema.Length; i++)
                {
                    if (i < oldparts.Length)
                    {
                        parts[i] = oldparts[i];
                    }
                    else
                    {
                        parts[i] = null;
                        Line += "\t";
                    }
                }
            }

            Schema = schema;

            metadatas = new Dictionary<string, string>();
            for (int i = 0; i < schema.Length; i++)
            {
                metadatas.Add(schema[i], parts[i]);    
            }

            Text = GetStringAttribute("QueryText");
            EntityGuid = GetStringAttribute("EntityGuid");

            ManualInnerRadius = GetDoubleAttribute("ManualInnerRadius");

            ManualOuterRadius = GetDoubleAttribute("ManualOuterRadius");

            IsImplicitLocation = GetStringAttribute("FinalTargetLocationOption") == "UserLocation";

            string FinalTargetLocation = GetStringAttribute("FinalTargetLocation");

            string[] latlong = FinalTargetLocation.Split(',');
            FinalLat = double.Parse(latlong[0]);
            FinalLong = double.Parse(latlong[1]);

            Frequency = GetIntAttribute("Frequency", 0);
            IsLocal = GetIntAttribute("IsLocal", 0);
            IsSingle = GetIntAttribute("IsSingleResult", 0);

            if (IsImplicitLocation == false)
            {
                ParseExplicitLocation(GetStringAttribute("LESAttributes"));
            }

            metric = new Dictionary<string, double>();

            MetricSchema = new List<string>();

            Segment = GetStringAttribute(@"en-us LocalCategory_CPQ_Mobile-1511:Segment");
        }

        private void ParseExplicitLocation(string lesAttribute)
        {
            string guidKey = "geospatialid\":\"";
            string typeKey = "type\":\"";
            string latKey = "lat\":\"";
            string longKey = "long\":\"";

            int index = lesAttribute.IndexOf(guidKey);
            if (index > 0)
            {
                ExplictLocationGuid = lesAttribute.Substring(index + guidKey.Length, 36);
            }

            ExplictLocationGuid = GetJsonValue(lesAttribute, guidKey);

            ExplictLocationType = GetJsonValue(lesAttribute, typeKey);

            string latlong = string.Empty;
            latlong = GetJsonValue(lesAttribute, latKey);
            if (!string.IsNullOrWhiteSpace(latlong))
            {
                ExplictLocationLat = double.Parse(latlong);
            }

            latlong = GetJsonValue(lesAttribute, longKey);
            if (!string.IsNullOrWhiteSpace(latlong))
            {
                ExplictLocationLong = double.Parse(latlong);
            }
        }

        private string GetJsonValue(string jsonString, string key)
        {
            int index = jsonString.IndexOf(key);
            if (index > 0)
            {
                int endIndex = jsonString.IndexOf("\",\"", index);
                if (endIndex > 0)
                {
                    return jsonString.Substring(index + key.Length, endIndex - index - key.Length);
                }
            }

            return string.Empty;
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

        public static List<QueryMetadata> ReadQueryMetadata(string fileName)
        {
            var querySet = new List<QueryMetadata>();

            var lines = File.ReadAllLines(fileName);
            var schema = lines[0].Split('\t');
            for (int i = 1; i < lines.Length; i++)
            {
                var query = new QueryMetadata(lines[i], schema);
                querySet.Add(query);
            }

            return querySet;
        }

        public static void AnalyzeQuerySet(List<QueryMetadata> querySet)
        {

        }
    }
}
