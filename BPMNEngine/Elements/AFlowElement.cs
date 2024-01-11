using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements
{
    [RequiredAttribute("sourceRef")]
    [RequiredAttribute("targetRef")]
    internal abstract class AFlowElement : AElement, IFlowElement
    {
        protected AFlowElement(XmlElement elem, XmlPrefixMap map, AElement parent) : 
            base(elem, map, parent)
        {
        }
        public string SourceRef => this["sourceRef"];
        public string TargetRef => this["targetRef"];
    }
}
