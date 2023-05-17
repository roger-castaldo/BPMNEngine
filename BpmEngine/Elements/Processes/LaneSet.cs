using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","laneSet")]
    [ValidParent(typeof(IProcess))]
    internal class LaneSet : AParentElement
    {
        public LaneSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
