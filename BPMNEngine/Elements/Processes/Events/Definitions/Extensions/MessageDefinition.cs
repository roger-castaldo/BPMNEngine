using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions.Extensions
{
    [XMLTag("exts", "MessageDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    internal class MessageDefinition : AElement
    {
        public MessageDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public string Name => this["name"];

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            var errors = new List<string>();
            if (Name == "*")
                errors.Add("A Message Definition cannot have the name of *, this is reserved");
            if (Parent.Parent.Parent is IntermediateThrowEvent && Name == null)
                errors.Add("A Message Definition for a Throw Event must have a Name defined");
            err = (err??Array.Empty<string>()).Concat(errors);
            return res&&!errors.Any();
        }
    }
}
