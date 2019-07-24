//------------------------------------------------------------
// HTML Parser
// Author: tlwu@microsoft.com
// Create: 2006-02-01
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SmartSearch.Html
{
    public class MarkupParser
    {
        enum ParsingState
        {
            // parse markup
            InText,
            InScriptText,

            // parse tag
            TagStart,
            InTagName,
            InInvalidContent,
            AfterStartTagName,
        };

        private List<HtmlNode> nodes;
        private string content;
        
        //public static bool IgnoreScript = false;
        public static bool IgnoreComment = false;

        public List<HtmlNode> Parse(string doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            
            content = doc;
            nodes = new List<HtmlNode>();

            ParseMarkup();
            return nodes;
        }

        /// <summary>
        /// Move position to the end of comment
        /// </summary>
        /// <param name="pos">postion</param>
        private void SkipComment(ref int pos)
        {
            Debug.Assert(pos > 0 && content[pos-1] == '<' && content[pos] == '!') ;

            int begin = pos - 1;

            if (pos + 2 < content.Length
                && content[pos + 1] == '-' && content[pos + 2] == '-')
            {
                pos += 3;
                if (pos < content.Length)
                {
                    pos = content.IndexOf("-->", pos);
                    if (pos < 0)
                    {
                        pos = content.Length;
                    }
                    else
                    {
                        pos += 2;
                    }
                }
            }
            else
            {
                while (pos < content.Length && content[pos] != '>')
                {
                    pos++;
                }
            }

            int end = Math.Min(content.Length - 1, pos);
            if (end > begin)
            {
                OnComment(begin, end - begin + 1);
            }
        }

        private void ParseMarkup()
        {
            ParsingState state = ParsingState.InText;
            int textBegin = 0;
            int textLength = 0;

            for (int pos = 0; pos < content.Length; pos++)
            {
                char ch = content[pos];
                switch (state)
                {
                    case ParsingState.InText:
                        if (ch == '<')
                            state = ParsingState.TagStart;
                        else
                            textLength++;
                        break;
                    case ParsingState.InScriptText:
                        if (ch == '<' && (IsScriptEnd(pos) || IsCommentStart(pos)))
                            state = ParsingState.TagStart;
                        else
                            textLength++;
                        break;
                    case ParsingState.TagStart:
                        if (ch == '<')
                        {
                            textLength++;
                            state = ParsingState.TagStart;
                        }
                        else if (ch == '!')
                        {
                            OnText(textBegin, textLength);
                            SkipComment(ref pos);
                            textBegin = pos + 1;
                            textLength = 0;
                            state = ParsingState.InText;
                        }
                        else if (ch == '/' || char.IsLetter(ch))
                        {
                            // new tag begin, finish current text first
                            OnText(textBegin, textLength);

                            HtmlNode node = ParseTag(ref pos);

                            state = ParsingState.InText;
                            if (node != null && node.IsStartTag(HtmlTagId.Script))
                            {
                                //SkipScript();
                                state = ParsingState.InScriptText;
                            }

                            textBegin = pos + 1;
                            textLength = 0;
                        }
                        else
                        {
                            textLength += 2;
                            state = ParsingState.InText;
                        }
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }

            Debug.Assert(state == ParsingState.InText || state == ParsingState.InScriptText || state == ParsingState.TagStart);

            if (state == ParsingState.TagStart)
                textLength++;

            OnText(textBegin, textLength);
        }

        private bool IsScriptEnd(int pos)
        {
            if (content.Length - pos >= 8
             && content.Substring(pos, 8).Equals("</script", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        private bool IsCommentStart(int pos)
        {
            if (content.Length - pos >= 4
             && content.Substring(pos, 4).Equals("<!--"))
            {
                return true;
            }
            return false;
        }

        /*
        private bool SkipScript(ref int pos)
        {
            // skip all text between <script> and </script>
            for (; pos < content.Length; pos++)
            {
                if (content[pos] == '<')
                {
                    bool isComment = IsCommentStart(pos);
                    bool isScriptEnd = false;
                    if (!isComment)
                    {
                        isScriptEnd = IsScriptEnd(pos);
                    }
                    
                    //if (isComment || isScriptEnd) OnText(textBegin, textEnd);
                    if (isComment)
                    {
                        SkipComment(ref pos);
                    }

                    if (isScriptEnd)
                    {
                        return;
                    }
                }
            }
        }
        */ 

        private HtmlNode ParseTag(ref int pos)
        {
            Debug.Assert(pos < content.Length);
            Debug.Assert((content[pos] == '/') || char.IsLetter(content[pos]));

            HtmlNode node = null;

            int tagBegin = pos - 1;
            int tagEnd = pos;

            ParsingState state = ParsingState.TagStart;
            bool isStartTag = true;

            if (content[pos] == '/')
            {
                pos++;
                isStartTag = false;

                if (!(pos < content.Length && char.IsLetter(content[pos])))
                {
                    state = ParsingState.InInvalidContent; 
                }
            }

            HtmlTagId tagId = HtmlTagId.Unknown;
            List<AttributeItem> attributes = null;

            int tagNameBegin = pos;
            int tagNameEnd = pos;
            bool hasTagName = false;
            for (; pos < content.Length; pos++)
            {
                char ch = content[pos];
                switch (state)
                {
                    case ParsingState.TagStart:
                        Debug.Assert(char.IsLetter(ch));
                        tagNameBegin = pos;
                        tagNameEnd = pos + 1;
                        state = ParsingState.InTagName;
                        break;

                    case ParsingState.InTagName:
                        if (ch == '>') // <tagname>
                        {
                            hasTagName = true;
                        }
                        else if (char.IsWhiteSpace(ch))
                        {
                            hasTagName = true;
                            if (!isStartTag)
                            {
                                // skip invalid text in end tag
                                while (pos < content.Length && content[pos] != '>')
                                {
                                    pos++;
                                }
                            }
                            else
                            {
                                state = ParsingState.AfterStartTagName;
                            }
                        }
                        else
                        {
                            tagNameEnd++;
                        }

                        if (hasTagName)
                        {
                            string name = content.Substring(tagNameBegin, tagNameEnd - tagNameBegin);
                            tagId = HtmlTags.GetTagId(name);
                        }

                        break;

                    case ParsingState.InInvalidContent:
                        // do nothing
                        break;

                    case ParsingState.AfterStartTagName:
                        if (ch == '>' || char.IsWhiteSpace(ch))
                        {
                            // do nothing
                        }
                        else // <name ...
                        {
                            attributes = AttributeParser.PareseAttributes(content, ref pos);
                            Debug.Assert(pos >= content.Length || content[pos] == '>');
                        }
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }

                if (pos >= content.Length || content[pos] == '>')
                {
                    //pos++;
                    break;
                }
            }

            if (tagId != HtmlTagId.Unknown)
            {
                tagEnd = (pos < content.Length) ? pos : (content.Length - 1);
                int length = tagEnd - tagBegin + 1;
                if (length > 0)
                {
                    node = new HtmlNode();
                    node.Index = tagBegin;
                    node.Length = length;
                    node.TagId = tagId;
                    node.NodeType = isStartTag ? HtmlNodeType.Element : HtmlNodeType.EndTag;
                    node.Attributes = attributes;
                    AddNode(node);
                }
            }
#if DEBUG
            else
            {
                tagEnd = (pos < content.Length) ? pos : (content.Length - 1);
                int length = tagEnd - tagBegin + 1;
                string text = content.Substring(tagBegin, length);
                Debug.WriteLine("Unknown Tag!" + text);
            }
#endif
            return node;
        }

        private void OnText(int begin, int length)
        {
            OnText(begin, length, HtmlNodeType.Text);
        }

        private void OnComment(int begin, int length)
        {
            if (!IgnoreComment)
                OnText(begin, length, HtmlNodeType.Comment);
        }

        private void OnText(int begin, int length, HtmlNodeType nodeType)
        {
            if (begin + length > content.Length)
                return;

            if (HtmlUtility.IsWhiteSpaceText(content, begin, length))
                return;

            HtmlNode node = new HtmlNode();
            node.Index = begin;
            node.Length = length;
            node.NodeType = nodeType;
            
            AddNode(node);
        }

        private void AddNode(HtmlNode node)
        {
           this.nodes.Add(node);
        }
    }
}
