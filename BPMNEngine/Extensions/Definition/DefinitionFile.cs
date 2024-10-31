using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Extensions.Definition
{
    [XMLTagAttribute("exts", "DefinitionFile")]
    [RequiredAttribute("Name")]
    [RequiredAttribute("Extension")]
    [ValidParent(typeof(ExtensionElements))]
    internal record DefinitionFile : AExtension
    {
        public string? Name => this["Name"];
        public string? Extension => this["Extension"];
        public string? ContentType => this["ContentType"];
        public byte[] Content { get; private init; }

        public DefinitionFile(XmlElement elem, IBaseElement? parent)
            : base(elem, parent)
        {
            Content = [];
            if (elem.ChildNodes.Count > 0)
                Content = Convert.FromBase64String((elem.ChildNodes[0] is XmlCDataSection section ? section.InnerText : elem.InnerText));
        }
    }
}
