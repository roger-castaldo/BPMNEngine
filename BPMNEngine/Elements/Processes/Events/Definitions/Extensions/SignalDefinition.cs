﻿using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions.Extensions
{
    [XMLTag("exts", "SignalDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(SignalEventDefinition))]
    internal class SignalDefinition : AElement
    {
        public SignalDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public string Type => this["type"];

        public override bool IsValid(out string[] err)
        {
            var errors = new List<string>();
            if (Type == "*")
                errors.Add("A Signal Definition cannot have the type of *, this is reserved");
            if (Parent.Parent is IntermediateThrowEvent && Type == null)
                errors.Add("A Signal Definition for a Throw Event must have a Type defined");
            bool isError = base.IsValid(out err);
            isError |= errors.Count > 0;
            if (err != null)
                errors.AddRange(err);
            err = errors.ToArray();
            return isError;
        }
    }
}
