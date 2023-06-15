using BPMNEngine.Elements.Processes.Scripts;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal class ScriptCondition : ACondition
    {
        private AScript _script;

        public ScriptCondition(AScript script)
            : base(script.Element, null,null) {
                _script = script;
        }

        public override bool Evaluate(IReadonlyVariables variables)
        {
            return (bool)_script.Invoke(variables);
        }
    }
}
