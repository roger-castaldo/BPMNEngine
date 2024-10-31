using BPMNEngine.Attributes;
using BPMNEngine.Extensions.Conditions;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Condition.Conditions
{
    [XMLTag("exts", "isNull")]
    internal record IsNull : ANegatableCondition
    {
        public IsNull(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory)
            : base(xmlElement, parent, elementFactory) { }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
            => (!string.IsNullOrWhiteSpace(this["variable"]), string.IsNullOrWhiteSpace(this["variable"]) ? ["You must supply a variable name to check for IsNull"] : []);
        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
            => ValueTask.FromResult(variables[this["variable"]!] == null);
    }
}
