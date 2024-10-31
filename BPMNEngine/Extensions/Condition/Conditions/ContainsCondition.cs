using BPMNEngine.Attributes;
using BPMNEngine.Extensions.Conditions;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System.Collections;

namespace BPMNEngine.Extensions.Condition.Conditions
{
    [XMLTag("exts", "containsCondition")]
    internal record ContainsCondition : ACompareCondition
    {
        public ContainsCondition(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory)
            : base(xmlElement, parent, elementFactory) { }

        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
            => ValueTask.FromResult((GetLeft(variables), GetRight(variables)) switch
            {
                (null, _) => false,
                (_, null) => false,
                (Array array, var right) => array.OfType<object>().Any(ol => Compare(ol, right, variables) == 0),
                (IDictionary dictionary, var right) => dictionary.Keys.OfType<object>().Any(ol => Compare(ol, right, variables) == 0)
                        || dictionary.Values.OfType<object>().Any(ol => Compare(ol, right, variables) == 0),
                (var left, var right) => left.ToString().Contains(right.ToString())
            });
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
    }
}
