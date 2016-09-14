using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","association")]
    [RequiredAttribute("id")]
    internal class Association : AElement
    {
        public Association(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }
    }
}
