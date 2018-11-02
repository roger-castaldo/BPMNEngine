using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    /// <summary>
    /// This implements a container to house the process variables and allows for editing of those variables.
    /// </summary>
    public sealed class ProcessVariablesContainer
    {
        private List<string> _nulls;
        private Dictionary<string, object> _variables;
        private int _stepIndex;

        private BusinessProcess _process = null;
        internal void SetProcess(BusinessProcess process) { _process = process; }

        /// <summary>
        /// Creates a new empty instance of container to house variables
        /// </summary>
        public ProcessVariablesContainer()
        {
            _nulls = new List<string>();
            _variables = new Dictionary<string, object>();
            _stepIndex = -1;
        }

        internal ProcessVariablesContainer(string elementID, ProcessState state,BusinessProcess process)
        {
            Log.Debug("Producing Process Variables Container for element[{0}]", new object[] { elementID });
            _stepIndex = state.Path.CurrentStepIndex(elementID);
            _nulls = new List<string>();
            _variables = new Dictionary<string, object>();
            _process = process;
            foreach (string str in state[elementID])
            {
                Log.Debug("Adding variable {0} to Process Variables Container for element[{1}]", new object[] { str,elementID });
                _variables.Add(str,state[elementID, str]);
            }
        }

        /// <summary>
        /// Called to get or set the value of a process variable
        /// </summary>
        /// <param name="name">The name of the process variable</param>
        /// <returns>The value of the process variable or null if not found</returns>
        public object this[string name]
        {
            get
            {
                object ret = null;
                bool found = false;
                lock (_variables)
                {
                    if (_variables.ContainsKey(name))
                    {
                        found = true;
                        ret = _variables[name];
                    }else if (_nulls.Contains(name))
                    {
                        found = true;
                        ret = null;
                    }
                }
                if (!found && _process != null)
                    ret = _process[name];
                else if (!found && BusinessProcess.Current != null)
                    ret = BusinessProcess.Current[name];
                return ret;
            }
            set
            {
                lock (_variables)
                {
                    _variables.Remove(name);
                    if (value == null && !_nulls.Contains(name))
                        _nulls.Add(name);
                    else if (value != null)
                    {
                        _variables.Add(name, value);
                        _nulls.Remove(name);
                    }
                }
            }
        }

        /// <summary>
        /// Called to get a list of all process variable names available
        /// </summary>
        public string[] Keys
        {
            get
            {
                List<string> ret = new List<string>();
                lock (_variables)
                {
                    ret.AddRange(_variables.Keys);
                    ret.AddRange(_nulls);
                }
                return ret.ToArray();
            }
        }

        /// <summary>
        /// Called to get a list of all process variable names available, including process definition constants and runtime constants
        /// </summary>
        public string[] FullKeys
        {
            get
            {
                List<string> ret = new List<string>(Keys);
                if (_process!=null)
                {
                    foreach (string key in _process.Keys)
                    {
                        if (!ret.Contains(key))
                            ret.Add(key);
                    }
                }else if (BusinessProcess.Current != null)
                {
                    foreach (string key in BusinessProcess.Current.Keys)
                    {
                        if (!ret.Contains(key))
                            ret.Add(key);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}
