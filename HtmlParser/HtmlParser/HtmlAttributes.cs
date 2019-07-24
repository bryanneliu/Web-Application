using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SmartSearch.Html
{
    public enum HtmlAttributeId
    {
        Abbr, AcceptCharset, Accept,
        AccessKey, Action, Align,
        Alink, Alt, Archive,
        Axis, Background, Bgcolor,
        Border, CellPadding, CellSpacing,
        Char, CharOff, Charset,
        Checked, Cite, Class,
        Classid, Clear, Code,
        CodeBase, CodeType, Color,
        Cols, ColSpan, Compact,
        Content, Coords, Data,
        DateTime, Declare, Defer,
        Dir, Disabled, EncType,
        Face, For, Frame,
        FrameBorder, Headers, Height,
        Href, HrefLang, HSpace,
        HttpEquiv, Id, IsMap,
        Label, Lang, Language,
        Link, LongDesc, MarginHeight,
        MarginWidth, MaxLength, Media,
        Method, Multiple, Name,
        NoHref, NoResize, NoShade,
        NoWrap, Object, Onblur,
        OnChange, OnClick, OnDblClick,
        OnFocus, OnKeyDown, OnKeyPress,
        OnKeyUp, OnLoad, OnMouseDown,
        OnMouseMove, OnMouseOut, OnMouseOver,
        OnMouseUp, OnReset, OnSelect,
        OnSubmit, OnUnload, Profile,
        Prompt, ReadOnly, Rel,
        Rev, Rows, RowSpan,
        Rules, Scheme, Scope,
        Scrolling, Selected, Shape,
        Size, Span, Src,
        StandBy, Start, Style,
        Summary, TabIndex, Target,
        Text, Title, Type,
        UseMap, VAlign, Value,
        ValueType, Version, VLink,
        VSpace, Width,

        Unknown
    };

    class HtmlAttributes
    {
        private static string[] attributeNames = new string[]{
            "abbr",            "accept-charset",      "accept",
            "accesskey",       "action",              "align",
            "alink",           "alt",                 "archive",
            "axis",            "background",          "bgcolor",
            "border",          "cellpadding",         "cellspacing",
            "char",            "charoff",             "charset",
            "checked",         "cite",                "class",
            "classid",         "clear",               "code",
            "codebase",        "codetype",            "color",
            "cols",            "colspan",             "compact",
            "content",         "coords",              "data",
            "datetime",        "declare",             "defer",
            "dir",             "disabled",            "enctype",
            "face",            "for",                 "frame",
            "frameborder",     "headers",             "height",
            "href",            "hreflang",            "hspace",
            "http-equiv",      "id",                  "ismap",
            "labe",            "lang",                "language",
            "link",            "longdesc",            "marginheight",
            "marginwidth",     "maxlength",           "media",
            "method",          "multiple",            "name",
            "nohref",          "noresize",            "noshade",
            "nowrap",          "object",              "onblur",
            "onchange",        "onclick",             "ondblclick",
            "onfocus",         "onkeydown",           "onkeypress",
            "onkeyup",         "onload",              "onmousedown",
            "onmousemove",     "onmouseout",          "onmouseover",
            "onmouseup",       "onreset",             "onselect",
            "onsubmit",        "onunload",            "profile",
            "prompt",          "readonly",            "rel",
            "rev",             "rows",                "rowspan",
            "rules",           "scheme",              "scope",
            "scrolling",       "selected",            "shape",
            "size",            "span",                "src",
            "standby",         "start",               "style",
            "summary",         "tabindex",            "target",
            "text",            "title",               "type",
            "usemap",          "valign",              "value",
            "valuetype",       "version",             "vlink",
            "vspace",          "width"
        };

        private static Dictionary<string, HtmlAttributeId> dictionary;

        static HtmlAttributes()
        {
            Debug.Assert(attributeNames.Length == (int)HtmlAttributeId.Unknown);

            dictionary = new Dictionary<string, HtmlAttributeId>();
            for (HtmlAttributeId i = HtmlAttributeId.Abbr; i != HtmlAttributeId.Unknown; i++)
            {
                int index = (int)i;
                dictionary.Add(attributeNames[index], i);
            }
        }

        public static string GetAttributeName(HtmlAttributeId id)
        {
            int index = (int)id;
            if (index >= 0 && index < attributeNames.Length)
            {
                return attributeNames[index];
            }
            else
            {
                return "unknown";
            }
        }

        public static HtmlAttributeId GetAttributeId(string name)
        {
            string lowerName = name.ToLower();
            HtmlAttributeId id = HtmlAttributeId.Unknown;
            if (!dictionary.TryGetValue(lowerName, out id))
            {
                id = HtmlAttributeId.Unknown;
            }

            return id;
        }
    }
}
