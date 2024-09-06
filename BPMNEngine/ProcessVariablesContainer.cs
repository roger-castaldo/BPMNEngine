using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace BPMNEngine
{
    internal sealed class ProcessVariablesContainer : IVariables
    {
        private readonly ReaderWriterLockSlim locker = new();
        private readonly List<string> nulls;
        private readonly Dictionary<string, object> variables;
        private readonly BusinessProcess process = null;
        private bool disposedValue;

        public ProcessVariablesContainer(Dictionary<string, object> props, BusinessProcess process)
        {
            nulls = new List<string>();
            variables = props??new Dictionary<string, object>();
            this.process=process;
        }

        internal ProcessVariablesContainer(string elementID, ProcessInstance processInstance)
        {
            process = processInstance.Process;
            process.WriteLogLine(elementID, LogLevel.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Producing Process Variables Container for element[{0}]", [elementID]));
            nulls = new List<string>();
            variables = new Dictionary<string, object>();
            processInstance.State[elementID].ForEach(key =>
            {
                process.WriteLogLine(elementID, LogLevel.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Adding variable {0} to Process Variables Container for element[{1}]", [key, elementID]));
                variables.Add(key, processInstance.State[elementID, key]);
            });
        }

        public object this[string name]
        {
            get
            {
                bool found = false;
                locker.EnterReadLock();
                if (variables.TryGetValue(name, out object ret))
                    found=true;
                else if (nulls.Contains(name))
                {
                    found=true;
                    ret=null;
                }
                locker.ExitReadLock();
                if (!found && process != null)
                    ret = process[name];
                return ret;
            }
            set
            {
                locker.EnterWriteLock();
                variables.Remove(name);
                if (value == null && !nulls.Contains(name))
                    nulls.Add(name);
                else if (value != null)
                {
                    variables.Add(name, value);
                    nulls.Remove(name);
                }
                locker.ExitWriteLock();
            }
        }

        public ImmutableArray<string> Keys
        {
            get
            {
                locker.EnterReadLock();
                var result = variables.Keys
                    .Concat(nulls)
                    .ToImmutableArray();
                locker.ExitReadLock();
                return result;
            }
        }

        public ImmutableArray<string> FullKeys
            => Array.Empty<string>()
            .Concat(Keys)
            .Concat(process==null ? [] : process.Keys)
            .Distinct()
            .ToImmutableArray();

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    locker.EnterWriteLock();
                    variables.Clear();
                    nulls.Clear();
                    locker.ExitWriteLock();
                    locker.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProcessVariablesContainer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
