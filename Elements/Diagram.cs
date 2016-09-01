using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Diagrams;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmndi","BPMNDiagram")]
    internal class Diagram : AParentElement
    {
        public string bpmnElement { get { return _GetAttributeValue("bpmnElement"); } }

        public Diagram(XmlElement elem)
            : base(elem) { }

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
                minX = Math.Min(minX, b.Rectangle.X);
                maxX = Math.Max(maxX, b.Rectangle.X + b.Rectangle.Width);
                minY = Math.Min(minY, b.Rectangle.Y);
                maxY = Math.Max(maxY, b.Rectangle.Y + b.Rectangle.Height);
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
            Size sz = Size;
            Bitmap bmp = new Bitmap(sz.Width, sz.Height);
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;
            foreach (IElement elem in Children)
                _RecurGetDimensions(elem, ref minX, ref maxX, ref minY, ref maxY);
            Graphics gp = Graphics.FromImage(bmp);
            gp.TranslateTransform(Math.Abs(minX), Math.Abs(minY));
            foreach (Shape shape in _Shapes)
            {
                StepStatuses status = path.GetStatus(shape.bpmnElement);
                BPMIcons? icon = shape.GetIcon(definition);
                IElement elem = definition.LocateElement(shape.bpmnElement);
                if (icon.HasValue)
                {
                    Image img = Bitmap.FromStream(Utility.LocateEmbededResource(_GetImageStreamName(status)));
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
                            gp.DrawEllipse(p, new RectangleF(shape.Rectangle.X, shape.Rectangle.Y, 11, 11));
                            gp.DrawEllipse(p, new RectangleF(shape.Rectangle.X, shape.Rectangle.Y + shape.Rectangle.Height - 11, 11, 11));
                            gp.DrawEllipse(p, new RectangleF(shape.Rectangle.X + shape.Rectangle.Width - 11, shape.Rectangle.Y, 11, 11));
                            gp.DrawEllipse(p, new RectangleF(shape.Rectangle.X + shape.Rectangle.Width - 11, shape.Rectangle.Y + shape.Rectangle.Height - 11, 11, 11));
                            gp.FillPolygon(Brushes.White, new Point[]{
                                new Point(shape.Rectangle.X,shape.Rectangle.Y+5),
                                new Point(shape.Rectangle.X+5,shape.Rectangle.Y+5),
                                new Point(shape.Rectangle.X+5,shape.Rectangle.Y),
                                new Point(shape.Rectangle.X+shape.Rectangle.Width-5,shape.Rectangle.Y),
                                new Point(shape.Rectangle.X+shape.Rectangle.Width-5,shape.Rectangle.Y+5),
                                new Point(shape.Rectangle.X+shape.Rectangle.Width,shape.Rectangle.Y+5),
                                new Point(shape.Rectangle.X+shape.Rectangle.Width,shape.Rectangle.Y+shape.Rectangle.Height-5),
                                new Point(shape.Rectangle.X+shape.Rectangle.Width-5,shape.Rectangle.Y+shape.Rectangle.Height-5),
                                new Point(shape.Rectangle.X+shape.Rectangle.Width-5,shape.Rectangle.Y+shape.Rectangle.Height),
                                new Point(shape.Rectangle.X+5,shape.Rectangle.Y+shape.Rectangle.Height),
                                new Point(shape.Rectangle.X+5,shape.Rectangle.Y+shape.Rectangle.Height-5),
                                new Point(shape.Rectangle.X,shape.Rectangle.Y+shape.Rectangle.Height-5),
                                new Point(shape.Rectangle.X,shape.Rectangle.Y+5)
                            });
                            gp.DrawLine(p, new Point(shape.Rectangle.X + 5, shape.Rectangle.Y), new Point(shape.Rectangle.X + shape.Rectangle.Width - 5, shape.Rectangle.Y));
                            gp.DrawLine(p, new Point(shape.Rectangle.X + shape.Rectangle.Width, shape.Rectangle.Y + 5), new Point(shape.Rectangle.X + shape.Rectangle.Width, shape.Rectangle.Y + shape.Rectangle.Height - 5));
                            gp.DrawLine(p, new Point(shape.Rectangle.X + 5, shape.Rectangle.Y + shape.Rectangle.Height), new Point(shape.Rectangle.X + shape.Rectangle.Width - 5, shape.Rectangle.Y + shape.Rectangle.Height));
                            gp.DrawLine(p, new Point(shape.Rectangle.X, shape.Rectangle.Y + 5), new Point(shape.Rectangle.X, shape.Rectangle.Y + shape.Rectangle.Height - 5));
                            switch (icon.Value)
                            {
                                case BPMIcons.Task:
                                    rect = new Rectangle(0, 0, 1, 1);
                                    break;
                                case BPMIcons.SendTask:
                                    rect = new Rectangle(278, 10, 46, 30);
                                    break;
                                case BPMIcons.ReceiveTask:
                                    rect = new Rectangle(330, 10, 46, 30);
                                    break;
                                case BPMIcons.UserTask:
                                    rect = new Rectangle(274, 52, 40, 47);
                                    break;
                                case BPMIcons.ManualTask:
                                    rect = new Rectangle(327, 53,55 , 36);
                                    break;
                                case BPMIcons.ServiceTask:
                                    rect = new Rectangle(335,100,48,45);
                                    break;
                                case BPMIcons.ScriptTask:
                                    rect = new Rectangle(385, 8, 33, 37);
                                    break;
                                case BPMIcons.BusinessRuleTask:
                                    rect = new Rectangle(272, 111, 49, 30);
                                    break;
                            }
                            gp.DrawImage(img, new Rectangle(shape.Rectangle.X+5,shape.Rectangle.Y+5,20,15), rect, GraphicsUnit.Pixel);
                            break;
                        default:
                            switch (icon.Value)
                            {
                                case BPMIcons.StartEvent:
                                    rect = new Rectangle(7, 5, 46, 46);
                                    break;
                                case BPMIcons.MessageStartEvent:
                                    rect = new Rectangle(62, 5, 46, 46);
                                    break;
                                case BPMIcons.TimerStartEvent:
                                    rect = new Rectangle(115, 5, 46, 46);
                                    break;
                                case BPMIcons.ConditionalStartEvent:
                                    rect = new Rectangle(168, 5, 46, 46);
                                    break;
                                case BPMIcons.SignalStartEvent:
                                    rect = new Rectangle(220, 5, 46, 46);
                                    break;
                                case BPMIcons.MessageIntermediateThrowEvent:
                                    rect = new Rectangle(8, 56, 46, 46);
                                    break;
                                case BPMIcons.EscalationIntermediateThrowEvent:
                                    rect = new Rectangle(62, 56, 46, 46);
                                    break;
                                case BPMIcons.LinkIntermediateThrowEvent:
                                    rect = new Rectangle(116, 56, 46, 46);
                                    break;
                                case BPMIcons.CompensationIntermediateThrowEvent:
                                    rect = new Rectangle(169, 56, 46, 46);
                                    break;
                                case BPMIcons.SignalIntermediateThrowEvent:
                                    rect = new Rectangle(221, 56, 46, 46);
                                    break;
                                case BPMIcons.MessageIntermediateCatchEvent:
                                    rect = new Rectangle(8, 107, 46, 46);
                                    break;
                                case BPMIcons.TimerIntermediateCatchEvent:
                                    rect = new Rectangle(62, 107, 46, 46);
                                    break;
                                case BPMIcons.ConditionalIntermediateCatchEvent:
                                    rect = new Rectangle(116, 107, 46, 46);
                                    break;
                                case BPMIcons.LinkIntermediateCatchEvent:
                                    rect = new Rectangle(169, 107, 46, 46);
                                    break;
                                case BPMIcons.SignalIntermediateCatchEvent:
                                    rect = new Rectangle(221, 107, 46, 46);
                                    break;
                                case BPMIcons.EndEvent:
                                    rect = new Rectangle(6, 160, 48, 48);
                                    break;
                                case BPMIcons.MessageEndEvent:
                                    rect = new Rectangle(61, 160, 48, 48);
                                    break;
                                case BPMIcons.EscalationEndEvent:
                                    rect = new Rectangle(114, 160, 48, 48);
                                    break;
                                case BPMIcons.ErrorEndEvent:
                                    rect = new Rectangle(167, 160, 48, 48);
                                    break;
                                case BPMIcons.CompensationEndEvent:
                                    rect = new Rectangle(220, 160, 48, 48);
                                    break;
                                case BPMIcons.SignalEndEvent:
                                    rect = new Rectangle(274, 160, 48, 48);
                                    break;
                                case BPMIcons.TerminateEndEvent:
                                    rect = new Rectangle(332, 160, 48, 48);
                                    break;
                                case BPMIcons.ExclusiveGateway:
                                    rect = new Rectangle(8, 214, 63, 63);
                                    break;
                                case BPMIcons.ParallelGateway:
                                    rect = new Rectangle(77, 214, 63, 63);
                                    break;
                                case BPMIcons.InclusiveGateway:
                                    rect = new Rectangle(149, 214, 63, 63);
                                    break;
                                case BPMIcons.ComplexGateway:
                                    rect = new Rectangle(222, 214, 63, 63);
                                    break;
                                case BPMIcons.EventBasedGateway:
                                    rect = new Rectangle(292, 214, 63, 63);
                                    break;
                            }
                            gp.DrawImage(img, shape.Rectangle, rect, GraphicsUnit.Pixel);
                            break;
                    }
                }
                if (elem != null)
                {
                    if (elem is TextAnnotation)
                        gp.DrawLines(new Pen(_GetBrush(status), Constants.PEN_WIDTH), new Point[]{
                            new Point(shape.Rectangle.X+20,shape.Rectangle.Y),
                            new Point(shape.Rectangle.X,shape.Rectangle.Y),
                            new Point(shape.Rectangle.X,shape.Rectangle.Y+shape.Rectangle.Height),
                            new Point(shape.Rectangle.X+20,shape.Rectangle.Y+shape.Rectangle.Height)
                        });
                    else if (elem is Lane || elem is Participant)
                        gp.DrawRectangle(new Pen(_GetBrush(status), Constants.PEN_WIDTH), shape.Rectangle);
                    if (elem.ToString() != "")
                    {
                        if (shape.Label != null)
                            gp.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status), shape.Label.Bounds.Rectangle);
                        else
                        {
                            SizeF size = gp.MeasureString(elem.ToString(), Constants.FONT);
                            if (elem is Lane || elem is LaneSet || elem is Participant)
                            {
                                Bitmap tbmp = new Bitmap((int)size.Height*2, (int)size.Width);
                                Graphics g = Graphics.FromImage(tbmp);
                                g.TranslateTransform(tbmp.Width / 2, tbmp.Height);
                                g.RotateTransform(-90);
                                g.TranslateTransform(0, 0);
                                g.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status),0,0);
                                g.Save();
                                gp.DrawImage(tbmp,new PointF(shape.Rectangle.X-7,shape.Rectangle.Y+((shape.Rectangle.Height-tbmp.Height)/2)));
                            }
                            else
                                gp.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status), (float)shape.Rectangle.X + (((float)shape.Rectangle.Width - size.Width) / 2), (float)shape.Rectangle.Y + (((float)shape.Rectangle.Height - size.Height) / 2));
                        }
                    }
                }
            }
            foreach (Edge edge in _Edges)
            {
                StepStatuses status = path.GetStatus(edge.bpmnElement);
                gp.DrawLines(edge.ConstructPen(_GetBrush(status), definition), edge.Points);
                if (edge.Label != null)
                {
                    IElement elem = definition.LocateElement(edge.bpmnElement);
                    if (elem != null)
                        gp.DrawString(elem.ToString(), Constants.FONT, _GetBrush(status), edge.Label.Bounds.Rectangle);
                }
            }
            return bmp;
        }

        private Brush _GetBrush(StepStatuses status)
        {
            Brush ret = Brushes.Black;
            switch (status)
            {
                case StepStatuses.Failed:
                    ret = Brushes.Red;
                    break;
                case StepStatuses.Succeeded:
                    ret = Brushes.Green;
                    break;
                case StepStatuses.Waiting:
                    ret = Brushes.Blue;
                    break;
            }
            return ret;
        }

        private string _GetImageStreamName(StepStatuses status)
        {
            string ret = "Org.Reddragonit.BpmEngine.symbols.Black.png";
            switch (status)
            {
                case StepStatuses.Failed:
                    ret = "Org.Reddragonit.BpmEngine.symbols.Red.png";
                    break;
                case StepStatuses.Succeeded:
                    ret = "Org.Reddragonit.BpmEngine.symbols.Green.png";
                    break;
                case StepStatuses.Waiting:
                    ret = "Org.Reddragonit.BpmEngine.symbols.Blue.png";
                    break;
            }
            return ret;
        }
    }
}
