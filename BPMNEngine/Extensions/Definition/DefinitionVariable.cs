using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Extensions.Definition
{
    [XMLTagAttribute("exts", "DefinitionVariable")]
    [RequiredAttribute("Name")]
    [RequiredAttribute("Type")]
    [ValidParent(typeof(ExtensionElements))]
    internal record DefinitionVariable : AExtension
    {
        public string? Name => this["Name"];
        public object Value { get; private init; }
        public DefinitionVariable(XmlElement elem, IBaseElement? parent)
            : base(elem, parent)
        {
            string text = elem.ChildNodes.Cast<XmlNode>()
                .OfType<XmlCDataSection>().FirstOrDefault()?.InnerText??elem.InnerText;
            Value = Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), this["Type"]), text);
        }
    }
}
