using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes
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
