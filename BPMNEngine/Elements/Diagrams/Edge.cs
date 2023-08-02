using Microsoft.Maui.Graphics;
using BPMNEngine.Attributes;
using BPMNEngine.Drawing;

using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Gateways;
using BPMNEngine.State;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNEdge")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Plane))]
    internal class Edge : ADiagramElement, IRenderingElement
    {
        private const float _PEN_SIZE = 2.0f;
        private static readonly float _baseTLength = _PEN_SIZE*1.5f;

        public IEnumerable<PointF> Points => Children
            .OfType<Waypoint>()
            .Select(elem => elem.Point);

        private RectF? _rectangle = null;
        public override RectF Rectangle
        {
            get
            {
                if (_rectangle == null)
                {
                    _rectangle=new RectF(0, 0, 0, 0);
                    Point? previous = null;
                    Points.ForEach(p =>
                    {
                        if (previous!=null)
                            _rectangle = MergeRectangle(ProduceRectangle(previous.Value, p), _rectangle);
                        previous=p;
                    });
                    Label l = Label;
                    _rectangle = new RectF(_rectangle.Value.X-3.5f, _rectangle.Value.Y-3.5f, _rectangle.Value.Width+6.5f, _rectangle.Value.Height+6.5f);
                    if (l != null)
                        _rectangle = MergeRectangle(_rectangle.Value,l.Bounds.Rectangle);
                }
                return _rectangle.Value;
            }
        }

        public Label Label 
            => Children.OfType<Label>().FirstOrDefault();
        
        public Edge(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        private static void GenerateTriangle(ICanvas surface, Color color, Point end,Point start)
        {
            float d = (float)Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
            float t = _baseTLength / d;
            Point pc = new(((1f - t) * end.X) + (t * start.X), ((1f - t) * end.Y) + (t * start.Y));
            Point fend = new(end.X, end.Y);
            Point p1 = new(pc.X-(fend.Y-pc.Y),(fend.X-pc.X)+pc.Y);
            Point p2 = new(fend.Y-pc.Y+pc.X,pc.Y-(fend.X-pc.X));
            surface.DrawLine(fend, pc);
            surface.FillColor=color;

            var path = new PathF(fend);
            path.LineTo(p1);
            path.LineTo(p2);
            path.LineTo(fend);
            path.Close();

            surface.FillPath(path);
        }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Points.Count()<2)
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "At least 2 points are required." });
                return false;
            }
            return res;
        }

        public void Render(ICanvas surface, ProcessPath path,Definition definition)
        {
            var points = Points.ToArray();
            var polyPath = new PathF(points[0]);
            points.Skip(1).ForEach(p => polyPath.LineTo(p));
            
            var color = Diagram.GetColor(path.GetStatus(BPMNElement));

            surface.StrokeColor=color;
            surface.StrokeSize=_PEN_SIZE;
            var elem = GetLinkedElement(definition);
            if (elem != null&&(elem is Association || elem is MessageFlow))
                surface.StrokeDashPattern = Constants.DASH_PATTERN;
            else
                surface.StrokeDashPattern=null;

            surface.DrawPath(polyPath);

            if (elem is MessageFlow)
            {
                surface.FillColor = color;
                surface.FillEllipse(new RectF(Points.First().X-0.5f,Points.First().Y-0.5f,1.5f, 1.5f));
                Edge.GenerateTriangle(surface, color, points[^1], points[^2]);
            }
            else
                Edge.GenerateTriangle(surface, color, points[^1], points[^2]);
            if (elem is SequenceFlow || elem is MessageFlow)
            {
                var sourceRef = (elem is SequenceFlow flow ? flow.SourceRef : ((MessageFlow)elem).SourceRef);
                var gelem = definition.LocateElement(sourceRef);
                if ((gelem is AGateway gateway) && (gateway.Default??"")==elem.ID)
                {
                    var centre = new Point(
                        (0.5f*points[0].X)+(0.5f*points[1].X),
                        (0.5f * points[0].Y) + (0.5f * points[1].Y)
                    );
                    surface.DrawLine(new Point(centre.X-3f, centre.Y-3f), new Point(centre.X+3f, centre.Y+3f));
                }
            }

            if (Label!=null)
            {
                surface.FontColor = color;
                surface.DrawString(elem.ToString(), Label.Bounds.Rectangle, HorizontalAlignment.Center, VerticalAlignment.Center);
            }
        }
    }
}
