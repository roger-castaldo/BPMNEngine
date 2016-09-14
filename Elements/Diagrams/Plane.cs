using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNPlane")]
    [RequiredAttribute("id")]
    internal class Plane : ADiagramElement
    {
        public Plane(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override bool IsValid(out string err)
        {
            if (this.Children.Length == 0)
            {
                err = "No child elements to render.";
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
