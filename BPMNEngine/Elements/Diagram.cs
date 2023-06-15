using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using BPMNEngine.Attributes;
using BPMNEngine.Drawing;

using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Diagrams;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces;
using BPMNEngine.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements
{
    [XMLTag("bpmndi","BPMNDiagram")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Diagram : AParentElement
    {

        public Diagram(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public SizeF Size
        {
            get
            {
                var minX = Children.OfType<ADiagramElement>().Min(ade=>ade.Rectangle.X);
                var maxX = Children.OfType<ADiagramElement>().Max(ade => ade.Rectangle.Width);
                var minY = Children.OfType<ADiagramElement>().Min(ade => ade.Rectangle.Y);
                var maxY = Children.OfType<ADiagramElement>().Max(ade => ade.Rectangle.Y+ade.Rectangle.Height);
                return new SizeF(maxX - minX, maxY - minY);
            }
        }

        public static readonly IFont DefaultFont = new Font("Arial");
        public const float FONT_SIZE = 10.5f;

        public static SkiaBitmapExportContext ProduceImage(int width,int height) =>new SkiaBitmapExportContext(width, height, 1.0f, dpi: 600,transparent:true);

        public IImage Render(ProcessPath path, Definition definition)
        {
            return Render(path, definition, null);
        }

        public IImage Render(ProcessPath path, Definition definition,string elemid)
        {
            return _Render(path, definition, elemid);
        }

        private IImage _Render(ProcessPath path, Definition definition, string elemid)
        {
            var size = Size;
            ADiagramElement diagElem = null;
            if (elemid!=null)
            {
                diagElem = Children.OfType<Plane>().SelectMany(p => p.Children).OfType<ADiagramElement>().FirstOrDefault(ade => ade.bpmnElement==elemid);
                size = new Size(diagElem.Rectangle.Width,diagElem.Rectangle.Height);
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
            var elem = Children.OfType<Plane>().SelectMany(p => p.Children).OfType<ADiagramElement>().FirstOrDefault(ade => ade.bpmnElement==elemid);
            return elem==null ? new RectF(0, 0, 0, 0) : elem.Rectangle;
        }

        public static Color GetColor(StepStatuses status)
        {
            Color ret = Colors.Black;
            switch (status)
            {
                case StepStatuses.Failed:
                    ret = Colors.Red;
                    break;
                case StepStatuses.Succeeded:
                    ret = Colors.Green;
                    break;
                case StepStatuses.Waiting:
                case StepStatuses.Started:
                    ret = Colors.Blue;
                    break;
                case StepStatuses.WaitingStart:
                    ret=Colors.DarkGoldenrod;
                    break;
                case StepStatuses.Aborted:
                    ret=Colors.Orange;
                    break;
            }
            return ret;
        }

        public static void DrawLines(ICanvas surface, Point[] points)
        {
            var prev = points.First();
            points.Skip(1)
                .Select((p,i) => {
                    var result = new { start = prev, end = p };
                    prev=p;
                    return result;
                })
                .ForEach(pair => surface.DrawLine(pair.start, pair.end));
        }
        

        public override bool IsValid(out string[] err)
        {
            if (!Children.Any())
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }

        internal bool RendersElement(string nextStep)
        {
            return Children.OfType<Plane>().SelectMany(p => p.Children).OfType<ADiagramElement>().Any(ade => ade.bpmnElement==nextStep);
        }
    }
}
