using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTagAttribute("bpmn", "endEvent")]
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
                .Any(child => PROCESS_END_TYPE.Contains(child.GetType()));

        public bool IsTermination
            => Children
                .Any(child => child is TerminateEventDefinition);

        public EndEvent(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (Outgoing.Any())
                errors = errors.Append("End Events cannot have an outgoing path.");
            if (!Incoming.Any())
                errors=errors.Append("End Events must have an incoming path.");
            return (isValid&&!errors.Any(), errors);
        }
    }
}
