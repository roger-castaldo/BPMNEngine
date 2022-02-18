using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Drawing;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Diagrams;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmndi","BPMNDiagram")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Diagram : AParentElement
    {
        private const float _SUB_PROCESS_CORNER_RADIUS = 10f;

        public Diagram(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public Size Size
        {
            get
            {
                int minX = 0;
                int maxX = 0;
                int minY = 0;
                int maxY = 0;
                foreach (IElement elem in Children)
                    _RecurGetDimensions(elem, ref minX, ref maxX, ref minY, ref maxY);
                return new Size(maxX - minX, maxY - minY);
            }
        }

        private void _RecurGetDimensions(IElement elem, ref int minX, ref int maxX, ref int minY, ref int maxY)
        {
            if (elem is Bounds)
            {
                Bounds b = (Bounds)elem;
                minX = Math.Min(minX, (int)b.Rectangle.X);
                maxX = Math.Max(maxX, (int)b.Rectangle.X + (int)b.Rectangle.Width);
                minY = Math.Min(minY, (int)b.Rectangle.Y);
                maxY = Math.Max(maxY, (int)b.Rectangle.Y + (int)b.Rectangle.Height);
            }
            else if (elem is Waypoint)
            {
                Waypoint w = (Waypoint)elem;
                minX = Math.Min(minX, w.Point.X);
                maxX = Math.Max(maxX, w.Point.X);
                minY = Math.Min(minY, w.Point.Y);
                maxY = Math.Max(maxY, w.Point.Y);
            }
            if (new List<Type>(elem.GetType().GetInterfaces()).Contains(typeof(IParentElement)))
            {
                foreach (IElement ie in ((IParentElement)elem).Children)
                    _RecurGetDimensions(ie, ref minX, ref maxX, ref minY, ref maxY);
            }
        }

        private Edge[] _Edges
        {
            get
            {
                List<Edge> ret = new List<Edge>();
                foreach (IElement elem in Children)
                    _RecurLocateEdges(elem, ref ret);
                return ret.ToArray();
            }
        }

        private void _RecurLocateEdges(IElement elem, ref List<Edge> edges)
        {
            if (elem is Edge)
                edges.Add((Edge)elem);
            if (elem is IParentElement)
            {
                foreach (IElement celem in ((IParentElement)elem).Children)
                    _RecurLocateEdges(celem, ref edges);
            }
        }

        private Shape[] _Shapes
        {
            get
            {
                List<Shape> ret = new List<Shape>();
                foreach (IElement elem in Children)
                    _RecurLocateShapes(elem, ref ret);
                return ret.ToArray();
            }
        }

        private void _RecurLocateShapes(IElement elem, ref List<Shape> shapes)
        {
            if (elem is Shape)
                shapes.Add((Shape)elem);
            if (elem is IParentElement)
            {
                foreach (IElement celem in ((IParentElement)elem).Children)
                    _RecurLocateShapes(celem, ref shapes);
            }
        }

        public Image Render(ProcessPath path, Definition definition)
        {
            return _Render(path, definition, null);
        }     

        private RectangleF? _ShiftRectangle(RectangleF? rectangle)
        {
            if (rectangle.HasValue) {
                int minX;
                int minY;
                int maxX;
                int maxY;
                _CalculateDimensions(out minX,out maxX,out minY,out maxY);
                return new RectangleF(Math.Abs(minX) + rectangle.Value.X, Math.Abs(minY) + rectangle.Value.Y, rectangle.Value.Width, rectangle.Value.Height);
            }
            return rectangle;
        }

        private int? _minX = null;
        private int? _minY = null;
        private int? _maxX = null;
        private int? _maxY = null;

        private void _CalculateDimensions(out int minX,out int maxX,out int minY,out int maxY)
        {
            if (!_minX.HasValue)
            {
                minX = 0;
                maxX = 0;
                minY = 0;
                maxY = 0;
                foreach (IElement elem in Children)
                    _RecurGetDimensions(elem, ref minX, ref maxX, ref minY, ref maxY);
                _minX = minX;
                _minY = minY;
                _maxX = maxX;
                _maxY = maxY;
            }
            else
            {
                minX = _minX.Value;
                minY = _minY.Value;
                maxX = _maxX.Value;
                maxY = _maxY.Value;
            }

        }

        private Image _Render(ProcessPath path, Definition definition, string elemid)
        {
            Size sz = Size;
            Bitmap bmp = new Bitmap(sz.Width, sz.Height);
            int minX;
            int minY;
            int maxX;
            int maxY;
            _CalculateDimensions(out minX, out maxX, out minY, out maxY);
            Graphics gp = Graphics.FromImage(bmp);
            gp.TranslateTransform(Math.Abs(minX), Math.Abs(minY));
            foreach (Shape shape in _Shapes)
            {
                if (shape.bpmnElement == (elemid == null ? shape.bpmnElement : elemid))
                    gp.DrawImage(_RenderShape(shape, path.GetStatus(shape.bpmnElement), shape.GetIcon(definition), definition.LocateElement(shape.bpmnElement)), shape.Rectangle);
            }
            foreach (Edge edge in _Edges)
            {
                if (edge.bpmnElement == (elemid == null ? edge.bpmnElement : elemid))
                    gp.DrawImage(_RenderEdge(edge, path.GetStatus(edge.bpmnElement), definition), edge.Rectangle);
            }
            return bmp;
        }

        internal Image RenderElement(ProcessPath path, Definition definition, string elementID,out RectangleF? rectangle)
        {
            foreach (Shape shape in _Shapes)
            {
                if (shape.bpmnElement == elementID)
                {
                    rectangle = _ShiftRectangle(shape.Rectangle);
                    return _RenderShape(shape, path.GetStatus(shape.bpmnElement), shape.GetIcon(definition), definition.LocateElement(shape.bpmnElement));
                }
            }
            foreach(Edge edge in _Edges)
            {
                if (edge.bpmnElement == elementID)
                {
                    rectangle = _ShiftRectangle(edge.Rectangle);
                    return _RenderEdge(edge, path.GetStatus(edge.bpmnElement), definition);
                }
            }
            rectangle = null;
            return null;
        }

        private Image _RenderEdge(Edge edge, StepStatuses status, Definition definition)
        {
            Bitmap ret = new Bitmap((int)edge.Rectangle.Width, (int)edge.Rectangle.Height);
            Graphics gp = Graphics.FromImage(ret);
            gp.TranslateTransform(0 - edge.Rectangle.X, 0 - edge.Rectangle.Y);
            gp.DrawLines(edge.ConstructPen(_GetBrush(status), definition), edge.Points);
            edge.AppendEnds(gp, _GetBrush(status), definition);
            if (edge.Label != null)
            {
                IElement elem = definition.LocateElement(edge.bpmnElement);
                if (elem != null)
                {
                    SizeF sf = gp.MeasureString(elem.ToString(), Constants.FONT, new SizeF(edge.Label.Bounds.Rectangle.Width, edge.Label.Bounds.Rectangle.Height), Constants.STRING_FORMAT);
                    gp.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status), new RectangleF(edge.Label.Bounds.Rectangle.X, edge.Label.Bounds.Rectangle.Y, Math.Max(edge.Label.Bounds.Rectangle.Width, sf.Width), Math.Max(edge.Label.Bounds.Rectangle.Height, sf.Height)), Constants.STRING_FORMAT);
                }
            }
            return ret;
        }

        private Image _RenderShape(Shape shape, StepStatuses status, BPMIcons? icon,IElement elem)
        {
            Bitmap ret = new Bitmap((int)Math.Ceiling(shape.Rectangle.Width), (int)Math.Ceiling(shape.Rectangle.Height));
            Graphics gp = Graphics.FromImage(ret);
            gp.TranslateTransform(0 - shape.Rectangle.X, 0 - shape.Rectangle.Y);
            if (icon.HasValue)
            {
                Rectangle rect = new Rectangle(0, 0, 0, 0);
                switch (icon.Value)
                {
                    case BPMIcons.Task:
                    case BPMIcons.SendTask:
                    case BPMIcons.ReceiveTask:
                    case BPMIcons.UserTask:
                    case BPMIcons.ManualTask:
                    case BPMIcons.ServiceTask:
                    case BPMIcons.ScriptTask:
                    case BPMIcons.BusinessRuleTask:
                        Pen p = new Pen(_GetBrush(status), Constants.PEN_WIDTH);
                        gp.DrawPath(p,_GenerateRoundedRectangle(shape.Rectangle.X, shape.Rectangle.Y, shape.Rectangle.Width, shape.Rectangle.Height));
                        IconGraphic.AppendIcon(new RectangleF(shape.Rectangle.X + 5, shape.Rectangle.Y + 5, 15, 15), icon.Value, gp, _GetColor(status));
                        break;
                    default:
                        IconGraphic.AppendIcon(shape.Rectangle, icon.Value, gp, _GetColor(status));
                        break;
                }
            }
            if (elem != null)
            {
                if (elem is TextAnnotation)
                    gp.DrawLines(new Pen(_GetBrush(status), Constants.PEN_WIDTH), new PointF[]{
                            new PointF(shape.Rectangle.X+20,shape.Rectangle.Y),
                            new PointF(shape.Rectangle.X,shape.Rectangle.Y),
                            new PointF(shape.Rectangle.X,shape.Rectangle.Y+shape.Rectangle.Height),
                            new PointF(shape.Rectangle.X+20,shape.Rectangle.Y+shape.Rectangle.Height)
                        });
                else if (elem is Lane || elem is Participant)
                    gp.DrawRectangle(new Pen(_GetBrush(status), Constants.PEN_WIDTH), Rectangle.Round(shape.Rectangle));
                else if (elem is SubProcess)
                    gp.DrawPath(new Pen(_GetBrush(status), Constants.PEN_WIDTH), _GenerateRoundedRectangle(shape.Rectangle.X, shape.Rectangle.Y, shape.Rectangle.Width, shape.Rectangle.Height));
                if (elem.ToString() != "")
                {
                    if (shape.Label != null)
                    {
                        SizeF sf = gp.MeasureString(elem.ToString(), Constants.FONT, new SizeF(shape.Label.Bounds.Rectangle.Width, float.MaxValue), Constants.STRING_FORMAT);
                        gp.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status), new RectangleF(shape.Label.Bounds.Rectangle.X, shape.Label.Bounds.Rectangle.Y, Math.Max(shape.Label.Bounds.Rectangle.Width, sf.Width), Math.Max(shape.Label.Bounds.Rectangle.Height, sf.Height)), Constants.STRING_FORMAT);
                    }
                    else
                    {
                        SizeF size = gp.MeasureString(elem.ToString(), Constants.FONT);
                        if (size.Height != 0 || size.Width != 0)
                        {
                            if (elem is Lane || elem is LaneSet || elem is Participant)
                            {
                                Bitmap tbmp = new Bitmap((int)size.Height * 2, (int)size.Width);
                                Graphics g = Graphics.FromImage(tbmp);
                                g.TranslateTransform(tbmp.Width / 2, tbmp.Height);
                                g.RotateTransform(-90);
                                g.TranslateTransform(0, 0);
                                g.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status), 0, 0);
                                g.Save();
                                gp.DrawImage(tbmp, new PointF(shape.Rectangle.X - 7, shape.Rectangle.Y + ((shape.Rectangle.Height - tbmp.Height) / 2)));
                            }
                            else
                                gp.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status), new RectangleF(shape.Rectangle.X + 0.5f, shape.Rectangle.Y + 15, shape.Rectangle.Width - 1, shape.Rectangle.Height - 15.5f), Constants.STRING_FORMAT);
                        }
                    }
                }
            }
            return ret;
        }

        private GraphicsPath _GenerateRoundedRectangle(float XPosition, float YPosition, float Width, float Height)
        {
            GraphicsPath ret = new GraphicsPath();
            ret.AddLine(XPosition + _SUB_PROCESS_CORNER_RADIUS, YPosition, XPosition + Width - (_SUB_PROCESS_CORNER_RADIUS * 2), YPosition);
            ret.AddArc(XPosition + Width - (_SUB_PROCESS_CORNER_RADIUS * 2), YPosition, _SUB_PROCESS_CORNER_RADIUS * 2, _SUB_PROCESS_CORNER_RADIUS * 2, 270, 90);
            ret.AddLine(XPosition + Width, YPosition + _SUB_PROCESS_CORNER_RADIUS, XPosition + Width, YPosition + Height - (_SUB_PROCESS_CORNER_RADIUS * 2));
            ret.AddArc(XPosition + Width - (_SUB_PROCESS_CORNER_RADIUS * 2), YPosition + Height - (_SUB_PROCESS_CORNER_RADIUS * 2), _SUB_PROCESS_CORNER_RADIUS * 2, _SUB_PROCESS_CORNER_RADIUS * 2, 0, 90);
            ret.AddLine(XPosition + Width - (_SUB_PROCESS_CORNER_RADIUS * 2), YPosition + Height, XPosition + _SUB_PROCESS_CORNER_RADIUS, YPosition + Height);
            ret.AddArc(XPosition, YPosition + Height - (_SUB_PROCESS_CORNER_RADIUS * 2), _SUB_PROCESS_CORNER_RADIUS * 2, _SUB_PROCESS_CORNER_RADIUS * 2, 90, 90);
            ret.AddLine(XPosition, YPosition + Height - (_SUB_PROCESS_CORNER_RADIUS * 2), XPosition, YPosition + _SUB_PROCESS_CORNER_RADIUS);
            ret.AddArc(XPosition, YPosition, _SUB_PROCESS_CORNER_RADIUS * 2, _SUB_PROCESS_CORNER_RADIUS * 2, 180, 90);
            ret.CloseFigure();
            return ret;
        }

        private Color _GetColor(StepStatuses status)
        {
            Color ret = Color.Black;
            switch (status)
            {
                case StepStatuses.Failed:
                    ret = Color.Red;
                    break;
                case StepStatuses.Succeeded:
                    ret = Color.Green;
                    break;
                case StepStatuses.Waiting:
                    ret = Color.Blue;
                    break;
            }
            return ret;
        }

        private Brush _GetBrush(StepStatuses status)
        {
            return new SolidBrush(_GetColor(status));
        }

        public override bool IsValid(out string[] err)
        {
            if (Children.Length == 0)
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }

        internal bool RendersElement(string nextStep)
        {
            foreach (Shape shape in _Shapes)
            {
                if (shape.bpmnElement == nextStep)
                    return true;
            }
            foreach (Edge edge in _Edges)
            {
                if (edge.bpmnElement == nextStep)
                    return true;
            }
            return false;
        }

        internal Image UpdateState(ProcessPath path, Definition elem, string nextStep)
        {
            return _Render(path, elem, nextStep);
        }
    }
}
