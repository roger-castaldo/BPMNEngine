using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNEdge")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Plane))]
    internal class Edge : ADiagramElement
    {

        private CustomLineCap _defaultFlowCap
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddLine(new PointF(1.5f,-3.5f),new PointF(1.5f,-1.5f));
                return new CustomLineCap(null, gp);
            }
        }

        public Point[] Points
        {
            get
            {
                List<Point> ret = new List<Point>();
                foreach (IElement elem in Children)
                {
                    if (elem is Waypoint)
                        ret.Add(((Waypoint)elem).Point);
                }
                return ret.ToArray();
            }
        }

        private RectangleF? _rectangle = null;
        public RectangleF Rectangle{
            get
            {
                if (_rectangle == null)
                {
                    int minX = int.MaxValue;
                    int maxX = int.MinValue;
                    int minY = int.MaxValue;
                    int maxY = int.MinValue;
                    foreach (Point p in Points)
                    {
                        minX = Math.Min(minX, p.X);
                        minY = Math.Min(minY, p.Y);
                        maxX = Math.Max(maxX, p.X);
                        maxY = Math.Max(maxY, p.Y);
                    }
                    Label l = Label;
                    if (l != null)
                    {
                        minX = Math.Min(minX, (int)Math.Floor(l.Bounds.Rectangle.X));
                        minY = Math.Min(minY, (int)Math.Floor(l.Bounds.Rectangle.Y));
                        maxX = Math.Max(maxX, (int)Math.Floor(l.Bounds.Rectangle.X+l.Bounds.Rectangle.Width));
                        maxY = Math.Max(maxY, (int)Math.Floor(l.Bounds.Rectangle.Y+l.Bounds.Rectangle.Height));
                    }
                    _rectangle = new RectangleF(minX-3, minY-3, maxX - minX+6,  maxY - minY+6);
                }
                return _rectangle.Value;
            }
        }

        public Label Label
        {
            get
            {
                foreach (IElement elem in Children)
                {
                    if (elem is Label)
                        return (Label)elem;
                }
                return null;
            }
        }

        public Edge(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public Pen ConstructPen(Brush brush, Definition definition)
        {
            Pen ret = new Pen(brush, Constants.PEN_WIDTH);
            IElement elem = _GetLinkedElement(definition);
            if (elem != null)
            {
                if (elem is Association || elem is MessageFlow)
                    ret.DashPattern = Constants.DASH_PATTERN;
            }
            return ret;
        }

        internal void AppendEnds(Graphics gp, Brush brush, Definition definition)
        {
            Pen p = ConstructPen(brush, definition);
            IElement elem = _GetLinkedElement(definition);
            if (elem != null)
            {
                Point[] points = Points;
                if (elem is MessageFlow)
                {
                    gp.FillEllipse(brush, new RectangleF(new PointF((float)points[0].X - 0.5f, (float)points[0].Y - 0.5f), new SizeF(1.5f, 1.5f)));
                    _GenerateTriangle(gp, brush, points[points.Length - 1],points[points.Length-2]);
                }
                else
                    _GenerateTriangle(gp, brush, points[points.Length - 1], points[points.Length - 2]);
                if (elem is SequenceFlow || elem is MessageFlow)
                {
                    string sourceRef = (elem is SequenceFlow ? ((SequenceFlow)elem).sourceRef : ((MessageFlow)elem).sourceRef);
                    IElement gelem = definition.LocateElement(sourceRef);
                    if (gelem != null)
                    {
                        if (gelem is AGateway)
                        {
                            if ((((AGateway)gelem).Default == null ? "" : ((AGateway)gelem).Default) == elem.id)
                            {
                                PointF centre = new PointF(
                                    ((0.5f*(float)points[0].X)+(0.5f*(float)points[1].X)),
                                    ((0.5f * (float)points[0].Y) + (0.5f * (float)points[1].Y))
                                );
                                gp.DrawLine(p,new PointF(centre.X-3f,centre.Y-3f),new PointF(centre.X+3f,centre.Y+3f));
                            }
                        }
                    }
                }
            }
        }

        private static readonly float _baseTLength = Constants.PEN_WIDTH*1.5f;

        private void _GenerateTriangle(Graphics gp, Brush brush, Point end,Point start)
        {
            float d = (float)Math.Sqrt(Math.Pow((double)end.X - (double)start.X, 2) + Math.Pow((double)end.Y - (double)start.Y, 2));
            float t = _baseTLength / d;
            PointF pc = new PointF(((1f - t) * (float)end.X) + (t * (float)start.X), ((1f - t) * (float)end.Y) + (t * (float)start.Y));
            PointF fend = new PointF((float)end.X, (float)end.Y);
            PointF p1 = new PointF(pc.X-(fend.Y-pc.Y),(fend.X-pc.X)+pc.Y);
            PointF p2 = new PointF(fend.Y-pc.Y+pc.X,pc.Y-(fend.X-pc.X));
            t = _baseTLength / d;
            gp.DrawLine(new Pen(Brushes.White,Constants.PEN_WIDTH), fend, pc);
            gp.FillPolygon(brush, new PointF[] {
                fend,
                p1,
                p2
            });
        }

        public override bool IsValid(out string[] err)
        {
            if (Points.Length<2)
            {
                err = new string[] { "At least 2 points are required." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
