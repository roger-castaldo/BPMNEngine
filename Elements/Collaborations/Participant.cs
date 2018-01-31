using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Collaborations
{
    [XMLTag("bpmn", "participant")]
    [RequiredAttribute("processRef")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    internal class Participant : AElement
    {
        public string Name { get { return this["name"]; } }
        public string ProcessRef { get { return this["processRef"]; } }

        public Participant(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }
    }
}
