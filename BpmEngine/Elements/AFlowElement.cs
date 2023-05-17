using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements
{
    [RequiredAttribute("sourceRef")]
    [RequiredAttribute("targetRef")]
    internal abstract class AFlowElement : AElement, IFlowElement
    {
        protected AFlowElement(XmlElement elem, XmlPrefixMap map, AElement parent) : 
            base(elem, map, parent)
        {
        }
        public string sourceRef => this["sourceRef"];
        public string targetRef => this["targetRef"];
    }
}
