using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn","lane")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(LaneSet))]
    internal class Lane : AParentElement
    {
        public IEnumerable<string> Nodes => Children
            .OfType<FlowNodeRef>()
            .Select(elem => elem.Value);

        public Lane(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map,parent) { }
    }
}
