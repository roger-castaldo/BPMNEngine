using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Drawing;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNPlane")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Diagram))]
    internal class Plane : ADiagramElement, IRenderingElement
    {
        public Plane(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        private RectF? _rectangle;
        public override RectF Rectangle
        {
            get
            {
                if (_rectangle==null)
                    Children.OfType<ADiagramElement>().ForEach(ade => _rectangle=MergeRectangle(ade.Rectangle, _rectangle));
                return _rectangle.Value;
            }
        }

        public override bool IsValid(out string[] err)
        {
            if (!Children.Any())
            {
                err = new string[] { "No child elements to render." };
                return false;
            }
            return base.IsValid(out err);
        }

        public void Render(ICanvas surface, ProcessPath path, Definition definition)
        {
            Children.OfType<IRenderingElement>().ForEach(ire => ire.Render(surface, path, definition));
        }
    }
}
