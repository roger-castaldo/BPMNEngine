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

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessInstance instance)
            : this(elementID, instance,null) { }

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessInstance instance,Exception error)
        {
            _variables = new ProcessVariablesContainer(elementID, instance);
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
    }
}
