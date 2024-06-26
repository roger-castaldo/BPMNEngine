using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "conditionalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record ConditionalEventDefinition : AElement, IEventDefinition
    {
        public ConditionalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Conditional;

        public bool IsValid(IReadonlyVariables variables)
            => ExtensionElement?.Children.OfType<ConditionSet>().Any(cset => cset.Evaluate(variables))??false;
    }
}
