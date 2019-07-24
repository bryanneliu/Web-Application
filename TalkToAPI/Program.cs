using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

namespace TalkToAPI
{
    class SearchRecallAnalysis
    {
        static public List<Doc> getOLSSearchResults()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_OLS);
            string strWebData = HtmlUtilities.downloadHtmlWithHeader(Constants.HttpRequest_OLS, Constants.Flight_OLS, Constants.LocalPath_HttpResponse_OLS);
            List<Doc> docList = Utilities.JsonSerializer(strWebData);
            foreach (Doc doc in docList)
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}",
                    doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                    doc.Path, doc.RedirectUrl, doc.HashedPath, //doc.Summary, 
                    doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                    doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                    doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore);
            }
            sw.Flush();
            sw.Close();

            return docList;
        }

        static public List<Doc> get3SZeroTermSearchResults()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_ThreeS);
            string strWebData = HtmlUtilities.downloadHtmlWithHeader(Constants.HttpRequest_ThreeS, Constants.Flight_ThreeS, Constants.LocalPath_HttpResponse_ThreeS);
            List<Doc> docList = Utilities.JsonSerializer(strWebData);

            foreach (Doc doc in docList)
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}",
                    doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                    doc.Path, doc.RedirectUrl, doc.HashedPath, //doc.Summary, 
                    doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                    doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                    doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore);
            }

            //sw.WriteLine(strWebData);
            sw.Flush();
            sw.Close();

            return docList;
        }

        static public List<Doc> getQFSearchResults()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_QF);
            string strWebData = HtmlUtilities.downloadHtmlWithHeader(Constants.HttpRequest_QF, Constants.Flight_QF, Constants.LocalPath_HttpResponse_QF);
            List<Doc> docList = Utilities.JsonSerializer(strWebData);

            foreach (Doc doc in docList)
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}",
                    doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                    doc.Path, doc.RedirectUrl, doc.HashedPath, //doc.Summary, 
                    doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                    doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                    doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore);
            }

            //sw.WriteLine(strWebData);
            sw.Flush();
            sw.Close();

            return docList;
        }

        static public List<Doc> getSPOSearchResults()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_SPO);
            string strWebData = HtmlUtilities.downloadHtmlWithHeader(Constants.HttpRequest_SPO, Constants.Flight_SPO, Constants.LocalPath_HttpResponse_SPO);
            List<Doc> docList = Utilities.JsonSerializer(strWebData);

            foreach (Doc doc in docList)
            {
                sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}",
                    doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                    doc.Path, doc.RedirectUrl, doc.HashedPath, //doc.Summary, 
                    doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                    doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                    doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore);
            }

            //sw.WriteLine(strWebData);
            sw.Flush();
            sw.Close();

            return docList;
        }

        static public void getTermSearchResults_Personal()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_TermSearch);

            string query = "";
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Query_Personal);
                query = sr.ReadLine();
                while (null != query)
                {
                    string url = Constants.HttpRequest_TermSearch_Prefix + query.Replace(" ", "+") + Constants.HttpRequest_TermSearch_Suffix;
                    string strWebData = HtmlUtilities.downloadHtml(url, Constants.LocalPath_HttpResponse_TermSearch);

                    List<Doc> docList = Utilities.JsonSerializer(strWebData);

                    foreach (Doc doc in docList)
                    {
                        string snippet = doc.Summary;
                        if (snippet != null)
                        {
                            snippet = snippet.Replace("\n", "");
                            snippet = snippet.Replace("\r", "");
                        }
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}\t{28}\t{29}",
                                    query, doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                                    doc.Path, doc.RedirectUrl, doc.HashedPath,
                                    doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                                    doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                                    doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore, snippet);
                    }
                    query = sr.ReadLine();
                }

                sw.Flush();
                sw.Close();
                sr.Close();
            }
            catch (IOException e)
            {
            }
        }

        static public void getTermSearchResults_Test()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_TermSearch_Test);

            string query = "";
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Query_Test);
                query = sr.ReadLine();
                while (null != query)
                {
                    string url = Constants.HttpRequest_TermSearch_Prefix + query.Replace(" ", "+") + Constants.HttpRequest_TermSearch_Suffix;
                    string strWebData = HtmlUtilities.downloadHtml(url, Constants.LocalPath_HttpResponse_TermSearch);

                    List<Doc> docList = Utilities.JsonSerializer(strWebData);

                    foreach (Doc doc in docList)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}\t{28}",
                                    query, doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                                    doc.Path, doc.RedirectUrl, doc.HashedPath, //doc.Summary, 
                                    doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                                    doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                                    doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore);
                    }
                    query = sr.ReadLine();
                }

                sw.Flush();
                sw.Close();
                sr.Close();
            }
            catch (IOException e)
            {
            }
        }

        static public void getMSWSearchResults_Personal()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_MSW_Personal);

            string query = "";
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Query_Personal);
                query = sr.ReadLine();
                while (null != query)
                {
                    string strWebData = HtmlUtilities.downloadHtmlWithRequestHeaderAndBody(query, Constants.LocalPath_HttpResponse_MSW);
                    strWebData = Utilities.NormalizeJson(strWebData);
                    query = Utilities.GetQuery(strWebData);

                    List<MSWDoc> docList = Utilities.JsonSerializerMSWDoc(strWebData);

                    foreach (MSWDoc doc in docList)
                    {
                        string snippet = doc.source.HitHighlightedSummary;
                        if (snippet != null)
                        {
                            snippet = snippet.Replace("\n", "");
                            snippet = snippet.Replace("\r", "");
                        }
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                            query, doc.Id, doc.ContentSource, doc.source.FileType, doc.source.Author,
                            doc.source.Created, doc.source.LastModifiedTime, doc.source.Path,
                            doc.source.Title, snippet);
                    }
                    query = sr.ReadLine();
                }

                sw.Flush();
                sw.Close();
                sr.Close();
            }
            catch (IOException e)
            {
            }
        }

        static public void getMSWSearchResults_Test()
        {
            StreamWriter sw = new StreamWriter(Constants.LocalPath_ParsedFields_MSW_Test);

            string query = "";
            try
            {
                StreamReader sr = new StreamReader(Constants.LocalPath_Query_Test);
                query = sr.ReadLine();
                while (null != query)
                {
                    string strWebData = HtmlUtilities.downloadHtmlWithRequestHeaderAndBody(query, Constants.LocalPath_HttpResponse_MSW);
                    strWebData = Utilities.NormalizeJson(strWebData);
                    //sw.WriteLine(strWebData);
                    query = Utilities.GetQuery(strWebData);

                    List<MSWDoc> docList = Utilities.JsonSerializerMSWDoc(strWebData);

                    foreach (MSWDoc doc in docList)
                    {
                        string snippet = doc.source.HitHighlightedSummary;
                        if (snippet != null)
                        {
                            snippet = snippet.Replace("\n", "");
                            snippet = snippet.Replace("\r", "");
                        }
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}",
                            query, doc.Id, doc.ContentSource, doc.source.FileType, doc.source.Author,
                            doc.source.Created, doc.source.LastModifiedTime, doc.source.Path,
                            doc.source.Title, snippet);
                    }
                    query = sr.ReadLine();
                }

                sw.Flush();
                sw.Close();
                sr.Close();
            }
            catch (IOException e)
            {
            }
        }


        static public void testOLSResponseJsonParser()
        {
            StreamReader sr = new StreamReader(@"D:\Test\recallAnalysis\OLSResponse.txt");
            StreamWriter sw = new StreamWriter(@"D:\Test\recallAnalysis\OLSParsedResponse.txt");

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line != "" && line != "\n")
                {
                    List<Doc> docList = Utilities.JsonSerializer(line);

                    foreach (Doc doc in docList)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}",
                            doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                            doc.Path, doc.RedirectUrl, doc.HashedPath, //doc.Summary, 
                            doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                            doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                            doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore);
                    }
                    sw.WriteLine();
                }
            }
            sr.Close();
            sw.Flush();
            sw.Close();
        }

        static public Dictionary<string, int> fieldCountAnalysis(List<Doc> docList)
        {
            Dictionary<string, int> count = new Dictionary<string, int>();
            foreach (Doc doc in docList)
            {
                Utilities.setDictionaryValue(count, "Author", Utilities.isNullOrEmptyOrZero(doc.Author));
                Utilities.setDictionaryValue(count, "FileType", Utilities.isNullOrEmptyOrZero(doc.FileType));
                Utilities.setDictionaryValue(count, "Id", Utilities.isNullOrEmptyOrZero(doc.Id));
                Utilities.setDictionaryValue(count, "CreatedTime", Utilities.isNull(doc.CreatedTime));
                Utilities.setDictionaryValue(count, "LastModifiedTime", Utilities.isNull(doc.LastModifiedTime));
                Utilities.setDictionaryValue(count, "LastModifiedByMeTime", Utilities.isNull(doc.LastModifiedByMeTime));
                Utilities.setDictionaryValue(count, "LastViewedByMeTime", Utilities.isNull(doc.LastViewedByMeTime));
                Utilities.setDictionaryValue(count, "Path", Utilities.isNullOrEmptyOrZero(doc.Path));
                Utilities.setDictionaryValue(count, "RedirectUrl", Utilities.isNullOrEmptyOrZero(doc.RedirectUrl));
                Utilities.setDictionaryValue(count, "HashedPath", Utilities.isNullOrEmptyOrZero(doc.HashedPath));
                Utilities.setDictionaryValue(count, "Summary", Utilities.isNullOrEmptyOrZero(doc.Summary));
                Utilities.setDictionaryValue(count, "Title", Utilities.isNullOrEmptyOrZero(doc.Title));

                Utilities.setDictionaryValue(count, "Views", Utilities.isZero(doc.Views));
                Utilities.setDictionaryValue(count, "Size", Utilities.isZero(doc.Size));
                Utilities.setDictionaryValue(count, "ViewLast7days", Utilities.isZero(doc.ViewLast7days));
                Utilities.setDictionaryValue(count, "PiSearchResultId", Utilities.isNullOrEmptyOrZero(doc.PiSearchResultId));

                Utilities.setDictionaryValue(count, "GraphRankingScore", Utilities.isZero(doc.GraphRankingScore));
                Utilities.setDictionaryValue(count, "SearchRankingScore", Utilities.isZero(doc.SearchRankingScore));
                Utilities.setDictionaryValue(count, "MergedRankingScore", Utilities.isZero(doc.MergedRankingScore));
                Utilities.setDictionaryValue(count, "ReUseScore", Utilities.isZero(doc.ReUseScore));
                Utilities.setDictionaryValue(count, "TrendingAroundMeScore", Utilities.isZero(doc.TrendingAroundMeScore));
                Utilities.setDictionaryValue(count, "ModifiedByMeScore", Utilities.isZero(doc.ModifiedByMeScore));
                Utilities.setDictionaryValue(count, "ViewedByMeScore", Utilities.isZero(doc.ViewedByMeScore));

                Utilities.setDictionaryValue(count, "IsExternalContent", Utilities.isFalse(doc.IsExternalContent));

                Utilities.setDictionaryValue(count, "ObjectEmbeddings", Utilities.isNullOrEmptyOrZero(doc.ObjectEmbeddings));
                Utilities.setDictionaryValue(count, "SPWebUrl", Utilities.isNullOrEmptyOrZero(doc.SPWebUrl));
                Utilities.setDictionaryValue(count, "RankInSearchResult", Utilities.isZero(doc.RankInSearchResult));
                Utilities.setDictionaryValue(count, "StatusKey", Utilities.isNullOrEmptyOrZero(doc.StatusKey));
                Utilities.setDictionaryValue(count, "FastRankRankingScore", Utilities.isZero(doc.FastRankRankingScore));
            }

            return count;
        }

        static public List<string> docOrdersAnalysis(List<Doc> docList)
        {
            List<string> orders = new List<string>();
            foreach (Doc doc in docList)
            {
                orders.Add(doc.Title + "\t" + doc.Author);
            }
            return orders;
        }

        static public void testThreeSResponseJsonParser()
        {
            StreamReader sr = new StreamReader(@"D:\Test\recallAnalysis\ThreeSResponse.txt");
            StreamWriter sw = new StreamWriter(@"D:\Test\recallAnalysis\ThreeSParsedResponse.txt");

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line != "" && line != "\n")
                {
                    List<Doc> docList = Utilities.JsonSerializer(line);

                    /*
                    foreach (Doc doc in docList)
                    {
                        sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}\t{27}",
                            doc.Author, doc.FileType, doc.Id, doc.CreatedTime, doc.LastModifiedTime, doc.LastModifiedByMeTime, doc.LastViewedByMeTime,
                            doc.Path, doc.RedirectUrl, doc.HashedPath, //doc.Summary, 
                            doc.Title, doc.Views, doc.Size, doc.ViewLast7days, doc.PiSearchResultId, doc.GraphRankingScore, doc.SearchRankingScore, doc.MergedRankingScore,
                            doc.ReUseScore, doc.TrendingAroundMeScore, doc.ModifiedByMeScore, doc.ViewedByMeScore, doc.IsExternalContent, doc.ObjectEmbeddings,
                            doc.SPWebUrl, doc.RankInSearchResult, doc.StatusKey, doc.FastRankRankingScore);
                    }
                    */

                    /*
                    Dictionary<string, int> count = fieldCountAnalysis(docList);
                    foreach (KeyValuePair<string, int> kvp in count)
                    {
                        sw.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    }
                    */

                    List<string> docOrders = docOrdersAnalysis(docList);
                    foreach (string order in docOrders)
                    {
                        sw.WriteLine(order);
                    }
                    sw.WriteLine();
                }
            }
            sr.Close();
            sw.Flush();
            sw.Close();
        }

        static void compareCount()
        {
            StreamWriter sw = new StreamWriter(@"D:\Test\recallAnalysis\EmptyFieldAnalysis.txt");

            List<Doc> OLSDocList = getOLSSearchResults();
            List<Doc> ThreeSDocList = get3SZeroTermSearchResults();
            Dictionary<string, int> OLSCount = fieldCountAnalysis(OLSDocList);
            Dictionary<string, int> ThreeSCount = fieldCountAnalysis(ThreeSDocList);

            foreach (string key in OLSCount.Keys)
            {
                sw.WriteLine(key + "\t" + OLSCount[key] + "\t" + ThreeSCount[key]);
            }

            List<string> OLSDocOrders = docOrdersAnalysis(OLSDocList);
            List<string> ThreeSDocOrders = docOrdersAnalysis(ThreeSDocList);


            int i = 0;
            for (i = 0; i < OLSDocOrders.Count; i++)
            {
                if (OLSDocOrders[i] != ThreeSDocOrders[i])
                {
                    sw.WriteLine("{0}\t{1}\t{2}", i, OLSDocOrders[i], ThreeSDocOrders[i]);
                }
                i++;
            }

            sw.WriteLine();

            int inOLSIn3SCount = 0;
            for (int j = 0; j < OLSDocOrders.Count; j++)
            {
                bool isFound = false;
                for (int k = 0; k < ThreeSDocOrders.Count; k++)
                {
                    if (OLSDocOrders[j] == ThreeSDocOrders[k])
                    {
                        inOLSIn3SCount++;
                        isFound = true;
                        break;
                    }
                }
                if (isFound == false)
                {
                    sw.WriteLine(OLSDocOrders[j]);
                }
            }
            sw.WriteLine("inOLSIn3SCount:{0}", inOLSIn3SCount);
            sw.WriteLine();


            int in3SInOLSCount = 0;
            for (int j = 0; j < ThreeSDocOrders.Count; j++)
            {
                bool isFound = false;
                for (int k = 0; k < OLSDocOrders.Count; k++)
                {
                    if (ThreeSDocOrders[j] == OLSDocOrders[k])
                    {
                        in3SInOLSCount++;
                        isFound = true;
                        break;
                    }
                }
                if (isFound == false)
                {
                    sw.WriteLine(ThreeSDocOrders[j]);
                }
            }
            sw.WriteLine("in3SInOLSCount:{0}", in3SInOLSCount);

            sw.Flush();
            sw.Close();
        }
        static void Main(string[] args)
        {
            //getMSWSearchResults_Test();
            //getMSWSearchResults_Personal();
            //getTermSearchResults_Personal();
            //getTermSearchResults_Test();
            //CreateHtmlPage.CreateHtmlForMSW();
            //CreateHtmlPage.CreateHtmlForTap();
            CreateHtmlPage.CreateSBS();
        }
    }
}
