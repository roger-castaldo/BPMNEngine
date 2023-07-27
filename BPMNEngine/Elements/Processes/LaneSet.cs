﻿using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn","laneSet")]
    [ValidParent(typeof(IProcess))]
    internal class LaneSet : AParentElement
    {
        public LaneSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}