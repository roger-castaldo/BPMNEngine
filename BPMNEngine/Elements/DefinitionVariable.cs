using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;

namespace BPMNEngine.Elements
{
    [XMLTag("exts", "DefinitionVariable")]
    [Required("Name")]
    [Required("Type")]
    [ValidParent(typeof(ExtensionElements))]
    internal class DefinitionVariable : AElement
    {
        public string Name => this["Name"];
        public object Value { get; private init; }

        public DefinitionVariable(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
            string text = elem.InnerText;
            if (elem.ChildNodes[0].NodeType == XmlNodeType.CDATA)
                text = ((XmlCDataSection)elem.ChildNodes[0]).InnerText;
            Value = Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), this["Type"]), text);
        }
    }
}
