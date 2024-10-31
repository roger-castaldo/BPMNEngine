using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Elements.Processes.Events;

namespace BPMNEngine.Extensions.EventDefinitions
{
    [XMLTagAttribute("exts", "MessageDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    internal record MessageDefinition : AExtension
    {
        public MessageDefinition(XmlElement elem, IBaseElement? parent)
            : base(elem, parent) { }

        public string? Name => this["name"];

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            var errs = new List<string>();
            if (Name == "*")
                errs.Add("A Message Definition cannot have the name of *, this is reserved");
            if (Parent?.Parent?.Parent is IntermediateThrowEvent && Name == null)
                errs.Add("A Message Definition for a Throw Event must have a Name defined");
            return (isValid&&errs.Count==0, errors.Concat(errs));
        }
    }
}
