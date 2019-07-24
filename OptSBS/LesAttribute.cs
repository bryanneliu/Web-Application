using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

//using System.Web.Script.Serialization;

namespace RecalculateLDCG
{
    [CLSCompliant(false)]
    public class LesAttribute
    {
        public string QueryText { get; set; }

        [DataContract]
        public struct Attribute
        {
            [DataMember(Name = "token", Order = 0)]
            public string Token { get; set; }

            [DataMember(Name = "location", Order = 1)]
            public string LocationResult { get; set; }

            [DataMember(Name = "lat", Order = 2)]
            public string Latitude { get; set; }

            [DataMember(Name = "long", Order = 3)]
            public string Longitude { get; set; }

            [DataMember(Name = "type", Order = 4)]
            public string Type { get; set; }

            [DataMember(Name = "userdistance", Order = 5)]
            public string UserDistance { get; set; }

            [DataMember(Name = "metadata", Order = 6)]
            public string Metadata { get; set; }

            [DataMember(Name = "confidence", Order = 7)]
            public string Confidence { get; set; }

            [DataMember(Name = "startoffset", Order = 8)]
            public string StartOffset { get; set; }

            [DataMember(Name = "endoffset", Order = 9)]
            public string EndOffset { get; set; }

            [DataMember(Name = "geospatialid", Order = 10)]
            public string GeospatialId { get; set; }

            [DataMember(Name = "expectedentitytype", Order = 11)]
            public string ExpectedEntityType { get; set; }

            [DataMember(Name = "leasattributesource", Order = 12)]
            public string LesAttributeSource { get; set; }
        }

        public readonly Attribute _attribute;

        public LesAttribute(string queryText, string token, string latitude, string longitude, string locationResult,
                            string locationType, string userDistance, string metaData, string confidence,
                            string startOffset, string endOffset, string geospatialId, string expectedEntityType,
                            string lesAttributeSource)
        {
            QueryText = queryText;

            // clean the invalid value
            if (expectedEntityType != null && expectedEntityType.Contains("--Select Entity Type--")) expectedEntityType = null;

            _attribute = new Attribute
            {
                Token = JsonConvert.ToString(token).Trim(new Char[] { ' ', '"' }),
                Latitude = latitude.Trim(),
                Longitude = longitude.Trim(),
                LocationResult = JsonConvert.ToString(locationResult).Trim(new Char[] { ' ', '"' }),
                Type = locationType.Trim(),
                UserDistance = userDistance.Trim(),
                Metadata = metaData.Trim(),
                Confidence = confidence.Trim(),
                StartOffset = startOffset.Trim(),
                EndOffset = endOffset.Trim(),
                GeospatialId = geospatialId,
                ExpectedEntityType = expectedEntityType,
                LesAttributeSource = lesAttributeSource
            };
        }

        public LesAttribute(string lesAttributes)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(lesAttributes.Trim(new[] { '[', ']' })));
            var serializer = new DataContractJsonSerializer(typeof(Attribute));
            _attribute = (Attribute)serializer.ReadObject(ms);
            ms.Close();
        }

        public string GetLesAttributes()
        {
            if (string.IsNullOrEmpty(_attribute.Token) && string.IsNullOrEmpty(_attribute.LocationResult)) return null;

            var stream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(Attribute));
            serializer.WriteObject(stream, _attribute);
            stream.Position = 0;
            var sr = new StreamReader(stream);
            return "[" + sr.ReadToEnd() + "]";
        }
    }
}
