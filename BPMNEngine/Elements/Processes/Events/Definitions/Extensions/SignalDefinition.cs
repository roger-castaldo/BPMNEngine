using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions.Extensions
{
    [XMLTag("exts", "SignalDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    internal class SignalDefinition : AElement
    {
        public SignalDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public string Type => this["type"];

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            var errors = new List<string>();
            if (Type == "*")
                errors.Add("A Signal Definition cannot have the type of *, this is reserved");
            if (Parent.Parent.Parent is IntermediateThrowEvent && Type == null)
                errors.Add("A Signal Definition for a Throw Event must have a Type defined");
            err = (err??Array.Empty<string>()).Concat(errors);
            return res&&!errors.Any();
        }
    }
}
