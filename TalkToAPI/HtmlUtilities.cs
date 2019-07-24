using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace TalkToAPI
{
    static public class HtmlUtilities
    {
        static public string downloadHtml(string url, string localPath)
        {
            StreamWriter sw = new StreamWriter(localPath);

            System.Net.WebClient wc = new WebClient();
            byte[] pageData = wc.DownloadData(url);
            string strWebData = System.Text.Encoding.Default.GetString(pageData);
            sw.WriteLine(strWebData);
            sw.Flush();
            sw.Close();
            return strWebData;
        }

        static public string downloadHtmlWithHeader(string url, string flight, string localPath)
        {
            StreamWriter sw = new StreamWriter(localPath);

            System.Net.WebClient wc = new WebClient();
            //wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wc.Headers.Add("Flights", flight);
            byte[] pageData = wc.DownloadData(url);
            string strWebData = System.Text.Encoding.Default.GetString(pageData);
            sw.WriteLine(strWebData);
            sw.Flush();
            sw.Close();
            return strWebData;
        }

        static public string downloadHtmlWithRequestHeaderAndBody(string query, string localPath)
        {
            StreamWriter sw = new StreamWriter(localPath);

            var client = new RestClient("https://substrate.office.com/search/api/v2/query?debug=1");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Host", "substrate.office.com");
            request.AddHeader("Postman-Token", "bdac1104-5504-461d-82d2-62b7f2dd3869,795e6368-a675-4586-85ab-7ffa819ab2f2");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Authorization", "Bearer " + Constants.BearerToken_MSW);
            request.AddHeader("client-session-id", "{{client-session-id}}");
            request.AddHeader("client-request-id", "3349fc94-4c54-4665-831c-fd7f44db2dfa");
            request.AddHeader("User-Agent", "ExecuteSearchTool");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "  {\"EntityRequests\": [\r\n    {\r\n      \"EntityType\": \"File\",\r\n      \"ContentSources\": [ \"OneDriveBusiness\", \"SharePoint\" ],\r\n      \"Query\": { \r\n      \t\"QueryString\": \"" + query + " filetype:(pptx OR ppt OR docx OR doc)\" , \r\n      \t\"ResultSourceQueryTemplate\": \"{searchTerms} -ContentClass=urn:content-class:SPSPeople\" }\r\n      ,\r\n      \"From\": 0,\r\n      \"Size\": 25,\r\n     \"ResultsMerge\":{\"Type\":\"Interleaved\"},\r\n      \"Sort\": [\r\n          {\r\n          \"Field\":\"PersonalScore\", // Time, Score\r\n          \"SortDirection\":\"Desc\" // Desc, None\r\n          }\r\n    \t]\r\n    }\r\n  ],\r\n  \"TextDecorations\": \"Off\",\r\n  \"TimeZone\": \"UTC\",\r\n  \"Cvid\": \"d743329e-2fd1-7006-c938-8dad2433117d\",\r\n  \"Scenario\": { \"Name\": \"sphomeweb\" }\r\n}", ParameterType.RequestBody);

            int leng = 702 + query.Length;
            request.AddHeader("content-length", leng.ToString());

            IRestResponse response = client.Execute(request);

            sw.WriteLine(response.Content);
            sw.Flush();
            sw.Close();
            return response.Content;
        }
    }
}
