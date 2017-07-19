using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    public class ReadOnlyProcessVariablesContainer
    {
        private ProcessVariablesContainer _variables;
        private Exception _error;

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessState state, BusinessProcess process)
            : this(elementID, state, process, null) { }

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessState state, BusinessProcess process,Exception error)
        {
            _variables = new BpmEngine.ProcessVariablesContainer(elementID, state, process);
            _error = error;
        }

        internal ReadOnlyProcessVariablesContainer(ProcessVariablesContainer variables)
        {
            _variables = variables;
        }

        public object this[string name] { get { return _variables[name]; } }

        public string[] Keys { get { return _variables.Keys; } }

        public Exception Error { get { return _error; } }
    }
}
