using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts", "ConditionSet")]
    [ValidParent(typeof(ExtensionElements))]
    internal record ConditionSet : AConditionSet
    {
        public ConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool Evaluate(IReadonlyVariables variables)
        {
            try
            {
                return Conditions.First().Evaluate(variables);
            }catch(Exception ex)
            {
                Exception(ex);
                return false;
            }
        }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Children.Length > 1)
            {
                err = (err?? []).Append("Too many children found.");
                return false;
            }
            return res;
        }
    }
}
