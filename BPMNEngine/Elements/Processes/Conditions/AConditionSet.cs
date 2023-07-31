using BPMNEngine.Elements.Processes.Scripts;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract class AConditionSet : ACondition
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

        public override bool IsValid(out string[] err)
        {
            if (!Children.Any())
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
