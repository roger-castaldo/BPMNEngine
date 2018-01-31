using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.TimerDefinition
{
    [XMLTag("exts", "DateString")]
    [ValidParent(typeof(TimerEventDefinition))]
    [ValidParent(typeof(ExtensionElements))]
    internal class XDateString : AElement
    {
        protected string _Code
        {
            get
            {
                if (this["Code"] != null)
                    return this["Code"];
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Text)
                        return n.Value;
                }
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.CDATA)
                        return ((XmlCDataSection)n).InnerText;
                }
                return "";
            }
        }

        public XDateString(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public override bool IsValid(out string[] err)
        {
            if (_Code == "")
            {
                err = new string[] { "No Date String Specified" };
                return false;
            }
            return base.IsValid(out err);
        }

        public DateTime GetTime(ProcessVariablesContainer variables)
        {
            if (_Code == "")
                throw new Exception("Invalid Date String Specified");
            DateString ds = new BpmEngine.DateString(_Code);
            return ds.GetTime(variables);
        }
    }
}
