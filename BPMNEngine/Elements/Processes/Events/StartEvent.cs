using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTag("bpmn","startEvent")]
    internal class StartEvent : AEvent
    {
        public StartEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        internal bool IsEventStartValid(IReadonlyVariables variables, IsEventStartValid isEventStartValid)
        {
            if (ExtensionElement != null
                && ((ExtensionElements)ExtensionElement).Children!=null
                && ((ExtensionElements)ExtensionElement).Children.Any(ie => ie is ConditionSet set && !set.Evaluate(variables))
                )
                return false;
            return isEventStartValid(this, variables);
        }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Incoming.Any(id=> !Definition.MessageFlows.Any(mf=>mf.ID==id)) && !SubType.HasValue)
            {
                err = (err ?? Array.Empty<string>()).Concat(new string[] { "Start Events cannot have an incoming path." });
                res=false;
            }
            if (!Outgoing.Any())
            {
                err = (err ?? Array.Empty<string>()).Concat(new string[] { "Start Events must have an outgoing path." });
                res = false;
            }else if (Outgoing.Count() > 1)
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "Start Events can only have 1 outgoing path." });
                res = false;
            }
            return res;
        }
    }
}
