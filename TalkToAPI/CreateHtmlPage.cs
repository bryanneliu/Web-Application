using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TalkToAPI
{
    static public class CreateHtmlPage
    {
        public const string PushPinFormat = "&pushpin={0},{1};{2};{3}";
        public const string BluePushpin = "4";
        public const string RedPuhspin = "7";

        private static string CreateTableRow(string key, string value, bool hiddenrow = false, string keycolor = null, string valuecolor = null)
        {
            if (string.IsNullOrEmpty(value) == null || string.IsNullOrEmpty(key) == null)
            {
                //height=\"30\"
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
        }

        public class Entity
        {
            public string Title { get; set; }
            public string Path { get; set; }
            public string Snippet { get; set; }
            public string Author { get; set; }
            public string CreatedDate { get; set; }
            public string LastModifiedDate { get; set; }

            public Entity(string title, string path, string snippet, string author, string createdDate, string lastModifiedDate)
            {
                Title = title;
                Path = path;
                Snippet = snippet;
                Author = author;
                CreatedDate = createdDate;
                LastModifiedDate = lastModifiedDate;
            }
        }
        public static string CreateHtmlFromEntitiesWithHighlights(string stack, string query, List<Entity> entities, List<string> entitiesToHighlight)
        {
            var aggregator = new Aggregator { index = 1, sb = new StringBuilder() };
            aggregator.sb.Append(string.Format("<table id=\"{0}\" border=\"2\" style=\"width:auto\">", stack));
            aggregator.sb.Append(CreateTableRow("", "Query: " + query, true));
            aggregator.sb.Append(CreateTableRow("Stack", stack, false, "brown", "brown"));
            var insertPoint = aggregator.sb.Length;
            var agg = entities.Aggregate(aggregator, (innerAggregator, entity) =>
            {
                var tempSb = new StringBuilder();
                var highlightColor = entitiesToHighlight.Contains(entity.Title) ? "red" : "darkblue";

                tempSb.Append(CreateTableRow("Title", entity.Title, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Path", entity.Path, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Snippet", entity.Snippet, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Author", entity.Author, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("CreatedDate", entity.CreatedDate, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("LastModifiedDate", entity.LastModifiedDate, false, highlightColor, highlightColor));

                innerAggregator.sb.Append(CreateTableRow(innerAggregator.index.ToString(), string.Format("<table id=\"{1}\">{0}</table>", tempSb.ToString(), innerAggregator.index.ToString()), false, highlightColor));
                innerAggregator.index++;

                return innerAggregator;
            });

            aggregator.sb.Append("</table>");
            aggregator.index = 1;
            return aggregator.sb.ToString();
        }

        public static string CreateHtmlFromResult(string stack, string query, List<Entity> entities)
        {
            if (entities == null) return string.Empty;
            var aggregator = new Aggregator { index = 1, sb = new StringBuilder() };
            aggregator.sb.Append(string.Format("<table id=\"{0}\" border=\"2\" style=\"width:auto\">", stack));
            aggregator.sb.Append(CreateTableRow("", "Query: " + query, true));
            aggregator.sb.Append(CreateTableRow("Stack", stack, false, "brown", "brown"));
            var insertPoint = aggregator.sb.Length;
            var agg = entities.Aggregate(aggregator, (innerAggregator, entity) =>
            {
                var tempSb = new StringBuilder();
                var highlightColor = "darkblue";

                tempSb.Append(CreateTableRow("Title", entity.Title, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Path", entity.Path, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Snippet", entity.Snippet, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("Author", entity.Author, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("CreatedDate", entity.CreatedDate, false, highlightColor, highlightColor));
                tempSb.Append(CreateTableRow("LastModifiedDate", entity.LastModifiedDate, false, highlightColor, highlightColor));

                innerAggregator.sb.Append(CreateTableRow(innerAggregator.index.ToString(), string.Format("<table id=\"{1}\">{0}</table>", tempSb.ToString(), innerAggregator.index.ToString()), false, highlightColor));
                innerAggregator.index++;

                return innerAggregator;
            });

            aggregator.sb.Append("</table>");
            aggregator.index = 1;
            return aggregator.sb.ToString();
        }

        /*
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

    */

        public static void CreateSBS()
        {
            Dictionary<string, List<Entity>> leftDict = ExtractQueryEntitiesFromTAP();
            Dictionary<string, List<Entity>> rightDict = ExtractQueryEntitiesFromMSW();

            var folderPrefix = @"D:\Test\recallAnalysis\SBS\";

            List<string> htmlList = new List<string>();
            htmlList.Add("SideBySideHtml");

            foreach (string key in leftDict.Keys)
            {
                List<Entity> leftEntities = leftDict[key];
                List<Entity> rightEntities = null;
                if (rightDict.ContainsKey(key))
                {
                    rightEntities = rightDict[key];
                }

                string html = CreateSBSHtmlPage(key, "TAP", "MSW", leftEntities, rightEntities);
                string fname = folderPrefix + key + ".html";
                File.WriteAllText(fname, html);
                htmlList.Add(html);
            }

            File.WriteAllLines(folderPrefix + "0000.uhrsupload.tsv", htmlList);
        }

        public static string CreateSBSHtmlPage(string query, string leftStack, string rightStack, List<Entity> leftEntities, List<Entity> rightEntities)
        {
            List<string> leftOnlyEntities = new List<string>(); // leftEntities.Where(s => rightEntities.Find(t => t.Title == s.Title) == null).Select(v => v.Title).ToList();
            List<string> rightOnlyEntities = new List<string>(); // rightEntities.Where(s => leftEntities.Find(t => t.Title == s.Title) == null).Select(v => v.Title).ToList();

            var leftHtml = CreateHtmlFromResult(leftStack, query, leftEntities); // leftOnlyEntities);
            var rightHtml = CreateHtmlFromResult(rightStack, query, rightEntities); // rightOnlyEntities);

            return string.Format("<table id=\"main\"><th style=\"font-weight:bold font-size:175%\" colspan=\"2\">Query: {0}</th><tr valign=top><td>{1}</td><td>{2}</td></tr></table>", query, leftHtml, rightHtml);
        }

        public static void CreateHtmlForTap()
        {
            string line = "";
            string prevQuery = "";
            StreamWriter sw = new StreamWriter(Constants.LocalPath_Html_Tap_Personal_Personal);
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Results_Tap_Personal_Personal);
                line = sr.ReadLine();
                List<Entity> entities = new List<Entity>();
                while (null != line)
                {
                    string[] parts = line.Split('\t');
                    string query = parts[0];
                    string author = parts[1];
                    string createdDate = parts[4];
                    string lastModifiedDate = parts[5];
                    string path = parts[8];
                    string title = parts[11];
                    string snippet = "";
                    Entity entity = new Entity(title, path, snippet, author, createdDate, lastModifiedDate);
                    if (prevQuery == "")
                    {
                        entities.Clear();
                        entities.Add(entity);
                    }
                    else if (query != prevQuery) //next query
                    {
                        string html = CreateHtmlFromResult("TAP", query, entities);
                        sw.WriteLine(query + "\t" + html);
                        entities.Clear();
                        entities.Add(entity);
                    }
                    else // same query
                    {
                        entities.Add(entity);
                    }
                    prevQuery = query;

                    line = sr.ReadLine();
                }

                sw.Flush();
                sw.Close();
                sr.Close();
            }
            catch (IOException e)
            {
            }
        }

        public static void CreateHtmlForMSW()
        {
            string line = "";
            string prevQuery = "";
            StreamWriter sw = new StreamWriter(Constants.LocalPath_Html_MSW_Personal_Personal);
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Results_MSW_Personal_Personal);
                line = sr.ReadLine();
                List<Entity> entities = new List<Entity>();
                while (null != line)
                {
                    string[] parts = line.Split('\t');
                    string query = parts[0];
                    string author = parts[4];
                    string createdDate = parts[5];
                    if (createdDate == "1/1/0001 12:00:00 AM") createdDate = "";
                    string lastModifiedDate = parts[6];
                    string path = parts[7];
                    string title = parts[8];
                    string snippet = parts[9];
                    Entity entity = new Entity(title, path, snippet, author, createdDate, lastModifiedDate);
                    if (prevQuery == "")
                    {
                        entities.Clear();
                        entities.Add(entity);
                    }
                    else if (query != prevQuery) //next query
                    {
                        string html = CreateHtmlFromResult("MSW", query, entities);
                        sw.WriteLine(query + "\t" + html);
                        entities.Clear();
                        entities.Add(entity);
                    }
                    else // same query
                    {
                        entities.Add(entity);
                    }
                    prevQuery = query;

                    line = sr.ReadLine();
                }

                sw.Flush();
                sw.Close();
                sr.Close();
            }
            catch (IOException e)
            {
            }
        }

        public static Dictionary<string, List<Entity>> ExtractQueryEntitiesFromMSW()
        {
            Dictionary<string, List<Entity>> dict = new Dictionary<string, List<Entity>>();
            string line = "";
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Results_MSW_Personal_Personal);
                line = sr.ReadLine();
                List<Entity> entities = new List<Entity>();
                while (null != line)
                {
                    string[] parts = line.Split('\t');
                    string query = parts[0].ToLower();
                    string author = parts[4];
                    string createdDate = parts[5];
                    if (createdDate == "1/1/0001 12:00:00 AM") createdDate = "";
                    string lastModifiedDate = parts[6];
                    string path = parts[7];
                    string title = parts[8];
                    string snippet = parts[9];
                    Entity entity = new Entity(title, path, snippet, author, createdDate, lastModifiedDate);

                    if (!dict.ContainsKey(query))
                    {
                        dict[query] = new List<Entity>();
                    }
                    dict[query].Add(entity);

                    line = sr.ReadLine();
                }
                sr.Close();
                return dict;
            }
            catch (IOException e)
            {
            }

            return null;
        }

        public static Dictionary<string, List<Entity>> ExtractQueryEntitiesFromTAP()
        {
            Dictionary<string, List<Entity>> dict = new Dictionary<string, List<Entity>>();
            string line = "";
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Results_Tap_Personal_Personal);
                line = sr.ReadLine();
                List<Entity> entities = new List<Entity>();
                while (null != line)
                {
                    string[] parts = line.Split('\t');
                    string query = parts[0].ToLower();
                    string author = parts[1];
                    string createdDate = parts[4];
                    string lastModifiedDate = parts[5];
                    string path = parts[8];
                    string title = parts[11];
                    string snippet = parts[29];
                    Entity entity = new Entity(title, path, snippet, author, createdDate, lastModifiedDate);

                    /*
                    if (prevQuery == "")      
                    {
                        entities.Clear();
                        entities.Add(entity);
                    }
                    else if (query != prevQuery) //next query
                    {
                        dict[prevQuery] = entities;   //dict is passing reference
                        entities.Clear();
                        entities.Add(entity);
                    }
                    else // same query
                    {
                        entities.Add(entity);
                    }
                    */

                    if (!dict.ContainsKey(query))
                    {
                        dict[query] = new List<Entity>();
                    }
                    dict[query].Add(entity);

                    line = sr.ReadLine();
                }

                sr.Close();
                return dict;
            }
            catch (IOException e)
            {
            }
            return null;
        }
    }
}
