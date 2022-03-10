using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    /// <summary>
    /// This class implements a Read Only version of the process variables container.  These are using in event delegates as the process variables
    /// cannot be changed by events.
    /// </summary>
    public class ReadOnlyProcessVariablesContainer : IReadonlyVariables
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

        /// <summary>
        /// Called to get the value of a process variable
        /// </summary>
        /// <param name="name">The name of the process variable</param>
        /// <returns>The value of the variable or null if not found</returns>
        public object this[string name] { get { return _variables[name]; } set { throw new Exception("Unable to change variable values in readonly process variables container."); } }

        /// <summary>
        /// Called to get a list of all process variable names available
        /// </summary>
        public string[] Keys { get { return _variables.Keys; } }

        /// <summary>
        /// Called to get a list of all process variable names available, including process definition constants and runtime constants
        /// </summary>
        public string[] FullKeys { get { return _variables.FullKeys; } }

        /// <summary>
        /// The error that occured, assuming this was passed to an error event delgate this will have a value
        /// </summary>
        public Exception Error { get { return _error; } }

        public void WriteLogLine(LogLevels level, string message)
        {
            ((ProcessVariablesContainer)_variables).WriteLogLine(level, new StackFrame(2, true), DateTime.Now, message);
        }
    }
}
