using BPMNEngine.Extensions.Conditions;
using BPMNEngine.Extensions.Scripts;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Condition.Conditions
{
    internal record ScriptCondition : ACondition
    {
        private readonly AScript script;
        public ScriptCondition(AScript script, IBaseElement? parent, IExtensionElementFactory elementFactory)
            : base(script.Element, parent, elementFactory)
        {
            this.script = script;
        }

        public override ValueTask<bool> IsElementStartValidAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
            => ValueTask.FromResult((bool)script.Invoke(variables, logger));
        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
            => script.IsValid(logger);
    }
}
