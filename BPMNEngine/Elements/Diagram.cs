using BPMNEngine.Attributes;
using BPMNEngine.Drawing;
using BPMNEngine.Elements.Diagrams;
using BPMNEngine.State;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmndi", "BPMNDiagram")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal record Diagram : AParentElement
    {
        public static readonly IFont DefaultFont = new Font("Arial");
        public const float FONT_SIZE = 10.5f;

        public Diagram(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public SizeF Size
            => new(
                Children.OfType<ADiagramElement>().Max(ade => ade.Rectangle.Width)-Children.OfType<ADiagramElement>().Min(ade => ade.Rectangle.X),
                Children.OfType<ADiagramElement>().Max(ade => ade.Rectangle.Y+ade.Rectangle.Height)-Children.OfType<ADiagramElement>().Min(ade => ade.Rectangle.Y)
            );

        public static SkiaBitmapExportContext ProduceImage(int width, int height)
            => new(width, height, 1.0f, dpi: 600, transparent: true);

        public IImage Render(ProcessPath path, Definition definition)
            => Render(path, definition, null);

        public IImage Render(ProcessPath path, Definition definition, string elemid)
        {
            var size = Size;
            ADiagramElement diagElem = null;
            if (elemid!=null)
            {
                diagElem = Children.OfType<Plane>().SelectMany(p => p.Children).OfType<ADiagramElement>().First(ade => ade.BPMNElement==elemid);
                size = new Size(diagElem.Rectangle.Width, diagElem.Rectangle.Height);
            }
            var image = ProduceImage((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
            var surface = image.Canvas;

            surface.Font = DefaultFont;
            surface.FontSize = FONT_SIZE;
            surface.FontColor = Colors.Black;

            if (diagElem!=null)
            {
                surface.Translate(0-diagElem.Rectangle.X, 0-diagElem.Rectangle.Y);
                if (diagElem is IRenderingElement renderingElement)
                    renderingElement.Render(surface, path, definition);
            }
            else
            {
                surface.Translate(0-Children.OfType<ADiagramElement>().Min(ade => ade.Rectangle.X), 0-Children.OfType<ADiagramElement>().Min(ade => ade.Rectangle.Y));
                Children.OfType<IRenderingElement>().ForEach(ire => { ire.Render(surface, path, definition); });
            }

            return image.Image;
        }

        public RectF GetElementRectangle(string elemid)
        {
            var element = Children.OfType<Plane>().SelectMany(p => p.Children).OfType<ADiagramElement>().FirstOrDefault(ade => ade.BPMNElement==elemid);
            return element==null ? new RectF(0, 0, 0, 0) : element.Rectangle;
        }

        public static Color GetColor(StepStatuses status)
            => status switch
            {
                StepStatuses.Failed => Colors.Red,
                StepStatuses.Succeeded => Colors.Green,
                StepStatuses.Waiting or StepStatuses.Started => Colors.Blue,
                StepStatuses.WaitingStart => Colors.DarkGoldenrod,
                StepStatuses.Aborted => Colors.Orange,
                _ => Colors.Black,
            };

        public static void DrawLines(ICanvas surface, Point[] points)
            => points.Skip(1)
                .Select((p, i) => new { start = points[i], end = p })
                .ForEach(pair => surface.DrawLine(pair.start, pair.end));


        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err = (err?? []).Append("No child elements found.");
                return false;
            }
            return res;
        }

        internal bool RendersElement(string nextStep)
            => Children.OfType<Plane>().SelectMany(p => p.Children).OfType<ADiagramElement>().Any(ade => ade.BPMNElement==nextStep);
    }
}
