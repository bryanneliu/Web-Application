using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SmartSearch.Html
{
    // Link Source: Where is a link from?
    public enum LinkSourceType
    {
        Anchor,         // <A HREF=...>
        Area,           // <AREA HREF=... ALT=...>
        Frame,          // <FRAME SRC=...>
        IFrame,         // <IFRAME SRC=...>
        Image,          // <IMG SRC=...>
        Link,           // <LINK HREF=...>
        Form            // forms where the possible outputs can be deterministically 
        // predicted, e.g.  SELECT/CHECKBOX/RADIO/HIDDEN
    };

    public class LinkItem
    {
        public LinkSourceType LinkSource;
        public string Url;
        public string NodeStack;
        public string AnchorText;
        public string LeftText;
        public string RightText;

        public LinkItem()
        {
            LinkSource = LinkSourceType.Anchor;
            Url = null;
            //NodeStack = "";
            AnchorText = null;
            LeftText = null;
            RightText = null;
        }

        public string NearbyText
        {
            get { return LeftText; }
        }

        //------------------------------------------------
        private static string[] sourceTypeName = new string[]
        {
            "Anchor",
            "Area",
            "Frame",
            "IFrame",
            "Image",
            "Link",
            "Form"
        };

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(256);
            builder.Append("LinkSource:");
            builder.AppendLine(sourceTypeName[(int)LinkSource]);
            builder.Append("Url:");
            builder.AppendLine(Url);
            builder.Append("AnchorText:");
            builder.AppendLine(AnchorText);
            //builder.Append("NearbyText:");
            //builder.AppendLine(NearbyText);
            return builder.ToString();
        }
    }

    public class ParsedHtml
    {
        private string html;
        private List<HtmlNode> nodes;
        private List<LinkItem> links;

        private string title;
        private string text;
        private string baseUrl;
        private bool allowFollow;

        private string metaTitle;
        private string metaKeywords;
        private string metaDescription;

        public string Html
        {
            set { html = value; }
            get { return html; }
        }

        public string Title
        {
            get { return title; }
        }

        public string Text
        {
            get { return text; }
        }

        public string MetaTitle
        {
            get { return metaTitle; }
        }

        public string MetaKeywords
        {
            get { return MetaKeywords; }
        }

        public string MetaDescription
        {
            get { return metaDescription; }
        }

        internal List<HtmlNode> Nodes
        {
            set { nodes = value; }
            get { return nodes; }
        }

        public List<LinkItem> Links
        {
            set { links = value; }
            get { return links; }
        }

        public bool AllowFollow
        {
            get { return allowFollow; }
        }

        public ParsedHtml()
        {
            html = null;
            nodes = null;
            links = null;
            title = null;
            text = null;
            baseUrl = null;
            allowFollow = true;
            metaTitle = null;
            metaKeywords = null;
            metaDescription = null;
        }

        internal string GetHtml(HtmlNode node)
        {
            Debug.Assert(node.Index >= 0 && node.Index + node.Length <= html.Length);
            return html.Substring(node.Index, node.Length);
        }

        public string TextBetween(int startTag, int endTag)
        {
            Debug.Assert(startTag >= 0 && startTag < nodes.Count);
            Debug.Assert(endTag >= 0 && endTag < nodes.Count);

            bool hasText = false;
            StringBuilder textBuilder = new StringBuilder(100);
            for (int i = startTag + 1; i < endTag; i++)
            {
                if ((i >= 0 && i < nodes.Count) && nodes[i].IsTextNode())
                {
                    if (hasText)
                    {
                        textBuilder.Append('\t');
                    }
                    string innerHtml = GetHtml(nodes[i]);
                    textBuilder.Append(innerHtml);
                    hasText = true;
                }
            }

            return textBuilder.ToString();
        }



        internal static int FindEndTag(List<HtmlNode> nodes, int from, HtmlTagId tagId)
        {
            Debug.Assert(nodes != null);

            for (int i = from; i < nodes.Count; i++)
            {
                if (nodes[i].IsEndTag(tagId))
                {
                    return i;
                }
            }

            return -1;
        }

        internal int FindEndTag(int from, HtmlTagId tagId)
        {
            return FindEndTag(this.nodes, from, tagId);
        }

        private void ProcessMeta(HtmlNode node)
        {
            string name = node.GetAttributeValue(HtmlAttributeId.Name);
            string content = node.GetAttributeValue(HtmlAttributeId.Content);
            if (content == null)
                return;

            if (name == "robots")
            {
                if (content.ToLower().Contains("nofollow"))
                {
                    this.allowFollow = false;
                }
            }
            else if (name == "title")
            {
                this.metaTitle = content;
            }
            else if (name == "keywords")
            {
                this.metaKeywords = content;
            }
            else if (name == "description")
            {
                this.metaDescription = content;
            }
        }

        public void Process()
        {
            this.title = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode node = nodes[i];
                if (node.IsStartTag(HtmlTagId.Title))
                {
                    int endTag = FindEndTag(i + 1, HtmlTagId.Title);
                    if (endTag == i + 2)
                    {
                        this.title = TextBetween(i, endTag);
                        return;
                    }
                }
                else if (node.IsStartTag(HtmlTagId.Base))
                {
                    this.baseUrl = node.GetAttributeValue(HtmlAttributeId.Href);
                }
                else if (node.IsStartTag(HtmlTagId.Meta))
                {
                    ProcessMeta(node);
                }
            }
        }


    };
}
