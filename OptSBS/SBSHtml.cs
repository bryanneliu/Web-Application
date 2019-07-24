using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RecalculateLDCG
{
    public class SBSHtml
    {
        public const string MapFormat = "<img src=\"http://dev.virtualearth.net/REST/v1/Imagery/Map/Road?format=jpeg&key=AnN98KKBTUeehdchVMIDr-iDdVQPpru5W-M0ZFPri_9KhsUYJWtwPGhIrDMpyJTQ{0}\"/>";
        public const string PushPinFormat = "&pushpin={0},{1};{2};{3}";
        public const string BluePushpin = "4";
        public const string RedPuhspin = "7";

        private static string CreateTableRow(string key, string value, bool hiddenrow = false, string keycolor = null, string valuecolor = null)
        {
            if (string.IsNullOrEmpty(value) == null || string.IsNullOrEmpty(key) == null)
            {
                return string.IsNullOrEmpty(key) == null
                    ? string.Format("<tr {1} class=\"{2}\"><td colspan=\"2\" style=\"color:{3}\">{0}</td></tr>", value, hiddenrow ? "style=\"display:none\"" : "", "valueonly", valuecolor ?? "black")
                    : string.Format("<tr {1} class=\"{2}\"><td colspan=\"2\" style=\"color:{3}\">{0}</td></tr>", key, hiddenrow ? "style=\"display:none\"" : "", key.Trim(), keycolor ?? "black");
            }

            return string.Format("<tr {2} class=\"{3}\"><td style=\"color:{4};padding-right:10\">{0}</td><td style=\"color:{5}\">{1}</td></tr>"
                , key, value, hiddenrow ? "style=\"display:none\"" : "", key.Trim(), keycolor ?? "black", valuecolor ?? "black");
        }

        private class Aggregator
        {
            public int index { get; set; }
            public StringBuilder sb { get; set; }
//            public Dictionary<int, Location> locations { get; set; }
        }

        public static string CreateHtmlFromEntitiesWithHighlights(string stack, string query, List<ResultMetadata> entities, List<string> entitiesToHighlight)
        {
            var aggregator = new Aggregator { index = 1, sb = new StringBuilder() };
            aggregator.sb.Append(string.Format("<table id=\"{0}\" border=\"2\" style=\"width:auto\">", stack));
            aggregator.sb.Append(CreateTableRow("", "Query: " + query, true));
            aggregator.sb.Append(CreateTableRow("Stack", stack, false, "brown", "brown"));
            var insertPoint = aggregator.sb.Length;
            var agg = entities.Aggregate(aggregator, (innerAggregator, entity) =>
            {
                var tempSb = new StringBuilder();
                var highlightColor = entitiesToHighlight.Contains(entity.EngineEntityId) ? "red" : "darkblue";

                /// Name
                tempSb.Append(CreateTableRow("Name", entity.Name.Trim('*', ' '), false, highlightColor, highlightColor));

                /// Rating
                /// 
                tempSb.Append(CreateTableRow("Rating", (entity.Rating > 0) ? entity.Rating.ToString() : " ", false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Rating Count", (entity.ReviewCount > 0) ? entity.ReviewCount.ToString() : " ", false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Primary URL", (string.IsNullOrWhiteSpace(entity.Url) || entity.Url.Length <= 60) ? entity.Url : entity.Url.Substring(0, 60), false, highlightColor, highlightColor));

                /// url
                /// 
                tempSb.Append(CreateTableRow("Address"
                    , string.Format("{0},{1} {2}", entity.Street, entity.City, entity.State)
                    , false, highlightColor, highlightColor));

                tempSb.Append(CreateTableRow("LineDistance"
                    , string.Format("{0}", Math.Round(entity.LineDistance, 2))
                    , false, highlightColor, highlightColor));

                //Add for Popularity
                tempSb.Append(CreateTableRow("P_Rating", (entity.P_Rating > 0) ? entity.P_Rating.ToString() : " ", false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("P_ReviewCount", (entity.P_ReviewCount > 0) ? entity.P_ReviewCount.ToString() : " ", false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("P_FBLikesCount", (entity.P_FBLikesCount > 0) ? entity.P_FBLikesCount.ToString() : " ", false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("P_EntityRelatedClickCount", (entity.P_EntityRelatedClickCount > 0) ? entity.P_EntityRelatedClickCount.ToString() : " ", false, highlightColor, highlightColor));
            //tempSb.Append(CreateTableRow("EntityRelatedUrlsClickCount", (entity.EntityRelatedUrlsClickCount > 0) ? entity.EntityRelatedUrlsClickCount.ToString() : " ", false, highlightColor, highlightColor));
            //tempSb.Append(CreateTableRow("SlapiLog_EntityImpressionCount_1Month", (entity.SlapiLog_EntityImpressionCount_1Month > 0) ? entity.SlapiLog_EntityImpressionCount_1Month.ToString() : " ", false, highlightColor, highlightColor));
            //tempSb.Append(CreateTableRow("SlapiLog_EntityClickCount_1Month", (entity.SlapiLog_EntityClickCount_1Month > 0) ? entity.SlapiLog_EntityClickCount_1Month.ToString() : " ", false, highlightColor, highlightColor));

            innerAggregator.sb.Append(CreateTableRow(innerAggregator.index.ToString(), string.Format("<table id=\"{1}\">{0}</table>", tempSb.ToString(), innerAggregator.index.ToString()), false, highlightColor));
                innerAggregator.index++;

                return innerAggregator;
            });

            // TODO: Create map on top
            aggregator.sb.Append("</table>");

            aggregator.index = 1;

            var mapsLinks = CreateTableRow("",
                string.Format(
                MapFormat,
                entities.Select((x, i) => new { index = i + 1, entity = x }).Aggregate(
                    new StringBuilder(),
                    (pushpins, kv)
                        => pushpins.AppendFormat(PushPinFormat, kv.entity.Lat, kv.entity.Long, entitiesToHighlight.Contains(kv.entity.EngineEntityId) ? RedPuhspin : BluePushpin, kv.index))));
                
            aggregator.sb.Insert(insertPoint, mapsLinks);
            return aggregator.sb.ToString();
        }

        public static string GetSideBySideHtml(string query, string leftStack, string rightStack, List<ResultMetadata> leftEntities, List<ResultMetadata> rightEntities)
        {
            var leftOnlyEntities = leftEntities.Where(s => rightEntities.Find(t => t.EngineEntityId == s.EngineEntityId) == null).Select(v => v.EngineEntityId).ToList();
            var rightOnlyEntities = rightEntities.Where(s => leftEntities.Find(t => t.EngineEntityId == s.EngineEntityId) == null).Select(v => v.EngineEntityId).ToList();

            var leftHtml = CreateHtmlFromEntitiesWithHighlights(leftStack, query, leftEntities, leftOnlyEntities);
            var rightHtml = CreateHtmlFromEntitiesWithHighlights(rightStack, query, rightEntities, rightOnlyEntities);

            return string.Format("<table id=\"main\"><th style=\"font-weight:bold font-size:175%\" colspan=\"2\">Query: {0}</th><tr valign=top><td>{1}</td><td>{2}</td></tr></table>", query, leftHtml, rightHtml);
        }

        public static string GetSideBySideHtml(string query, string leftStack, string rightStack, string refStack, List<ResultMetadata> leftEntities, List<ResultMetadata> rightEntities, List<ResultMetadata> refEntities)
        {
            var leftOnlyEntities = leftEntities.Where(s => rightEntities.Find(t => t.EngineEntityId == s.EngineEntityId) == null).Select(v => v.EngineEntityId).ToList();
            var rightOnlyEntities = rightEntities.Where(s => leftEntities.Find(t => t.EngineEntityId == s.EngineEntityId) == null).Select(v => v.EngineEntityId).ToList();

            var leftHtml = CreateHtmlFromEntitiesWithHighlights(leftStack, query, leftEntities, leftOnlyEntities);
            var rightHtml = CreateHtmlFromEntitiesWithHighlights(rightStack, query, rightEntities, rightOnlyEntities);
            var refHtml = refEntities != null ? CreateHtmlFromEntitiesWithHighlights(refStack, query, refEntities, new List<string>()) : string.Empty;

            return string.Format("<table id=\"main\"><th style=\"font-weight:bold font-size:175%\" colspan=\"2\">Query: {0}</th><tr valign=top><td>{1}</td><td>{2}</td><td>{3}</td></tr></table>", query, leftHtml, rightHtml, refHtml);
        }
    }
}