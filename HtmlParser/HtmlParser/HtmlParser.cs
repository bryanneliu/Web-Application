//------------------------------------------------------------
// HTML Parser
// Author: tlwu@microsoft.com
// Create: 2006-02-01
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSearch.Html
{
    public class HtmlParser
    {
        MarkupParser markupParser;
        LinkExtractor linkExtractor;

        public HtmlParser()
        {
            markupParser = new MarkupParser();
            linkExtractor = new LinkExtractor();
        }

        /// <summary>
        /// parse a HTML
        /// </summary>
        /// <param name="html">content of HTML file</param>
        /// <returns>parsed result</returns>
        public ParsedHtml Parse(string html)
        {
            // markup parsing
            List<HtmlNode> nodes = markupParser.Parse(html);
            if (nodes == null)
                return null;

            // parsing links
            ParsedHtml parsedHtml = new ParsedHtml();
            parsedHtml.Html = html;
            parsedHtml.Nodes = nodes;
            
            
            // process parsed Html
            parsedHtml.Process();

            return parsedHtml;
        }

        public void ExtractLinks(ParsedHtml parsedHtml, bool extractSurroundingText)
        {
            linkExtractor.Process(parsedHtml, extractSurroundingText);
        }
    }
}
