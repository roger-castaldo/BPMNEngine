using BPMNEngine.Attributes;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmn", "collaboration")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal record Collaboration : AParentElement
    {
        public Collaboration(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err = (err?? []).Append("Collaboration requires at least 1 child element.");
                return false;
            }
            return res;
        }
    }
}
