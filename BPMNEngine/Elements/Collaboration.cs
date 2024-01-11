using BPMNEngine.Attributes;

namespace BPMNEngine.Elements
{
    [XMLTag("bpmn","collaboration")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Collaboration : AParentElement
    {
        public Collaboration(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "Collaboration requires at least 1 child element." });
                return false;
            }
            return res;
        }
    }
}
