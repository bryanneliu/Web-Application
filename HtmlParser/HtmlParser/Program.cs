using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace SmartSearch.Html
{
    class Program
    {
        static MarkupParser markupParser = new MarkupParser();
        static public int num = 1;
        public static string ToString(string html, HtmlNode node)
        {
            StringBuilder builder = new StringBuilder(128);

            if (node.NodeType == HtmlNodeType.Element)
            {
                string tagName = HtmlTags.GetTagName(node.TagId);
                builder.Append("<");
                builder.Append(tagName);
                if (node.Attributes != null)
                {
                    foreach (AttributeItem item in node.Attributes)
                    {
                        if (item.Value.Contains("\""))
                        {
                            builder.AppendFormat(" {0}='{1}'", item.Name, item.Value);
                        }
                        else
                        {
                            builder.AppendFormat(" {0}=\"{1}\"", item.Name, item.Value);
                        }
                    }
                }
                builder.Append(">");
            }
            else if (node.NodeType == HtmlNodeType.EndTag)
            {
                string tagName = HtmlTags.GetTagName(node.TagId);
                builder.AppendFormat("</{0}>", tagName);
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                string text = HtmlUtility.NormalizeText(html, node.Index, node.Length);
                if (text != null)
                    builder.Append(text);
            }
            else if (node.NodeType == HtmlNodeType.Comment)
            {
                string text = html.Substring(node.Index, node.Length);
                builder.Append(text);
            }
            else
            {
                Debug.Assert(false);
            }

            return builder.ToString();
        }

        public static void Dump(string path, string html, List<HtmlNode> nodes)
        {
            using (StreamWriter streamWriter = new StreamWriter(path + "l", false, Encoding.UTF8))
            {
                foreach (HtmlNode node in nodes)
                {
                    string text = ToString(html, node);
                    streamWriter.WriteLine(text);
                }
            }
        }

        private static void Dump(ParsedHtml parsedHtml)
        {
            //Console.WriteLine("PageTitle:" + parsedHtml.Title);
            StreamWriter sw = new StreamWriter(@"D:\HtmlParser\LOG\bing" + num.ToString() + ".txt");
            num++;

            int link = 0;
            foreach (LinkItem item in parsedHtml.Links)
            {
                link++;
                //Console.WriteLine("Line:{0}\n{1}\n", link, item.ToString());
                sw.WriteLine("Line:{0}\n{1}\n", link, item.ToString());
            }
        }

        public static void Process(string directory)
        {
            DirectoryInfo folder = new DirectoryInfo(directory);

            FileInfo[] files = folder.GetFiles("*.htm");
            foreach (FileInfo file in files)
            {
                string path = file.FullName;
                using (StreamReader streamReader = File.OpenText(path))
                {
                    string html = streamReader.ReadToEnd();
                    List<HtmlNode> nodes = markupParser.Parse(html);
                    Dump(path, html, nodes);

                    LinkExtractor linkExtractor = new LinkExtractor();

                    ParsedHtml parsedHtml = new ParsedHtml();
                    parsedHtml.Html = html;
                    parsedHtml.Nodes = nodes;
                    linkExtractor.Process(parsedHtml, true);
                    Dump(parsedHtml);
                }
            }
        }

        /// <summary>  
        /// Levenshtein Distance）  
        /// </summary>  
        /// <param name="source"></param>  
        /// <param name="target"></param>  
        /// <param name="similarity">0~1</param>  
        /// <param name="isCaseSensitive"></param>  
        /// <returns></returns>  
        public static Int32 LevenshteinDistance(String source, String target, out Double similarity, Boolean isCaseSensitive = false)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target))
                {
                    similarity = 1;
                    return 0;
                }
                else
                {
                    similarity = 0;
                    return target.Length;
                }
            }
            else if (String.IsNullOrEmpty(target))
            {
                similarity = 0;
                return source.Length;
            }

            String From, To;
            if (isCaseSensitive)
            {     
                From = source;
                To = target;
            }
            else
            {     
                From = source.ToLower();
                To = target.ToLower();
            }

            //init  
            Int32 m = From.Length;
            Int32 n = To.Length;
            Int32[,] H = new Int32[m + 1, n + 1];
            for (Int32 i = 0; i <= m; i++) H[i, 0] = i;  
            for (Int32 j = 1; j <= n; j++) H[0, j] = j;

            //rec  
            for (Int32 i = 1; i <= m; i++)
            {
                Char SI = From[i - 1];
                for (Int32 j = 1; j <= n; j++)
                {   
                    if (SI == To[j - 1])
                        H[i, j] = H[i - 1, j - 1];
                    else
                        H[i, j] = Math.Min(H[i - 1, j - 1], Math.Min(H[i - 1, j], H[i, j - 1])) + 1;
                }
            }

            Int32 MaxLength = Math.Max(m, n);   
            similarity = ((Double)(MaxLength - H[m, n])) / MaxLength;

            return H[m, n];
        }


        static void Main(string[] args)
        {
            UserInterface.testNodes();
        }
    }
}
