using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    internal abstract record AHandlingEvent : AEvent
    {
        private IEnumerable<string> Types
            => Children
            .OfType<ErrorEventDefinition>()
            .FirstOrDefault()?.ErrorTypes
            ??Children.OfType<MessageEventDefinition>()
            .FirstOrDefault()?.MessageTypes
            ??Children.OfType<SignalEventDefinition>()
            .FirstOrDefault()?.SignalTypes;

        private ConditionalEventDefinition Condition
            => Children
            .OfType<ConditionalEventDefinition>()
            .FirstOrDefault();

        protected AHandlingEvent(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory)
        { }

        public async ValueTask<int> EventCostAsync(EventSubTypes evnt, object? data, AFlowNode source, IReadonlyVariables variables, ILogger? logger)
        {
            if (Equals(SubType,evnt))
            {
                var handlesEvent = SubType! switch
                {
                    EventSubTypes.Message => Types.Any(t => t.Equals(data)||t.Equals("*")),
                    EventSubTypes.Signal => Types.Any(t => t.Equals(data)||t.Equals("*")),
                    EventSubTypes.Conditional => await Condition.IsValidAsync(variables, logger),
                    EventSubTypes.Error => (
                        (data is IntermediateProcessExcepion intermediateProcessException && Types.Any(t => t.Equals(intermediateProcessException.ProcessMessage)||t.Equals(intermediateProcessException.Message)||t.Equals("*")))
                        ||(data is Exception exception && Types.Any(t => t.Equals(exception.Message)||t.Equals(exception.GetType().Name)||t.Equals("*")))
                    ),
                    _ => true
                };
                if (handlesEvent)
                    return GetEventCost(evnt, source, variables);
            }
            return int.MaxValue;
        }


        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (!SubType.HasValue)
            {
                errors = errors.Append($"{GetType().Name}s must have a subtype.");
                isValid = false;
            }
            return (isValid, errors);
        }
        protected abstract int GetEventCost(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables);

    }
}
