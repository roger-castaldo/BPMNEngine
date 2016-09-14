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
    internal class Participant : AElement
    {
        public string Name { get { return _GetAttributeValue("name"); } }
        public string ProcessRef { get { return _GetAttributeValue("processRef"); } }

        public Participant(XmlElement elem, XmlPrefixMap map)
            : base(elem, map)
        {}
    }
}
