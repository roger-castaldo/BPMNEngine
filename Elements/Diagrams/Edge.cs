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
    internal class Edge : ADiagramElement
    {

        private CustomLineCap _defaultFlowCap
        {
            get
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddLine(new PointF(-1.5f,-3.5f),new PointF(1.5f,-1.5f));
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
                if (elem is Association)
                    ret.DashPattern = Constants.DASH_PATTERN;
                else if (elem is MessageFlow)
                {
                    ret.DashPattern = Constants.DASH_PATTERN;
                    ret.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                    ret.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                }
                else
                    ret.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                if (elem is SequenceFlow || elem is MessageFlow)
                {
                    string sourceRef = (elem is SequenceFlow ? ((SequenceFlow)elem).sourceRef : ((MessageFlow)elem).sourceRef);
                    IElement gelem = definition.LocateElement(sourceRef);
                    if (gelem != null)
                    {
                        if (gelem is AGateway)
                        {
                            if ((((AGateway)gelem).Default == null ? "" : ((AGateway)gelem).Default) == elem.id)
                                ret.CustomStartCap = _defaultFlowCap;
                        }
                    }
                }
            }
            return ret;
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
