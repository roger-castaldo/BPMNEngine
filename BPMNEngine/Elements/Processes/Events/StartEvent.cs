﻿using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTagAttribute("bpmn", "startEvent")]
    internal record StartEvent : AEvent
    {
        public StartEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        internal bool IsEventStartValid(IReadonlyVariables variables, IsEventStartValid isEventStartValid)
            => (
                ExtensionElement==null ||
                ExtensionElement.Children.OfType<ConditionSet>().All(cset => cset.Evaluate(variables))
            )
            && isEventStartValid(this, variables);

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Incoming.Any(id => !OwningDefinition.MessageFlows.Any(mf => mf.ID==id)) && !SubType.HasValue)
            {
                err = (err ?? []).Append("Start Events cannot have an incoming path.");
                res=false;
            }
            if (!Outgoing.Any())
            {
                err = (err ?? []).Append("Start Events must have an outgoing path.");
                res = false;
            }
            else if (Outgoing.Count() > 1)
            {
                err = (err?? []).Append("Start Events can only have 1 outgoing path.");
                res = false;
            }
            return res;
        }
    }
}
