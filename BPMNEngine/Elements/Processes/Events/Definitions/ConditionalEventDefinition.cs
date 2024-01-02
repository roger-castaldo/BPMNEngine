using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "conditionalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class ConditionalEventDefinition : AElement, IEventDefinition
    {
        public ConditionalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Conditional;

        public bool IsValid(IReadonlyVariables variables)
        {
            if (
                ExtensionElement!=null &&
                ((ExtensionElements)ExtensionElement).Children != null &&
                ((ExtensionElements)ExtensionElement).Children.Any(ie => ie is ConditionSet set && set.Evaluate(variables))
            )
                return true;
            return false;
        }
    }
}
