using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    internal class ReadOnlyProcessVariablesContainer : IReadonlyVariables
    {
        private IVariables _variables;
        private Exception _error;

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessState state, BusinessProcess process)
            : this(elementID, state, process, null) { }

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessState state, BusinessProcess process,Exception error)
        {
            _variables = new ProcessVariablesContainer(elementID, state, process);
            _error = error;
        }

        internal ReadOnlyProcessVariablesContainer(IVariables variables)
        {
            _variables = variables;
        }

        public object this[string name] { get { return _variables[name]; } set { throw new Exception("Unable to change variable values in readonly process variables container."); } }

        public string[] Keys { get { return _variables.Keys; } }

        public string[] FullKeys { get { return _variables.FullKeys; } }

        public Exception Error { get { return _error; } }

        public void WriteLogLine(LogLevels level, string message)
        {
            ((ProcessVariablesContainer)_variables).WriteLogLine(level, new StackFrame(2, true), DateTime.Now, message);
        }
    }
}
