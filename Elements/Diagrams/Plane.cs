using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNPlane")]
    internal class Plane : ADiagramElement
    {
        public Plane(XmlElement elem)
            : base(elem) { }
    }
}
