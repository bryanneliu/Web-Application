using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SmartSearch.Html
{
    public enum HtmlTagId
    {
        A, Abbr, Acronym, Address,
        Applet, Area, B, Base,
        Basefont, Bdo, Big, Blockquote,
        Body, Br, Button, Caption,
        Center, Cite, Code, Col,
        Colgroup, Dd, Del, Dfn,
        Dir, Div, Dl, Dt,
        Em, Fieldset, Font, Form,
        Frame, Frameset, H1, H2,
        H3, H4, H5, H6,
        Head, Hr, Html, I,
        Iframe, Img, Input, Ins,
        Isindex, Kbd, Label, Legend,
        Li, Link, Map, Menu,
        Meta, Noframes, Noscript, Object,
        Ol, Optgroup, Option, P,
        Param, Pre, Q, S,
        Samp, Script, Select, Small,
        Span, Strike, Strong, Style,
        Sub, Sup, Table, Tbody,
        Td, Textarea, Tfoot, Th,
        Thead, Title, Tr, Tt,
        U, Ul, Var,
        
        Unknown
    };

    class HtmlTags
    {
        public int Count
        {
            get { return tagNames.Length; }
        }

        private static string[] tagNames = new string[]
        {
            "A",          "ABBR",       "ACRONYM",    "ADDRESS",    
            "APPLET",     "AREA",       "B",          "BASE",    
            "BASEFONT",   "BDO",        "BIG",        "BLOCKQUOTE",   
            "BODY",       "BR",         "BUTTON",     "CAPTION",    
            "CENTER",     "CITE",       "CODE",       "COL",    
            "COLGROUP",   "DD",         "DEL",        "DFN",    
            "DIR",        "DIV",        "DL",         "DT",    
            "EM",         "FIELDSET",   "FONT",       "FORM",    
            "FRAME",      "FRAMESET",   "H1",         "H2",    
            "H3",         "H4",         "H5",         "H6",    
            "HEAD",       "HR",         "HTML",        "I",    
            "IFRAME",     "IMG",        "INPUT",      "INS",    
            "ISINDEX",    "KBD",        "LABEL",      "LEGEND",    
            "LI",         "LINK",       "MAP",        "MENU",    
            "META",       "NOFRAMES",   "NOSCRIPT",   "OBJECT",    
            "OL",         "OPTGROUP",   "OPTION",     "P",    
            "PARAM",      "PRE",        "Q",          "S",    
            "SAMP",       "SCRIPT",     "SELECT",     "SMALL",    
            "SPAN",       "STRIKE",     "STRONG",     "STYLE",    
            "SUB",        "SUP",        "TABLE",      "TBODY",    
            "TD",         "TEXTAREA",   "TFOOT",      "TH",    
            "THEAD",      "TITLE",      "TR",         "TT",    
            "U",          "UL",         "VAR"
        };

        private static Dictionary<string, HtmlTagId> dictionary;

        static HtmlTags()
        {
            dictionary = new Dictionary<string, HtmlTagId>();
            for (HtmlTagId i = HtmlTagId.A; i != HtmlTagId.Unknown; i++)
            {
                int index = (int)i;
                Debug.Assert(index < tagNames.Length);
                dictionary.Add(tagNames[index], i);
            }
        }

        public static string GetTagName(HtmlTagId id)
        {
            int index = (int)id;
            if (index >= 0 && index < tagNames.Length)
            {
                return tagNames[index];
            }
            else
            {
                return "UNKNOWN";
            }
        }

        public static HtmlTagId GetTagId(string name)
        {
            string upperName = name.ToUpper();
            HtmlTagId id = HtmlTagId.Unknown;
            if (!dictionary.TryGetValue(upperName, out id))
            {
                id = HtmlTagId.Unknown;
            }

            return id;
        }
    }

    
}
