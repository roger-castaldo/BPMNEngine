using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Collaborations
{
    [XMLTag("bpmn","textAnnotation")]
    [RequiredAttribute("id")]
    internal class TextAnnotation : AElement
    {
        public string Content
        {
            get
            {
                string ret = "";
                foreach (XmlNode n in SubNodes)
                {
                    if (n.Name=="bpmn:text")
                    {
                        ret = n.InnerText;
                        break;
                    }
                }
                return ret;
            }
        }

        public TextAnnotation(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override bool IsValid(out string err)
        {
            if (Content=="")
            {
                err = "No content was specified.";
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
