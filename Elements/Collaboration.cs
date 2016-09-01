using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn","collaboration")]
    internal class Collaboration : AParentElement
    {
        public Collaboration(XmlElement elem)
            : base(elem) { }
    }
}
