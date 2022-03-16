using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.Extensions
{
    [XMLTag("exts", "MessageDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(MessageEventDefinition))]
    internal class MessageDefinition : AElement
    {
        public MessageDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public string Name { get { return this["name"]; } }

        public override bool IsValid(out string[] err)
        {
            List<string> errors = new List<string>();
            if (Name == "*")
                errors.Add("A Message Definition cannot have the name of *, this is reserved");
            if (Parent.Parent is IntermediateThrowEvent && Name == null)
                errors.Add("A Message Definition for a Throw Event must have a Name defined");
            bool isError = base.IsValid(out err);
            isError |= errors.Count > 0;
            if (err != null)
                errors.AddRange(err);
            err = errors.ToArray();
            return isError;
        }
    }
}
