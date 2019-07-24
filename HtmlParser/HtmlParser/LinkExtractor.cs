using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SmartSearch.Html
{
    class LinkExtractor
    {
        private ParsedHtml parsedHtml;
        private bool extractSurroundingText = false;

        public void Process(ParsedHtml parsedHtml, bool extractSurroundingText)
        {
            Debug.Assert(parsedHtml != null);
            Debug.Assert(parsedHtml.Html != null);
            Debug.Assert(parsedHtml.Nodes != null);

            this.extractSurroundingText = extractSurroundingText;
            this.parsedHtml = parsedHtml;
            parsedHtml.Links = new List<LinkItem>();

            int linkCount = 0;
            List<int> linkPosition = new List<int>();
            for (int i = 0; i < parsedHtml.Nodes.Count; i++)
            {
                ProcessNode(i);

                if (parsedHtml.Links.Count > linkCount)
                {
                    linkPosition.Add(i);
                    linkCount++;
                }
            }

            //TODO: move the following out of HtmlParser since this is not generic
            // Extract Image Anchor like <a href=...><img src=... alt=...></a>
            Debug.Assert(parsedHtml.Links.Count == linkCount);
            for (int i = 0; i < linkCount - 1; i++)
            {
                if (linkPosition[i] + 1 == linkPosition[i + 1] &&
                    parsedHtml.Links[i].LinkSource == LinkSourceType.Anchor &&
                    string.IsNullOrEmpty(parsedHtml.Links[i].AnchorText) &&
                    parsedHtml.Links[i + 1].LinkSource == LinkSourceType.Image)
                    {
                        parsedHtml.Links[i].AnchorText = "<Img_Anchor>";
                        if (!string.IsNullOrEmpty(parsedHtml.Links[i + 1].AnchorText))
                        {
                            parsedHtml.Links[i].AnchorText += parsedHtml.Links[i + 1].AnchorText;
                        }
                    }
            }
        }

        /*
        public List<LinkItem> Extract(string html, List<HtmlNode> nodes)
        {
            Debug.Assert(html != null);
            Debug.Assert(nodes != null);

            parsedHtml = new ParsedHtml();
            parsedHtml.Html = html;
            parsedHtml.Nodes = nodes;
            parsedHtml.Links = new List<LinkItem>();

            for (int i = 0; i < nodes.Count; i++)
            {
                ProcessNode(i);
            }

            return parsedHtml.Links;
        }
        */

        private HtmlNode GetNode(int i)
        {
            return parsedHtml.Nodes[i];
        }

        public static string NormalizeUrl(string url)
        {
            if (url == null || url.IndexOfAny("<>'\"".ToCharArray()) >= 0)
                return null;

            url = url.Trim();
            if (url.Length == 0)
                return null;

            int i;
            if ((i = url.IndexOf('#')) >= 0)
            {
                url = url.Substring(0, i);
            }
            
            url = url.ToLower();
            if (url.IndexOf("mailto:") >= 0 
             || url.IndexOf("javascript:") >= 0 
             || url.IndexOf("document.") >= 0)
            {
                return null;
            }

            return url;
        }

        private void AddLink(LinkItem linkItem)
        {
            string url = LinkExtractor.NormalizeUrl(linkItem.Url);
            if (url != null)
            {
                linkItem.Url = url;
                parsedHtml.Links.Add(linkItem);
            }
        }

        private void ProcessNode(int i)
        {
            HtmlNode node = GetNode(i);

            if (node.IsStartTag(HtmlTagId.A))
            {
                ExtractAnchorLink(i);
            }
            else if (node.IsStartTag(HtmlTagId.Area))
            {
                ExtractAreaLink(i);
            }
            else if (node.IsStartTag(HtmlTagId.Img))
            {
                ExtractImageLink(i);
            }
            else if (node.IsStartTag(HtmlTagId.Frame))
            {
                ExtractFrameLink(i);
            }
            else if (node.IsStartTag(HtmlTagId.Iframe))
            {
                ExtractIFrameLink(i);
            }
            else if (node.IsStartTag(HtmlTagId.Link))
            {
                ExtractLinkLink(i);
            }
            else if (node.IsStartTag(HtmlTagId.Form))
            {
                //TODO:
                //ExtractFormLink(i);
            }
        }

        private string GetNodeStack(int startTag, int endTag)
        {
            string nodeStack = "";
            if (endTag < parsedHtml.Nodes.Count - 1)
            {
                endTag++;
            }
            if (startTag > 0)
            {
                startTag--;
            }

            while (startTag <= endTag)
            {
                HtmlNode node = GetNode(startTag);
                switch (node.NodeType)
                {
                    case HtmlNodeType.Element:
                        if (node.GetAttributeValue(HtmlAttributeId.Class) != null)
                        {
                            nodeStack += "<" + HtmlTags.GetTagName(node.TagId) + " class=" + node.GetAttributeValue(HtmlAttributeId.Class) + ">";
                        }
                        else
                        {
                            nodeStack += "<" + HtmlTags.GetTagName(node.TagId) + ">";
                        }
                        break;
                    case HtmlNodeType.EndTag:
                        nodeStack += "</" + HtmlTags.GetTagName(node.TagId) + ">";
                        break;
                    case HtmlNodeType.Text:
                        nodeStack += "_";
                        break;
                }
                startTag++;
            }
            return nodeStack;
        }

        const int MaxLeftOrRightNode = 10;

        private bool IsMeaningFullText(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetterOrDigit(text[i]))
                {
                    return true;
                }
            }

            return false;
        }


        // This is for anchor text mining for detect a text block (either meeting a line ending, or another link)
        // For handling <A> tag without </A>
        // TODO: refine this function
        public bool IsLinkSeparator(HtmlNode node)
        {
            // line ending nodes
            if (node.TagId == HtmlTagId.Td || node.TagId == HtmlTagId.Tr ||
                node.TagId == HtmlTagId.Div || node.TagId == HtmlTagId.Br ||
                node.TagId == HtmlTagId.H1 || node.TagId == HtmlTagId.H2 ||
                node.TagId == HtmlTagId.H3 || node.TagId == HtmlTagId.H4 ||
                node.TagId == HtmlTagId.H5 || node.TagId == HtmlTagId.H6 ||
                node.TagId == HtmlTagId.Table || node.TagId == HtmlTagId.Li ||
                node.TagId == HtmlTagId.Script || node.TagId == HtmlTagId.A)
            {
                return true;
            }

            return false;
        }

        // This is for surrounding text mining for detect a text block.
        // When meeting such tag, do not search surrounding text further
        // TODO: refine this function
        public bool IsSurroundingTextSeparator(HtmlNode node)
        {
            if (
                //node.TagId == HtmlTagId.Table || 
                node.TagId == HtmlTagId.Br ||
                node.TagId == HtmlTagId.Script ||
                node.TagId == HtmlTagId.Body)
            {
                return true;
            }

            return false;
        }


        private string GetLeftNearText(int startTag)
        {
            string nearText = "";
            if (!extractSurroundingText)
                return nearText;

            int count = 0;
            while (startTag > 0)
            {
                startTag--;

                HtmlNode node = GetNode(startTag);
                if (node.NodeType == HtmlNodeType.Text)
                {
                    string text = parsedHtml.GetHtml(node).Replace("&nbps;", " ").Trim();
                    if (text.Length > 0)
                    {
                        nearText += text;
                        if (IsMeaningFullText(nearText))
                            return nearText;
                    }
                }
                else
                {
                    if (IsSurroundingTextSeparator(node))
                        break;
                }

                count++;
                if (count > MaxLeftOrRightNode)
                    break;
            }

            return nearText;
        }

        private string GetRightNearText(int endTag)
        {
            string nearText = "";
            if (!extractSurroundingText)
                return nearText;

            int count = 0;
            while (endTag < parsedHtml.Nodes.Count - 1)
            {
                endTag++;

                HtmlNode node = GetNode(endTag);
                if (node.NodeType == HtmlNodeType.Text)
                {
                    string text = parsedHtml.GetHtml(node).Replace("&nbps;", " ").Trim();
                    if (text.Length > 0)
                    {
                        nearText += text;
                        if (IsMeaningFullText(nearText))
                            return nearText;
                    }
                }
                else
                {
                    if (IsSurroundingTextSeparator(node))
                        break;
                }

                count++;
                if (count > MaxLeftOrRightNode)
                    break;
            }

            return nearText;
        }

        private void ExtractAnchorLink(int startTag)
        {
            HtmlNode node = GetNode(startTag);
            string url = node.GetAttributeValue(HtmlAttributeId.Href);
            if (url != null && url.Length > 0)
            {
                LinkItem linkItem = new LinkItem();
                linkItem.LinkSource = LinkSourceType.Anchor;
                linkItem.Url = url;

                int endTag = parsedHtml.FindEndTag(startTag + 1, GetNode(startTag).TagId);

                // This is to avoid <A> ....<A> .... </A>
                for (int i = startTag + 1; i < endTag; i++)
                {
                    if (IsLinkSeparator(parsedHtml.Nodes[i]))
                    {
                        endTag = i;
                        break;
                    }
                }

                if (endTag > 0 && endTag < parsedHtml.Nodes.Count)
                {
                    linkItem.AnchorText = parsedHtml.TextBetween(startTag, endTag);
                    //linkItem.NodeStack = GetNodeStack(startTag, endTag);
                    linkItem.LeftText = GetLeftNearText(startTag);
                    linkItem.RightText = GetRightNearText(endTag);
                }
                AddLink(linkItem);
            }
        }

        private void ExtractImageLink(int startTag)
        {
            HtmlNode node = GetNode(startTag);
            string url = node.GetAttributeValue(HtmlAttributeId.Src);
            if (url != null && url.Length > 0)
            {
                string anchorText = "";
                
                string alt = node.GetAttributeValue(HtmlAttributeId.Alt);
                if (alt != null)
                {
                    anchorText = alt;
                }

                string title = node.GetAttributeValue(HtmlAttributeId.Title);
                if (title != null)
                {
                    if (string.IsNullOrEmpty(anchorText))
                        anchorText = title;
                    else
                        anchorText = anchorText + ";;" + title;
                }

                LinkItem linkItem = new LinkItem();
                linkItem.LinkSource = LinkSourceType.Image;
                linkItem.Url = url;
                linkItem.AnchorText = anchorText;
                AddLink(linkItem);
            }
        }

        private void ExtractAreaLink(int startTag)
        {
            HtmlNode node = GetNode(startTag);
            string url = node.GetAttributeValue(HtmlAttributeId.Href);
            if (url != null && url.Length > 0)
            {
                string anchorText = "";
                AttributeItem attribute = node.GetAttribute(HtmlAttributeId.Alt);
                if (attribute != null)
                {
                    anchorText = attribute.Value;
                }

                LinkItem linkItem = new LinkItem();
                linkItem.LinkSource = LinkSourceType.Area;
                linkItem.Url = url;
                linkItem.AnchorText = anchorText;
                AddLink(linkItem);
            }
        }

        private void ExtractFrameLink(int startTag)
        {
            HtmlNode node = GetNode(startTag);
            string url = node.GetAttributeValue(HtmlAttributeId.Src);
            if (url != null && url.Length > 0)
            {
                LinkItem linkItem = new LinkItem();
                linkItem.LinkSource = LinkSourceType.Frame;
                linkItem.Url = url;
                AddLink(linkItem);
            }
        }

        private void ExtractIFrameLink(int startTag)
        {
            HtmlNode node = GetNode(startTag);
            string url = node.GetAttributeValue(HtmlAttributeId.Src);
            if (url != null && url.Length > 0)
            {
                LinkItem linkItem = new LinkItem();
                linkItem.LinkSource = LinkSourceType.IFrame;
                linkItem.Url = url;
                AddLink(linkItem);
            }
        }

        private void ExtractLinkLink(int startTag)
        {
            HtmlNode node = GetNode(startTag);
            string url = node.GetAttributeValue(HtmlAttributeId.Src);
            if (url != null && url.Length > 0)
            {
                LinkItem linkItem = new LinkItem();
                linkItem.LinkSource = LinkSourceType.Link;
                linkItem.Url = url;
                AddLink(linkItem);
            }
        }
    }
}
