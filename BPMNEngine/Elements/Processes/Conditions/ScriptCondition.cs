using BPMNEngine.Elements.Processes.Scripts;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal class ScriptCondition : ACondition
    {
        private readonly AScript _script;

        public ScriptCondition(AScript script)
            : base(script.Element, null,null) {
                _script = script;
        }

        public override bool Evaluate(IReadonlyVariables variables)
            => (bool)_script.Invoke(variables);
    }
}
