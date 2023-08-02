using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "ConditionSet")]
    [ValidParent(typeof(ExtensionElements))]
    internal class ConditionSet : AConditionSet
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
                Error(ex.Message);
                return false;
            }
        }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Children.Count() > 1)
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "Too many children found." });
                return false;
            }
            return res;
        }
    }
}
