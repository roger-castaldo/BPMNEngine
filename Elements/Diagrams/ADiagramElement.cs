using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    internal abstract class ADiagramElement : AParentElement
    {
        public string bpmnElement { get { return _GetAttributeValue("bpmnElement"); } }

        protected IElement _GetLinkedElement(Definition definition)
        {
            return definition.LocateElement(bpmnElement);
        }

        public ADiagramElement(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }
    }
}
