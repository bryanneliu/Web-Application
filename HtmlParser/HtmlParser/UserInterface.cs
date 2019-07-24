using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmartSearch.Html
{
    class UserInterface
    {
        static MarkupParser markupParser = new MarkupParser();
        public static Dictionary<string,string> L1Categories = new Dictionary<string,string>();

        public static void RootLeftpath(string html, List<HtmlNode> nodes)
        {
            string L1_path = @"D:\265.com\L1\LeftL1Categories.txt";
            StreamWriter sw = new StreamWriter(L1_path);

            int start = 0;
            string url = "", category = "";

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeType == HtmlNodeType.Element)
                {
                    string tagName = HtmlTags.GetTagName(nodes[i].TagId);

                    if (tagName.ToLower() == "li" && nodes[i].Attributes != null && nodes[i].Attributes.Count > 0)
                    {
                        for (int k = 0; k < nodes[i].Attributes.Count; k++)
                        {
                            if (nodes[i].Attributes[k].Name == "class" && nodes[i].Attributes[k].Value == "siteCateLeftItems")
                            {
                                start = 1;
                                continue;
                            }
                        }
                    }
                    if(start == 1 && tagName.ToLower() == "a"  && nodes[i].Attributes != null && nodes[i].Attributes.Count > 0)
                    {
                        string site = @"http://www.265.com";
                        for(int k=0; k<nodes[i].Attributes.Count; k++)
                        {
                            if(nodes[i].Attributes[k].Name == "href")
                            {
                                url = site + nodes[i].Attributes[k].Value;
                            }
                        }
                    }
     
                }
                else if (nodes[i].NodeType == HtmlNodeType.Text)
                {
                    if (start == 1)
                    {
                        category = html.Substring(nodes[i].Index, nodes[i].Length);
                        //Console.WriteLine(category);
                    }
                }
                else if (nodes[i].NodeType == HtmlNodeType.EndTag)
                {
                    string tagName = HtmlTags.GetTagName(nodes[i].TagId);
                    if (tagName.ToLower() == "li" && start > 0)
                    {
                        start = 0;
                        sw.WriteLine("{0}\t{1}", category, url);
                        sw.Flush();
                    }
                }
                else{
                    continue;
                }
            }
            sw.Close();
        }

        public static void RootRightpath(string html, List<HtmlNode> nodes)
        {
            string L1_path = @"D:\265.com\L1\RightL1Categories.txt";
            StreamWriter sw = new StreamWriter(L1_path);

            int start = 0;
            string url = "", category = "";

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeType == HtmlNodeType.Element)
                {
                    string tagName = HtmlTags.GetTagName(nodes[i].TagId);

                    if (tagName.ToLower() == "li" && nodes[i].Attributes != null && nodes[i].Attributes.Count > 0)
                    {
                        for (int k = 0; k < nodes[i].Attributes.Count; k++)
                        {
                            if (nodes[i].Attributes[k].Name == "class" && nodes[i].Attributes[k].Value == "title")
                            {
                                start = 1;
                                continue;
                            }
                        }
                    }
                    if (start == 1 && tagName.ToLower() == "a" && nodes[i].Attributes != null && nodes[i].Attributes.Count > 0)
                    {
                        string site = @"http://www.265.com";
                        for (int k = 0; k < nodes[i].Attributes.Count; k++)
                        {
                            if (nodes[i].Attributes[k].Name == "href")
                            {
                                url = site + nodes[i].Attributes[k].Value;
                            }
                        }
                    }

                }
                else if (nodes[i].NodeType == HtmlNodeType.Text)
                {
                    if (start == 1)
                    {
                        category = html.Substring(nodes[i].Index, nodes[i].Length);
                        //Console.WriteLine(category);
                    }
                }
                else if (nodes[i].NodeType == HtmlNodeType.EndTag)
                {
                    string tagName = HtmlTags.GetTagName(nodes[i].TagId);
                    if (tagName.ToLower() == "li" && start > 0)
                    {
                        start = 0;
                        sw.WriteLine("{0}\t{1}", category, url);
                        sw.Flush();
                    }
                }
                else
                {
                    continue;
                }
            }
            sw.Close();
        }

        public static void Subpath(string html, List<HtmlNode> nodes, string filepath)
        {
           
            StreamWriter sw = new StreamWriter(filepath);

            int start = 0;
            string category = "", url = "";

            for (int i = 0; i < nodes.Count; i++)
            {
               if (nodes[i].NodeType == HtmlNodeType.Element)
                {
                    string tagName = HtmlTags.GetTagName(nodes[i].TagId);
                    if (tagName.ToLower() == "div" && nodes[i].Attributes != null && nodes[i].Attributes.Count > 0)
                    {
                        for (int k = 0; k < nodes[i].Attributes.Count; k++)
                        {
                            if (nodes[i].Attributes[k].Name == "class" && nodes[i].Attributes[k].Value == "titleCS")
                            {
                                start = 1;
                            }
                        }
                    }

                    if (tagName.ToLower() == "ul" && nodes[i].Attributes != null && nodes[i].Attributes.Count > 0)
                    {
                        for (int k = 0; k < nodes[i].Attributes.Count; k++)
                        {
                            if (nodes[i].Attributes[k].Name == "class" && nodes[i].Attributes[k].Value == "listUrl")
                            {
                                start = 2;
                            }
                        }
                    }

                    if (tagName.ToLower() == "a" && nodes[i].Attributes != null && nodes[i].Attributes.Count > 0 && start == 2)
                    {
                        for (int k = 0; k < nodes[i].Attributes.Count; k++)
                        {
                            if (nodes[i].Attributes[k].Name == "href")
                            {
                                url = nodes[i].Attributes[k].Value;
                            }

                        }
                        
                    }
                }else if (nodes[i].NodeType == HtmlNodeType.Text){
                    if (start == 1)
                    {
                        category = html.Substring(nodes[i].Index, nodes[i].Length).ToString();
                        start = 2;
                    }
                }
                else if (nodes[i].NodeType == HtmlNodeType.EndTag)
                {
                    string tagName = HtmlTags.GetTagName(nodes[i].TagId);
                    if (tagName.ToLower() == "li" && start == 2)
                    {
                        sw.WriteLine("{0}\t{1}", category, url);
                        sw.Flush();
                    }

                    if (tagName.ToLower() == "ul" && start == 2)
                    {
                        start = 0;
                    }
                }
            }
            sw.Close();
        }

        public static void L2Categories()
        {
            string L1CategoriesPath = @"D:\265.com\L1\1.txt";
            StreamReader sr = new StreamReader(L1CategoriesPath);

            string L2CategoriesDir = @"D:\265.com\L2\";

            string category = "", url = "" , tmp = "";
            int count = 0;
            while ((tmp = sr.ReadLine())!= null)
            {
                if (tmp != "" && tmp != "\n")
                {
                    Console.WriteLine(++count);
                    string[] line = tmp.Split('\t');
                    category = line[0];
                    url = line[1];

                    string L2CategoryPath = L2CategoriesDir + category + ".txt";
                    string html = DownloadHtml.DownloadHtmlstring(url);
                    List<HtmlNode> nodes = markupParser.Parse(html);

                    Subpath(html, nodes, L2CategoryPath);
                }
            }
            sr.Close();
        }

        public static void testNodes()
        {
            string navigation_site_url = @"http://www.265.com/";

            string html = DownloadHtml.DownloadHtmlstring(navigation_site_url);

            List<HtmlNode> nodes = markupParser.Parse(html);

            //RootLeftpath(html, nodes);

            //RootRightpath(html, nodes);

            L2Categories();
        }
    }
}
