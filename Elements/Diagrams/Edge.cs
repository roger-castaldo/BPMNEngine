using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNEdge")]
    [RequiredAttribute("id")]
    internal class Edge : ADiagramElement
    {
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

        public Edge(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

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
