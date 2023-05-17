using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","association")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(IProcess))]
    internal class Association : AFlowElement
    {
        public Association(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }
    }
}
