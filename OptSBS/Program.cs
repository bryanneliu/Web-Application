using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace RecalculateLDCG
{
    class Program
    {
        static void Main(string[] args)
        {
            if (string.Compare(args[0], "metric", true)==0)
            {
                var querySet = QueryMetadata.ReadQueryMetadata(args[1]);
                var resultSet = ResultMetadata.ReadResultMetadata(args[2]);

                // remove result set which entity id is not in query set
                resultSet = ResultMetadata.TrimResultSet(resultSet, querySet);

                var metricList = new List<string>();
                for (int i = 6; i < args.Length; i++)
                {
                    var metricName = args[i].ToLower();
                    var metricValue = MetricCalculator.CalculateMetric(metricName, querySet, resultSet);
                    metricList.Add(metricName);
                    Console.WriteLine("{0}: {1}", metricName, Math.Round(metricValue, 4));
                }

                MetricLogger.GenerateAnalysisReport(args[3], querySet, resultSet, metricList);
                MetricLogger.OutputQueryMetadata(args[4], querySet, metricList);
                MetricLogger.OutputEntityMetadata(args[5], querySet, resultSet, metricList);
                MetricLogger.GenerateQuerySetAnalysis(args[1], querySet);
                MetricLogger.GenerateEntitySetAnalysis(resultSet);
            }
            else if (args[0].StartsWith("removejunk"))
            {
                var querySet = QueryMetadata.ReadQueryMetadata(args[1]);
                var resultSet = ResultMetadata.ReadResultMetadata(args[2]);
                JunkFilter.DemoteJunk(args[0], querySet, resultSet);
                MetricLogger.OutputEntityMetadata(args[3], querySet, resultSet, new List<string>());
            }
            else if (string.Compare(args[0], "trim", true) == 0)
            {
                var querySet = QueryMetadata.ReadQueryMetadata(args[1]);
                var resultSet = ResultMetadata.ReadResultMetadata(args[2]);
                JunkFilter.TrimResult(resultSet);
                MetricLogger.OutputEntityMetadata(args[3], querySet, resultSet, new List<string>());
            }
            else if (string.Compare(args[0], "statjunk", true) == 0)
            {
                var querySet = QueryMetadata.ReadQueryMetadata(args[1]);
                var resultSet = ResultMetadata.ReadResultMetadata(args[2]);
                MetricLogger.StatisticJunk(args[3], querySet, resultSet);
            }
            else if (string.Compare(args[0], "createsbs", true) == 0)
            {
                var querySet = QueryMetadata.ReadQueryMetadata(args[1]);
                var resultSet1 = ResultMetadata.ReadResultMetadata(args[2]);
                var resultSet2 = ResultMetadata.ReadResultMetadata(args[3]);
                Dictionary<string, List<ResultMetadata>> refSet = null;
                if (args.Length > 5)
                {
                    refSet = ResultMetadata.ReadResultMetadata(args[4]);
                }

                var folderPrefix = args.Last();

                List<string> htmlList = new List<string>();
                htmlList.Add("SideBySideHtml");
                foreach (var query in querySet)
                {
                    if (resultSet1.ContainsKey(query.EntityGuid) && resultSet2.ContainsKey(query.EntityGuid))
                    {
                        string key1 = string.Concat(resultSet1[query.EntityGuid].Select(s => s.EngineEntityId));
                        string key2 = string.Concat(resultSet2[query.EntityGuid].Select(s => s.EngineEntityId));
                        if (key1 != key2)
                        {
                            var html = SBSHtml.GetSideBySideHtml(query.Text, "LEFT", "RIGHT", "REFERENCE", resultSet1[query.EntityGuid], resultSet2[query.EntityGuid], refSet == null || !refSet.ContainsKey(query.EntityGuid) ? null : refSet[query.EntityGuid]);
                            string fname = folderPrefix + query.EntityGuid + ".html";
                            File.WriteAllText(fname, html);
                            htmlList.Add(html);
                        }
                    }
                }

                Console.WriteLine("queryCount:{0}, set1Count:{1}, set2Count:{2}, diffCount:{3}",
                    querySet.Count, resultSet1.Count, resultSet2.Count, htmlList.Count - 1);
                File.WriteAllLines(folderPrefix + "0000.uhrsupload.tsv", htmlList);
            }
        }
    }
}
