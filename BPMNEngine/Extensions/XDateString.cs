using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions
{
    [XMLTagAttribute("exts", "DateString")]
    [ValidParent(typeof(TimerEventDefinition))]
    [ValidParent(typeof(ExtensionElements))]
    internal record XDateString(XmlElement Element, IBaseElement? Parent) : AExtension(Element,Parent)
    {
        public string Code { get; private init; } = Element.Attributes.Cast<XmlAttribute>()
                    .Where(att => string.Equals(att.Name, "Code", StringComparison.InvariantCultureIgnoreCase))
                    .Select(att => att.Value)
                    .FirstOrDefault() ??
            Element.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType==XmlNodeType.Text).Select(n => n.Value).FirstOrDefault() ??
            Element.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType==XmlNodeType.CDATA).Select(n => ((XmlCDataSection)n).Value).FirstOrDefault() ??
            string.Empty;

        private DateString? dateCode;

        public DateTime? GetTime(IReadonlyVariables variables)
            => (dateCode??=(string.IsNullOrWhiteSpace(Code) ? null : new(Code)))?.GetTime(variables);

        public (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
            => (string.IsNullOrEmpty(Code) ? (false, ["No Date String Specified"]) : (true, []));
    }
}
