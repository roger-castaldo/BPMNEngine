using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine
{
    internal sealed class ProcessVariablesContainer : IVariables
    {
        private readonly List<string> nulls;
        private readonly Dictionary<string, object> variables;
        private readonly BusinessProcess process = null;

        public ProcessVariablesContainer(Dictionary<string,object> props,BusinessProcess process)
        {
            nulls = new List<string>();
            variables = props??new Dictionary<string, object>();
            this.process=process;
        }

        internal ProcessVariablesContainer(string elementID, ProcessInstance processInstance)
        {
            process = processInstance.Process;
            process.WriteLogLine(elementID,LogLevel.Debug,new System.Diagnostics.StackFrame(1,true),DateTime.Now,string.Format("Producing Process Variables Container for element[{0}]", new object[] { elementID }));
            nulls = new List<string>();
            variables = new Dictionary<string, object>();
            processInstance.State[elementID].ForEach(key =>
            {
                process.WriteLogLine(elementID, LogLevel.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Adding variable {0} to Process Variables Container for element[{1}]", new object[] { key, elementID }));
                variables.Add(key, processInstance.State[elementID, key]);
            });
        }

        public object this[string name]
        {
            get
            {
                object ret = null;
                bool found = false;
                lock (variables)
                {
                    if (variables.ContainsKey(name))
                    {
                        found = true;
                        ret = variables[name];
                    }else if (nulls.Contains(name))
                    {
                        found = true;
                        ret = null;
                    }
                }
                if (!found && process != null)
                    ret = process[name];
                return ret;
            }
            set
            {
                lock (variables)
                {
                    variables.Remove(name);
                    if (value == null && !nulls.Contains(name))
                        nulls.Add(name);
                    else if (value != null)
                    {
                        variables.Add(name, value);
                        nulls.Remove(name);
                    }
                }
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                IEnumerable<string> result;
                lock (variables)
                {
                    result = variables.Keys
                        .Concat(nulls);
                }
                return result;
            }
        }

        public IEnumerable<string> FullKeys
            => Keys.Concat(process==null ? Array.Empty<string>() : process.Keys)
            .Distinct();
    }
}
