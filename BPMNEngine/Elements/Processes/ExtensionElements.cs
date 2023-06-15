using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes
{
    [XMLTag("bpmn", "extensionElements")]
    [ValidParent(null)]
    internal class ExtensionElements : AParentElement
    {
        public ExtensionElements(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

    }
}
