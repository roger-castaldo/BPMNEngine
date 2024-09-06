using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System.Collections;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts", "containsCondition")]
    internal record ContainsCondition : ACompareCondition
    {
        public ContainsCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement)
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
            => ValueTask.FromResult<bool>((GetLeft(variables), GetRight(variables)) switch
            {
                (null, _) => false,
                (_, null) => false,
                (Array array, var right) => array.OfType<object>().Any(ol => ACompareCondition.Compare(ol, right, variables)==0),
                (IDictionary dictionary, var right) => dictionary.Keys.OfType<object>().Any(ol => ACompareCondition.Compare(ol, right, variables)==0)
                        || dictionary.Values.OfType<object>().Any(ol => ACompareCondition.Compare(ol, right, variables)==0),
                (var left, var right) => left.ToString().Contains(right.ToString())
            });
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
    }
}
