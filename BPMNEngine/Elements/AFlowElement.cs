using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements
{
    [RequiredAttributeAttribute("sourceRef")]
    [RequiredAttributeAttribute("targetRef")]
    internal abstract record AFlowElement : AElement, IFlowElement
    {
        protected AFlowElement(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public string? SourceRef => this["sourceRef"];
        public string? TargetRef => this["targetRef"];
    }
}
