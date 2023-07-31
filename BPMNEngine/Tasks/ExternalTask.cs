using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Tasks
{
    internal class ExternalTask : ITask
    {
        private readonly ATask task;
        protected readonly ProcessInstance businessProcess;
        public bool Aborted { get; private set; }
        public IVariables Variables { get; private init; }

        public ExternalTask(ATask task,ProcessVariablesContainer variables, ProcessInstance process)
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

        public IEnumerable<XmlNode> SubNodes
            => task.SubNodes;

        public IElement ExtensionElement
            => task.ExtensionElement;

        private void WriteLogLine(LogLevel level,string message)
            => businessProcess.WriteLogLine(task, level, new StackFrame(2, true), DateTime.Now, message);

        public void Debug(string message)
            => WriteLogLine(LogLevel.Debug, message);

        public void Debug(string message, object[] pars)
            => WriteLogLine(LogLevel.Debug, string.Format(message,pars));

        public void Error(string message)
            => WriteLogLine(LogLevel.Error, message);

        public void Error(string message, object[] pars)
            => WriteLogLine(LogLevel.Error, string.Format(message, pars));

        public Exception Exception(Exception exception)
            => businessProcess.WriteLogException(task,new StackFrame(1),DateTime.Now,exception);

        public void Fatal(string message)
            => WriteLogLine(LogLevel.Critical, message);

        public void Fatal(string message, object[] pars)
            => WriteLogLine(LogLevel.Critical, string.Format(message, pars));

        public void Info(string message)
            => WriteLogLine(LogLevel.Information, message);

        public void Info(string message, object[] pars)
            => WriteLogLine(LogLevel.Information, string.Format(message, pars));
        #endregion

        public void EmitError(Exception error, out bool isAborted)
        {
            businessProcess.EmitTaskError(this, error, out isAborted);
            Aborted=Aborted||isAborted;
        }

        public void EmitMessage(string message, out bool isAborted)
        {
            businessProcess.EmitTaskMessage(this, message,out isAborted);
            Aborted=Aborted||isAborted;
        }


        public void Escalate(out bool isAborted)
        {
            businessProcess.EscalateTask(this, out isAborted);
            Aborted=Aborted||isAborted;
        }

        public void Signal(string signal, out bool isAborted)
        {
            businessProcess.EmitTaskSignal(this, signal, out isAborted);
            Aborted=Aborted||isAborted;
        }

        
    }
}
