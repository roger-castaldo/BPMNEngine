using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTagAttribute("bpmn", "intermediateThrowEvent")]
    internal record IntermediateThrowEvent : AEvent
    {
        public object Message
            => SubType switch
            {
                EventSubTypes.Signal => Children
                            .OfType<SignalEventDefinition>()
                            .Select(child => child.SignalTypes.First())
                            .FirstOrDefault(),
                EventSubTypes.Error => Children
                        .OfType<ErrorEventDefinition>()
                        .Select(child => child.ErrorTypes.First())
                        .Select(err => new IntermediateProcessExcepion(err))
                        .FirstOrDefault(),
                EventSubTypes.Message => Children
                                                .OfType<MessageEventDefinition>()
                                                .Select(child => child.MessageTypes.First())
                                                .FirstOrDefault(),
                _ => null
            };

        public IntermediateThrowEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (!Incoming.Any())
                errors = errors.Append("Intermediate Throw Events must have an incoming path.");
            else if (Incoming.Count()!= 1)
                errors = errors.Append("Intermediate Throw Events must have only 1 incoming path.");
            return (isValid&&!errors.Any(), errors);
        }
    }
}
