using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Collaborations
{
    [XMLTag("bpmn","messageFlow")]
    [RequiredAttribute("sourceRef")]
    [RequiredAttribute("targetRef")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal class MessageFlow : AElement
    {
        public string sourceRef => this["sourceRef"];
        public string targetRef => this["targetRef"];

        public MessageFlow(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }
    }
}
