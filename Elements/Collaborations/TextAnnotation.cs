using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Collaborations
{
    [XMLTag("bpmn","textAnnotation")]
    [RequiredAttribute("id")]
    internal class TextAnnotation : AParentElement
    {
        public string Content
        {
            get
            {
                string ret = "";
                foreach (IElement elem in this.Children)
                {
                    if (elem is Text)
                    {
                        ret = ((Text)elem).Value;
                        break;
                    }
                }
                return ret;
            }
        }

        public TextAnnotation(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out string[] err)
        {
            if (Content=="")
            {
                err = new string[] { "No content was specified." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
