﻿using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions.TimerDefinition
{
    [XMLTag("exts", "DateString")]
    [ValidParent(typeof(TimerEventDefinition))]
    [ValidParent(typeof(ExtensionElements))]
    internal class XDateString : AElement
    {
        protected string Code 
            => this["Code"] ?? 
            SubNodes.Where(n=>n.NodeType==XmlNodeType.Text).Select(n=>n.Value).FirstOrDefault() ??
            SubNodes.Where(n=>n.NodeType==XmlNodeType.CDATA).Select(n=>((XmlCDataSection)n).Value).FirstOrDefault() ?? 
            string.Empty;

        public XDateString(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public override bool IsValid(out string[] err)
        {
            if (string.IsNullOrEmpty(Code))
            {
                err = new string[] { "No Date String Specified" };
                return false;
            }
            return base.IsValid(out err);
        }

        public DateTime GetTime(IReadonlyVariables variables)
        {
            if (string.IsNullOrEmpty(Code))
                throw new Exception("Invalid Date String Specified");
            var ds = new DateString(Code);
            return ds.GetTime(variables);
        }
    }
}
