using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Collaborations
{
    [XMLTag("bpmn","textAnnotation")]
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

        public TextAnnotation(XmlElement elem)
            : base(elem) { }
    }
}
