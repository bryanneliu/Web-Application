using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSearch.Html
{
    public enum HtmlNodeType
    {
        Element,
        Text,         // text between tags and not in comment
        Comment,      // comment text
        EndTag,
        Invalid
    };

    public class HtmlNode
    {
        public HtmlTagId TagId;
        public HtmlNodeType NodeType;
        public int Index;
        public int Length;
        public List<AttributeItem> Attributes;

        public HtmlNode()
        {
            TagId = HtmlTagId.Unknown;
            NodeType = HtmlNodeType.Invalid;
            Index = -1;
            Length = -1;
            Attributes = null;
        }

        public bool IsStartTag(HtmlTagId id)
        {
            return (NodeType == HtmlNodeType.Element && TagId == id);
        }

        public bool IsEndTag(HtmlTagId id)
        {
            return (NodeType == HtmlNodeType.EndTag && TagId == id);
        }

        public bool IsTextNode()
        {
            return (NodeType == HtmlNodeType.Text);
        }

        public AttributeItem GetAttribute(HtmlAttributeId name)
        {
            if (Attributes != null)
            {
                foreach (AttributeItem item in Attributes)
                {
                    if (item.NameId == name)
                        return item;
                }
            }
            return null;
        }

        public string GetAttributeValue(HtmlAttributeId name)
        {
            AttributeItem item = GetAttribute(name);
            if (item != null)
            {
                return item.Value;
            }

            return null;
        }
    };
}
