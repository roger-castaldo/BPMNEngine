using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;

namespace BPMNEngine.Tasks
{
    internal record ExternalTask : ITask
    {
        private readonly ATask task;
        protected readonly ProcessInstance businessProcess;
        public bool Aborted { get; private set; }
        public IVariables Variables { get; private init; }

        public ExternalTask(ATask task, ProcessVariablesContainer variables, ProcessInstance process)
        {
            this.task=task;
            Variables=variables;
            businessProcess=process;
        }

        #region IElement
        string IElement.this[string attributeName]
            => task[attributeName];

        public IElement Process
            => task.Process;

        public IElement SubProcess
            => task.SubProcess;

        public IElement Lane
            => task.Lane;

        public string ID
            => task.ID;

        public ImmutableArray<XmlNode> SubNodes
            => task.SubNodes;

        public IParentElement ExtensionElement
            => task.ExtensionElement;

        private void WriteLogLine(LogLevel level, string message)
            => businessProcess.WriteLogLine(task, level, new StackFrame(2, true), DateTime.Now, message);

        public void Debug(string message)
            => WriteLogLine(LogLevel.Debug, message);

        public void Debug(string message, object[] pars)
            => WriteLogLine(LogLevel.Debug, string.Format(message, pars));

        public void Error(string message)
            => WriteLogLine(LogLevel.Error, message);

        public void Error(string message, object[] pars)
            => WriteLogLine(LogLevel.Error, string.Format(message, pars));

        public Exception Exception(Exception exception)
            => businessProcess.WriteLogException(task, new StackFrame(1), DateTime.Now, exception);

        public void Fatal(string message)
            => WriteLogLine(LogLevel.Critical, message);

        public void Fatal(string message, object[] pars)
            => WriteLogLine(LogLevel.Critical, string.Format(message, pars));

        public void Info(string message)
            => WriteLogLine(LogLevel.Information, message);

        public void Info(string message, object[] pars)
            => WriteLogLine(LogLevel.Information, string.Format(message, pars));
        #endregion

        private async ValueTask<bool> ProcessEvent(Func<ValueTask<bool>> invocation)
        {
            var isAborted = await invocation();
            Aborted=Aborted||isAborted;
            return isAborted;
        }

        public ValueTask<bool> EmitErrorAsync(Exception error)
            => ProcessEvent(() => businessProcess.EmitTaskErrorAsync(this, error));

        public ValueTask<bool> EmitMessageAsync(string message)
            => ProcessEvent(() => businessProcess.EmitTaskMessageAsync(this, message));


        public ValueTask<bool> EscalateAsync()
            => ProcessEvent(() => businessProcess.EscalateTaskAsync(this));

        public  ValueTask<bool> SignalAsync(string signal)
            => ProcessEvent(() => businessProcess.EmitTaskSignalAsync(this, signal));


    }
}
