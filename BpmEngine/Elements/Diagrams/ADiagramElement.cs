using Microsoft.Maui.Graphics;
using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Diagrams
{
    [RequiredAttribute("bpmnElement")]
    internal abstract class ADiagramElement : AParentElement
    {
        public string bpmnElement { get { return this["bpmnElement"]; } }

        protected IElement _GetLinkedElement(Definition definition)
        {
            return definition.LocateElement(bpmnElement);
        }

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
            float minX = Math.Min(initial.Left, additional.Value.Left);
            float minY = Math.Min(initial.Top, additional.Value.Top);
            float maxX = Math.Max(initial.Left+initial.Width, additional.Value.X+additional.Value.Width);
            float maxY = Math.Max(initial.Top + initial.Height, additional.Value.Y+additional.Value.Height);
            return new RectF(minX, minY, Math.Abs(maxX-minX), Math.Abs(maxY-minY));
        }
    }
}
