using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNPlane")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Diagram))]
    internal class Plane : ADiagramElement
    {
        public Plane(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out string[] err)
        {
            if (this.Children.Length == 0)
            {
                err = new string[] { "No child elements to render." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
