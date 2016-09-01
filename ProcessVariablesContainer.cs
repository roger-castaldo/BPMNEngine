using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    public sealed class ProcessVariablesContainer
    {
        private List<sVariableEntry> _variables;
        private int _stepIndex;

        public ProcessVariablesContainer()
        {
            _variables = new List<sVariableEntry>();
            _stepIndex = -1;
        }

        internal ProcessVariablesContainer(string elementID, ProcessState state)
        {
            _stepIndex = state.Path.CurrentStepIndex(elementID);
            _variables = new List<sVariableEntry>();
            foreach (string str in state[elementID])
                _variables.Add(new sVariableEntry(str, _stepIndex, state[elementID]));
        }

        public object this[string name]
        {
            get
            {
                object ret = null;
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (sve.Name == name)
                        {
                            ret = sve.Value;
                            break;
                        }
                    }
                }
                return ret;
            }
            set
            {
                lock (_variables)
                {
                    for (int x = 0; x < _variables.Count; x++)
                    {
                        if (_variables[x].Name == name)
                        {
                            _variables.RemoveAt(x);
                            break;
                        }
                    }
                    _variables.Add(new sVariableEntry(name, _stepIndex, value));
                }
            }
        }

        public string[] Keys
        {
            get
            {
                List<string> ret = new List<string>();
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (!ret.Contains(sve.Name))
                            ret.Add(sve.Name);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}
