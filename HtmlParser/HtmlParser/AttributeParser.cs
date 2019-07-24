using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace SmartSearch.Html
{
    public class AttributeItem
    {
        public HtmlAttributeId NameId;
        public string Name;
        public string Value;
        
        public AttributeItem()
        {
            NameId = HtmlAttributeId.Unknown;
            Name = null;
            Value = null;
        }
    };

    /// <summary>
    /// HTML Attribute Parser. Get attribute name and value pairs until reach the end of tag '>'
    /// </summary>
    class AttributeParser
    {
        enum ParsingState
        {
            AttributeStart,
            InAtrributeName,
            AfterAttributeName,
            AfterEqualMark
        };

        public static bool IgnoreUnknownHtmlAttribute = true;

        public static bool IsNameCharacter(char ch)
        {
            return (ch < 128 && (char.IsLetter(ch) || ch == '-'));
        }

        public static List<AttributeItem> PareseAttributes(string html, ref int pos)
        {
            List<AttributeItem> attributes = new List<AttributeItem>();

            bool fHasMoreAttributes = true;
            while (fHasMoreAttributes)
            {
                while (pos < html.Length && html[pos] != '>' && !IsNameCharacter(html[pos]))
                {
                    pos++;
                }

                if (pos < html.Length && IsNameCharacter(html[pos]))
                {
                    AttributeItem attribute = PareseAttribute(html, ref pos);
                    if (attribute != null)
                    {
                        attributes.Add(attribute);
                    }
                }
                else
                {
                    fHasMoreAttributes = false;
                }
            }

            while (pos < html.Length && html[pos] != '>')
            {
                pos++;
            }

            return attributes;
        }

        public static AttributeItem PareseAttribute(string html, ref int pos)
        {
            Debug.Assert(pos < html.Length);
            Debug.Assert(IsNameCharacter(html[pos]));

            AttributeItem attribute = new AttributeItem();

            int nameBegin = pos;
            int nameLength = 0;
            ParsingState state = ParsingState.InAtrributeName;

            for (; pos < html.Length && html[pos] != '>'; pos++)
            {
                if (html[pos] == '=') // <tagname attrib=
                {
                    state = ParsingState.AfterEqualMark;
                    pos++;
                    break;
                }
                else if (char.IsWhiteSpace(html[pos])) // <tagname attrib ...
                {
                    state = ParsingState.AfterAttributeName;
                }
                else
                {
                    if (state == ParsingState.InAtrributeName)
                    {
                        nameLength++;
                    }
                    else if (char.IsLetter(html[pos])) // <tagname attrib1 a...
                    {
                        Debug.Assert(state == ParsingState.AfterAttributeName);
                        nameBegin = pos;
                        nameLength = 1;
                        state = ParsingState.InAtrributeName;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (state == ParsingState.AfterEqualMark)
            {
                attribute.Name = html.Substring(nameBegin, nameLength);

                for (; pos < html.Length && html[pos] != '>'; pos++)
                {
                    if (char.IsWhiteSpace(html[pos]))
                    {
                        // do nothing
                    }
                    else
                    {
                        attribute.Value = ParseValue(html, ref pos);
                        break;
                    }
                }
            }

            if (attribute.Name != null && attribute.Value != null)
            {
                attribute.NameId = HtmlAttributes.GetAttributeId(attribute.Name);
                if (IgnoreUnknownHtmlAttribute && attribute.NameId == HtmlAttributeId.Unknown) // ignore unknown attributes
                {
                    return null;
                }

                return attribute;
            }
            else
            {
                return null;
            }
        }

        public static string ParseValue(string html, ref int pos)
        {
            Debug.Assert(pos < html.Length);

            int begin = pos;
            int length = 0;

            char firstChar = html[pos];
            if (firstChar == '\'' || firstChar == '"')
            {
                pos++;

                begin = pos;
                while (pos < html.Length && html[pos] != firstChar)
                {
                    pos++;
                    length++;
                }

                pos++;
            }
            else
            {
                while (pos < html.Length && html[pos] != '>' && !char.IsWhiteSpace(html[pos]))
                {
                    pos++;
                    length++;
                }
            }

            if (length > 0)
            {
                return html.Substring(begin, length);
            }

            return null;
        }

    }
}
