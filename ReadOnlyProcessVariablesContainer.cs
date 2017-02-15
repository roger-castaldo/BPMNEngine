using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    public class ReadOnlyProcessVariablesContainer
    {
        private ProcessVariablesContainer _variables;

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessState state)
        {
            _variables = new BpmEngine.ProcessVariablesContainer(elementID,state);
        }

        internal ReadOnlyProcessVariablesContainer(ProcessVariablesContainer variables)
        {
            _variables = variables;
        }

        public object this[string name] { get { return _variables[name]; } }

        public string[] Keys { get { return _variables.Keys; } }
    }
}
