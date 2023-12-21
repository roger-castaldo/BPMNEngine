using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNLabel")]
    [ValidParent(typeof(Edge))]
    [ValidParent(typeof(Shape))]
    internal class Label : AParentElement
    {
        public Bounds Bounds
            => Children.OfType<Bounds>().FirstOrDefault();

        public Label(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Bounds == null)
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "No bounds for the label are specified." });
                return false;
            }
            return res;
        }
    }
}
