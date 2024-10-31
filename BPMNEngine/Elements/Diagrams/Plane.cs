using BPMNEngine.Attributes;
using BPMNEngine.Drawing;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.State;
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTagAttribute("bpmndi", "BPMNPlane")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Diagram))]
    internal record Plane : ADiagramElement, IRenderingElement
    {
        public Plane(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        private RectF? _rectangle;
        public override RectF Rectangle
        {
            get
            {
                if (_rectangle==null)
                    Children.OfType<ADiagramElement>().ForEach(ade => _rectangle=MergeRectangle(ade.Rectangle, _rectangle));
                return _rectangle??new(0, 0, 0, 0);
            }
        }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (!Children.Any())
            {
                errors = errors.Append("No child elements to render.");
                isValid=false;
            }
            return (isValid, errors);
        }

        public void Render(ICanvas surface, ProcessPath path, Definition definition)
            => Children.OfType<IRenderingElement>().ForEach(ire => ire.Render(surface, path, definition));
    }
}
