using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn", "extensionElements")]
    internal class ExtensionElements : AElement
    {
        public ExtensionElements(XmlElement elem)
            : base(elem) { }
    }
}
