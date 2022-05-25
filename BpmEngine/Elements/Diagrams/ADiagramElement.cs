using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [RequiredAttribute("bpmnElement")]
    internal abstract class ADiagramElement : AParentElement
    {
        public string bpmnElement { get { return this["bpmnElement"]; } }

        protected IElement _GetLinkedElement(Definition definition)
        {
            return definition.LocateElement(bpmnElement);
        }

        public ADiagramElement(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
