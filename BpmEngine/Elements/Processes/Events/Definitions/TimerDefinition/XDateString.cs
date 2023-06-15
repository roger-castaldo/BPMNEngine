using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Events.Definitions.TimerDefinition
{
    [XMLTag("exts", "DateString")]
    [ValidParent(typeof(TimerEventDefinition))]
    [ValidParent(typeof(ExtensionElements))]
    internal class XDateString : AElement
    {
        protected string _Code => this["Code"] ?? 
            SubNodes.Where(n=>n.NodeType==XmlNodeType.Text).Select(n=>n.Value).FirstOrDefault() ??
            SubNodes.Where(n=>n.NodeType==XmlNodeType.CDATA).Select(n=>((XmlCDataSection)n).Value).FirstOrDefault() ?? 
            String.Empty;

        public XDateString(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public override bool IsValid(out string[] err)
        {
            if (String.IsNullOrEmpty(_Code))
            {
                err = new string[] { "No Date String Specified" };
                return false;
            }
            return base.IsValid(out err);
        }

        public DateTime GetTime(IReadonlyVariables variables)
        {
            if (String.IsNullOrEmpty(_Code))
                throw new Exception("Invalid Date String Specified");
            var ds = new DateString(_Code);
            return ds.GetTime(variables);
        }
    }
}
