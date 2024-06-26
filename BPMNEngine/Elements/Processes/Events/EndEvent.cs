using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTagAttribute("bpmn","endEvent")]
    internal record EndEvent : AEvent
    {
        private static readonly Type[] PROCESS_END_TYPE =
        [
            typeof(CompensationEventDefinition),
            typeof(ConditionalEventDefinition),
            typeof(ErrorEventDefinition),
            typeof(EscalationEventDefinition),
            typeof(LinkEventDefinition),
            typeof(SignalEventDefinition),
            typeof(MessageEventDefinition),
            typeof(TimerEventDefinition)
        ];

        public bool IsProcessEnd 
            => !Children
                .Any(child =>  PROCESS_END_TYPE.Contains(child.GetType()));

        public bool IsTermination 
            => Children
                .Any(child => child is TerminateEventDefinition);

        public EndEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Outgoing.Any()) 
            { 
                err=(err?? []).Append("End Events cannot have an outgoing path.");
                res=false;
            }
            if (!Incoming.Any())
            {
                err = (err?? []).Append("End Events must have an incoming path.");
                res=false;
            }
            return res;
        }
    }
}
