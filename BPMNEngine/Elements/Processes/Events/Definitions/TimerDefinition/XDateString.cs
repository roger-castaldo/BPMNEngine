using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions.TimerDefinition
{
    [XMLTagAttribute("exts", "DateString")]
    [ValidParent(typeof(TimerEventDefinition))]
    [ValidParent(typeof(ExtensionElements))]
    internal record XDateString : AElement
    {
        private string Code
            => this["Code"] ??
            SubNodes.Where(n => n.NodeType==XmlNodeType.Text).Select(n => n.Value).FirstOrDefault() ??
            SubNodes.Where(n => n.NodeType==XmlNodeType.CDATA).Select(n => ((XmlCDataSection)n).Value).FirstOrDefault() ??
            string.Empty;

        private readonly DateString DateCode;

        public XDateString(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
            DateCode = (string.IsNullOrEmpty(Code) ? null : new(Code));
        }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (string.IsNullOrEmpty(Code))
            {
                err=(err?? []).Append("No Date String Specified");
                return false;
            }
            return res;
        }

        public DateTime GetTime(IReadonlyVariables variables)
            => DateCode.GetTime(variables);
    }
}
