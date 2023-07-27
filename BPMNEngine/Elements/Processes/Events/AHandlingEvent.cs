using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Events
{
    internal abstract class AHandlingEvent : AEvent
    {
        protected AHandlingEvent(XmlElement elem, XmlPrefixMap map, AElement parent) : 
            base(elem, map, parent)
        {}

        private IEnumerable<string> _types => Children
            .Where(child => child is ErrorEventDefinition || child is MessageEventDefinition || child is SignalEventDefinition)
            .Select(child => (child is ErrorEventDefinition ?
                    ((ErrorEventDefinition)child).ErrorTypes :
                    (child is MessageEventDefinition messageDefinition ?
                        messageDefinition.MessageTypes :
                ((SignalEventDefinition)child).SignalTypes)))
            .FirstOrDefault();

        private ConditionalEventDefinition _condition => Children
            .OfType<ConditionalEventDefinition>()
            .FirstOrDefault();

        public int EventCost(EventSubTypes evnt, object data, AFlowNode source, IReadonlyVariables variables)
        {
            bool handlesEvent = false;
            if (SubType.Value==evnt)
            {
                handlesEvent=true;
                switch (SubType.Value)
                {
                    case EventSubTypes.Message:
                    case EventSubTypes.Signal:
                        handlesEvent = _types.Contains((string)data)||_types.Contains("*");
                        break;
                    case EventSubTypes.Error:
                        Exception ex = (Exception)data;
                        handlesEvent = _types.Contains(ex.Message)||_types.Contains(ex.GetType().Name)||_types.Contains("*");
                        break;
                    case EventSubTypes.Conditional:
                        handlesEvent=_condition.IsValid(variables);
                        break;
                }
            }
            if (handlesEvent)
                return GetEventCost(evnt, source, variables);
            return int.MaxValue;
        }


        public override bool IsValid(out string[] err)
        {
            if (!SubType.HasValue)
            {
                err = new string[] { string.Format("{0}s must have a subtype.",new object[] { GetType().Name }) };
                return false;
            }
            switch (SubType.Value)
            {
                case EventSubTypes.Message:
                case EventSubTypes.Signal:
                case EventSubTypes.Error:
                    if (_types==null || !_types.Any())
                    {
                        err=new string[] { string.Format("A subtype of {0} must define the types to intercept", new object[] { SubType.Value }) };
                        return false;
                    }
                    break;
                case EventSubTypes.Conditional:
                    if (_condition==null)
                    {
                        err=new string[] { string.Format("A subtype of {0} must contains a ConditionalEventDefinition", new object[] { SubType.Value }) };
                        return false;
                    }
                    break;
            }
            return base.IsValid(out err);
        }
        protected abstract int GetEventCost(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables);

    }
}
