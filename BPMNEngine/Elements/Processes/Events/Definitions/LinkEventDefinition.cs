using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "linkEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record LinkEventDefinition : AElement, IEventDefinition
    {
        public LinkEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public EventSubTypes Type => EventSubTypes.Link;
    }
}
