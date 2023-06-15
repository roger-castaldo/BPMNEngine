using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements
{
    [XMLTag("exts", "DefinitionFile")]
    [Required("Name")]
    [Required("Extension")]
    [ValidParent(typeof(ExtensionElements))]
    internal class DefinitionFile : AElement
    {
        public string Name { get { return this["Name"]; } }
        public string Extension { get { return this["Extension"]; } }
        public string ContentType { get { return this["ContentType"]; } }

        private byte[] _content;
        public byte[] Content { get { return _content; } }

        public DefinitionFile(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
            _content = new byte[0];
            if (elem.ChildNodes.Count > 0)
                _content = Convert.FromBase64String((elem.ChildNodes[0] is XmlCDataSection ? ((XmlCDataSection)elem.ChildNodes[0]).InnerText : elem.InnerText));
        }
    }
}
