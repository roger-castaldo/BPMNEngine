using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Extensions.EventDefinitions
{
    [XMLTagAttribute("exts", "ErrorDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    internal record ErrorDefinition : AExtension
    {
        public ErrorDefinition(XmlElement elem, IBaseElement? parent)
            : base(elem, parent) { }

        public string? Type => this["type"];

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            var errs = new List<string>();
            if (Type == "*")
                errs.Add("An Error Definition cannot have the type of *, this is reserved");
            if (Parent?.Parent?.Parent is IntermediateThrowEvent && Type==null)
                errs.Add("An Error Definition for a Throw Event must have a Type defined");
            return (isValid&&errs.Count==0, errors.Concat(errs));
        }
    }
}
