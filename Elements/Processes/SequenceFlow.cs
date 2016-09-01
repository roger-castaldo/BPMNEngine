using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","sequenceFlow")]
    internal class SequenceFlow : AElement
    {
        public string name { get { return _GetAttributeValue("name"); } }
        public string sourceRef { get { return _GetAttributeValue("sourceRef"); } }
        public string targetRef { get { return _GetAttributeValue("targetRef"); } }

        public SequenceFlow(XmlElement elem)
            : base(elem) { }
    }
}
