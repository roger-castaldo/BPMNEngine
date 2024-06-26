using BPMNEngine.Elements.Processes.Events.Definitions;
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

        protected AHandlingEvent(XmlElement elem, XmlPrefixMap map, AElement parent) : 
            base(elem, map, parent) {}

        public int EventCost(EventSubTypes evnt, object data, AFlowNode source, IReadonlyVariables variables)
        {
            if (SubType.Value==evnt)
            {
                var handlesEvent=SubType.Value switch
                {
                    EventSubTypes.Message=>Types.Any(t=>t.Equals(data)||t.Equals("*")),
                    EventSubTypes.Signal => Types.Any(t => t.Equals(data)||t.Equals("*")),
                    EventSubTypes.Conditional=>Condition.IsValid(variables),
                    EventSubTypes.Error =>(
                        (data is IntermediateProcessExcepion intermediateProcessException && Types.Any(t=>t.Equals(intermediateProcessException.ProcessMessage)||t.Equals(intermediateProcessException.Message)||t.Equals("*")))
                        ||(data is Exception exception && Types.Any(t=>t.Equals(exception.Message)||t.Equals(exception.GetType().Name)||t.Equals("*")))
                    ),
                    _ =>true
                };
                if (handlesEvent)
                    return GetEventCost(evnt, source, variables);
            }
            return int.MaxValue;
        }


        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!SubType.HasValue)
            {
                err = (err?? []).Append($"{GetType().Name}s must have a subtype.");
                return false;
            }
            return res;
        }
        protected abstract int GetEventCost(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables);

    }
}
