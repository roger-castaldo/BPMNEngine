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
        public byte[] Content { get; private init; }

        public DefinitionFile(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
            Content = Array.Empty<byte>();
            if (elem.ChildNodes.Count > 0)
                Content = Convert.FromBase64String((elem.ChildNodes[0] is XmlCDataSection section ? section.InnerText : elem.InnerText));
        }
    }
}
