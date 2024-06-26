using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Events.Definitions.Extensions
{
    [XMLTagAttribute("exts", "ErrorDefinition")]
    [ValidParent(typeof(ExtensionElements))]
    internal record ErrorDefinition : AElement
    {
        public ErrorDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public string Type => this["type"];
        
        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            var errors = new List<string>();
            if (Type == "*")
                errors.Add("An Error Definition cannot have the type of *, this is reserved");
            if (Parent.Parent.Parent is IntermediateThrowEvent && Type==null)
                errors.Add("An Error Definition for a Throw Event must have a Type defined");
            err = (err?? []).Concat(errors);
            return res && errors.Count==0;
        }
    }
}
