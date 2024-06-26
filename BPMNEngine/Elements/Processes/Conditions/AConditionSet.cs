using BPMNEngine.Elements.Processes.Scripts;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract record AConditionSet : ACondition
    {
        protected AConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected IEnumerable<ACondition> Conditions 
            => Children
            .OfType<ACondition>().Concat(
                Children
                .OfType<AScript>()
                .Select(asc => new ScriptCondition(asc))
            );

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err = (err?? []).Append("No child elements found within a condition set.");
                return false;
            }
            return res;
        }
    }
}
