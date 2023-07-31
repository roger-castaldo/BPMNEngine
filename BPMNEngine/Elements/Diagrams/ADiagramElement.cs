using Microsoft.Maui.Graphics;
using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Diagrams
{
    [RequiredAttribute("bpmnElement")]
    internal abstract class ADiagramElement : AParentElement
    {
        public string BPMNElement => this["bpmnElement"];

        protected IElement GetLinkedElement(Definition definition)
            => definition.LocateElement(BPMNElement);

        public abstract RectF Rectangle { get; }

        public ADiagramElement(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected static Rect ProduceRectangle(Point p1, Point p2)
        {
            return new Rect(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X-p2.X),
                Math.Abs(p1.Y-p2.Y)
            );
        }

        protected static Rect MergeRectangle(RectF initial, RectF? additional)
        {
            if (additional==null)
                return initial;
            return new RectF(
                Math.Min(initial.Left, additional.Value.Left),
                Math.Min(initial.Top, additional.Value.Top),
                Math.Abs(Math.Max(initial.Left+initial.Width, additional.Value.X+additional.Value.Width) - Math.Min(initial.Left, additional.Value.Left)),
                Math.Abs(Math.Max(initial.Top + initial.Height, additional.Value.Y+additional.Value.Height) - Math.Min(initial.Top, additional.Value.Top))
            );
        }
    }
}
