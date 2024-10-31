using BPMNEngine.Attributes;
using BPMNEngine.Extensions.Conditions;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Condition.Conditions
{
    [XMLTag("exts", "lessThanCondition")]
    internal record LessThanCondition : ACompareCondition
    {
        public LessThanCondition(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory)
            : base(xmlElement, parent, elementFactory) { }

        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
            => ValueTask.FromResult(Compare(variables) < 0);
    }
}
