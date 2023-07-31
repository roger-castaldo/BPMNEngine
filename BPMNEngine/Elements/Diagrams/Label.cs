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

        public override bool IsValid(out string[] err)
        {
            if (Bounds == null)
            {
                err = new string[] { "No bounds specified." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
