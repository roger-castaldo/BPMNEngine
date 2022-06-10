using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.Extensions
{
    [XMLTag("exts", "ErrorDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(ErrorEventDefinition))]
    internal class ErrorDefinition : AElement
    {
        public ErrorDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public string Type { get { return this["type"]; } }
        
        public override bool IsValid(out string[] err)
        {
            List<string> errors = new List<string>();
            if (Type == "*")
                errors.Add("An Error Definition cannot have the type of *, this is reserved");
            if (Parent.Parent is IntermediateThrowEvent && Type==null)
                errors.Add("An Error Definition for a Throw Event must have a Type defined");
            bool isError = base.IsValid(out err);
            isError |= errors.Count > 0;
            if (err != null)
                errors.AddRange(err);
            err = errors.ToArray();
            return isError;
        }
    }
}
