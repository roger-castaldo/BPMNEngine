using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    internal abstract class ATask : AFlowNode
    {
        public ATask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
