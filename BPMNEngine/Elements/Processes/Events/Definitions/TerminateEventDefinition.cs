using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "terminateEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record TerminateEventDefinition : AParentElement, IEventDefinition
    {
        public TerminateEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public EventSubTypes Type
            => EventSubTypes.Terminate;
    }
}
