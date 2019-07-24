using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSearch.Html
{
    class HtmlUtility
    {
        public static string HtmlDecode(string text)
        {
            //TODO: implement it
            return text;
        }

        public static bool IsWhiteSpaceText(string html, int index, int length)
        {
            int maxIndex = Math.Min(html.Length - 1, index + length);
            for (int i = index; i < maxIndex; i++)
            {
                if (!char.IsWhiteSpace(html[i]))
                    return false;
            }

            return true;
        }

        public static string NormalizeText(string html, int index, int length)
        {
            StringBuilder builder = new StringBuilder(length);

            int maxIndex = Math.Min(html.Length - 1, index + length);

            bool first = true;
            int i = index;
            while (i < maxIndex)
            {
                // skip space;
                while (i < maxIndex && char.IsWhiteSpace(html[i]))
                {
                    i++;
                }

                if (i < maxIndex)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        builder.Append(' ');
                    }

                    while (i < maxIndex && !char.IsWhiteSpace(html[i]))
                    {
                        builder.Append(html[i]);
                        i++;
                    }
                }
            }

                
            return builder.ToString();
        }
    }
}
