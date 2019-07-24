using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SmartSearch.Html
{
    enum InputItemType
    {
        Select,

        Checkbox,
        Radio,
        Hidden,

        Button,
        Submit,
        Reset
    }

    internal class InputItem
    {
        public string Name;
        public string Value;
        public InputItemType Type;
    }

    internal class HtmlForm
    {
        enum FormType
        {
            Get,
            Post
        };

        private List<InputItem> inputItems;
        private FormType formType;
        private bool isEnumerable;
        private string action;

        public bool HasValidLinks
        {
            get { return ((formType == FormType.Get) && isEnumerable); }
        }

        public HtmlForm()
        {
            this.inputItems = new List<InputItem>();
            this.formType = FormType.Get;
            this.isEnumerable = false;
            this.action = null;
        }

        public void AddInputItem(string name, string value, InputItemType type)
        {
            if (name != null && value != null)
            {
                InputItem item = new InputItem();
                item.Name = name;
                item.Value = value;
                item.Type = type;
                inputItems.Add(item);
            }
        }

        private void AddInputItem(HtmlNode node)
        {
            string type = node.GetAttributeValue(HtmlAttributeId.Type);
            string name = node.GetAttributeValue(HtmlAttributeId.Name);
            string value = node.GetAttributeValue(HtmlAttributeId.Value);

            if (type == "checkbox")
            {
                AddInputItem(name, value, InputItemType.Checkbox);
            }
            else if (type == "radio")
            {
                AddInputItem(name, value, InputItemType.Radio);
            }
            else if (type == "hidden")
            {
                AddInputItem(name, value, InputItemType.Hidden);
            }
            else if (type == "button")
            {
                AddInputItem(name, value, InputItemType.Button);
            }
            else if (type == "submit")
            {
                AddInputItem(name, value, InputItemType.Submit);
            }
            else if (type == "reset")
            {
                // ignore
            }
            else // file, image, password, text
            {
                this.isEnumerable = false;
            }
        }

        public static HtmlForm Parse(List<HtmlNode> nodes, int startTag, int endTag)
        {
            string action = nodes[startTag].GetAttributeValue(HtmlAttributeId.Action);
            //TODO: check whether action is a normal URL instead of javascript
            if (action == null)
            {
                return null;
            }


            HtmlForm form = new HtmlForm();
            form.action = action;

            string method = nodes[startTag].GetAttributeValue(HtmlAttributeId.Method);
            if (method == "post")
            {
                form.formType = FormType.Post;
            }

            for (int i = startTag + 1; i < endTag; i++)
            {
                if (nodes[i].IsStartTag(HtmlTagId.Input))
                {
                    form.AddInputItem(nodes[i]);
                }
                else if (nodes[i].IsStartTag(HtmlTagId.Select))
                {
                    // <SELECT ID="oSelect" NAME="Cars" SIZE="2" MULTIPLE>
                    // <OPTION VALUE="1" SELECTED>BMW
                    // <OPTION VALUE="2">Porsche
                    // </SELECT>

                    string name = nodes[i].GetAttributeValue(HtmlAttributeId.Name);
                    for (int j = i + 1; j < endTag; j++)
                    {
                        if (nodes[j].TagId == HtmlTagId.Option)
                        {
                            string value = nodes[j].GetAttributeValue(HtmlAttributeId.Value);
                            form.AddInputItem(name, value, InputItemType.Select);
                        }
                    }
                }
                else if (nodes[i].IsStartTag(HtmlTagId.Textarea))
                {
                    form.isEnumerable = false;
                }
            }

            return form;
        }

        
    }

    internal class InputBuilder
    {
        public static List<string> Add(List<string> querySet, string name, string value)
        {
            if (name != null && value != null)
            {
                string part = name + "=" + value;
                Add(querySet, part);
            }
            return querySet;
        }

        private static List<string> Add(List<string> querySet, string queryPart)
        {
            if (querySet.Count > 0)
            {
                for (int i = 0; i < querySet.Count; i++)
                {
                    querySet[i] += queryPart;
                }
            }
            else
            {
                querySet.Add(queryPart); // first part
            }

            return querySet;
        }

        public static List<string> Multiply(List<string> part1, List<string> part2)
        {
            if (part1.Count == 0)
                return part2;
            else if (part2.Count == 0)
                return part1;

            Debug.Assert(part1.Count > 0 && part2.Count > 0);

            List<string> querySet = new List<string>(part1.Count * part2.Count);
            for (int i = 0; i < part1.Count; i++)
            {
                for (int j = 0; j < part2.Count; j++)
                {
                    string query = part1[i] + "&" + part2[j];
                    querySet.Add(query);
                }
            }

            return querySet;
        }
    }

    class FormParser
    {
        public static List<HtmlForm> ParseForms(List<HtmlNode> nodes)
        {
            List<HtmlForm> forms = new List<HtmlForm>();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].IsStartTag(HtmlTagId.Form))
                {
                    int startTag = i;
                    int endTag = ParsedHtml.FindEndTag(nodes, startTag + 1, HtmlTagId.Form);
                    if (endTag > 0)
                    {
                        HtmlForm form = HtmlForm.Parse(nodes, startTag, endTag);
                        if (form != null)
                            forms.Add(form);
                        i = endTag;
                    }
                }
            }

            return forms;
        }
    }
}
