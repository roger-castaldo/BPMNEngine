using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("exts", "DefinitionFile")]
    [RequiredAttribute("Name")]
    [RequiredAttribute("Extension")]
    [ValidParent(typeof(ExtensionElements))]
    internal record DefinitionFile : AElement
    {
        public string Name =>this["Name"];
        public string Extension =>this["Extension"]; 
        public string ContentType => this["ContentType"];
        public byte[] Content { get; private init; }

        public DefinitionFile(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent)
        {
            Content = [];
            if (elem.ChildNodes.Count > 0)
                Content = Convert.FromBase64String((elem.ChildNodes[0] is XmlCDataSection section ? section.InnerText : elem.InnerText));
        }
    }
}
