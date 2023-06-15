using BPMNEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTag("bpmn", "participant")]
    [RequiredAttribute("processRef")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal class Participant : AElement
    {
        public Participant(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }
    }
}
