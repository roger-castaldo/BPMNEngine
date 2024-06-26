using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(AConditionSet))]
    internal abstract record ACondition : AParentElement
    {
        public abstract bool Evaluate(IReadonlyVariables variables);

        protected ACondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
