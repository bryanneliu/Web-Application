using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace SmartSearch.Html
{
    class DownloadHtml
    {
        public static string DownloadHtmlstring(string url)
        {
            System.Net.WebClient wc = new WebClient();

            //string html = wc.DownloadString(url);

            byte[] pageData = wc.DownloadData(url);
            string strWebData = System.Text.Encoding.Default.GetString(pageData);

            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=(\")?([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[3].Value;
            Console.WriteLine(webCharSet);
            if (webCharSet != null && webCharSet != "" && Encoding.GetEncoding(webCharSet) != Encoding.Default)
            {
                strWebData = Encoding.GetEncoding(webCharSet).GetString(pageData);
            }
            return strWebData;
        }
    }
}
