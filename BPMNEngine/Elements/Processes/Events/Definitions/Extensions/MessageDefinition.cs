using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions.Extensions
{
    [XMLTag("exts", "MessageDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(MessageEventDefinition))]
    internal class MessageDefinition : AElement
    {
        public MessageDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public string Name => this["name"];

        public override bool IsValid(out string[] err)
        {
            var errors = new List<string>();
            if (Name == "*")
                errors.Add("A Message Definition cannot have the name of *, this is reserved");
            if (Parent.Parent is IntermediateThrowEvent && Name == null)
                errors.Add("A Message Definition for a Throw Event must have a Name defined");
            bool isError = base.IsValid(out err);
            isError |= errors.Count > 0;
            if (err != null)
                errors.AddRange(err);
            err = errors.ToArray();
            return isError;
        }
    }
}
