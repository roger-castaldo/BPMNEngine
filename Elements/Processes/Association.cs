using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","association")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Process))]
    internal class Association : AElement
    {
        public Association(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }
    }
}
