using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Tasks
{
    internal class ExternalTask : ITask
    {
        private readonly ATask _task;
        private readonly ProcessVariablesContainer _variables;
        protected ProcessInstance _businessProcess;
        private bool _aborted;
        public bool Aborted { get { return _aborted; } }

        public ExternalTask(ATask task,ProcessVariablesContainer variables, ProcessInstance process)
        {
            _task=task;
            _variables=variables;
            _businessProcess=process;
        }

        #region IElement
        string IElement.this[string attributeName]
        {
            get { return _task[attributeName]; }
        }

        public IElement Process
        {
            get { return _task.Process; }
        }

        public IElement SubProcess
        {
            get { return _task.SubProcess; }
        }

        public IElement Lane
        {
            get { return _task.Lane; }
        }

        public string id
        {
            get { return _task.id; }
        }

        public IEnumerable<XmlNode> SubNodes
        {
            get { return _task.SubNodes; }
        }

        public IElement ExtensionElement
        {
            get { return _task.ExtensionElement; }
        }

        private void WriteLogLine(LogLevel level,string message)
        {
            _businessProcess.WriteLogLine(_task, level, new StackFrame(2, true), DateTime.Now, message);
        }

        public void Debug(string message)
        {
            WriteLogLine(LogLevel.Debug, message);
        }

        public void Debug(string message, object[] pars)
        {
            WriteLogLine(LogLevel.Debug, string.Format(message,pars));
        }

        public void Error(string message)
        {
            WriteLogLine(LogLevel.Error, message);
        }

        public void Error(string message, object[] pars)
        {
            WriteLogLine(LogLevel.Error, string.Format(message, pars));
        }

        public Exception Exception(Exception exception)
        {
            return _businessProcess.WriteLogException(_task,new StackFrame(1),DateTime.Now,exception);
        }

        public void Fatal(string message)
        {
            WriteLogLine(LogLevel.Critical, message);
        }

        public void Fatal(string message, object[] pars)
        {
            WriteLogLine(LogLevel.Critical, string.Format(message, pars));
        }

        public void Info(string message)
        {
            WriteLogLine(LogLevel.Information, message);
        }

        public void Info(string message, object[] pars)
        {
            WriteLogLine(LogLevel.Information, string.Format(message, pars));
        }
        #endregion

        public void EmitError(Exception error, out bool isAborted)
        {
            _businessProcess.EmitTaskError(this, error, out isAborted);
            _aborted=_aborted||isAborted;
        }

        public void EmitMessage(string message, out bool isAborted)
        {
            _businessProcess.EmitTaskMessage(this, message,out isAborted);
            _aborted=_aborted||isAborted;
        }


        public void Escalate(out bool isAborted)
        {
            _businessProcess.EscalateTask(this, out isAborted);
            _aborted=_aborted||isAborted;
        }

        public void Signal(string signal, out bool isAborted)
        {
            _businessProcess.EmitTaskSignal(this, signal, out isAborted);
            _aborted=_aborted||isAborted;
        }

        public IVariables Variables { get { return _variables; } }
    }
}
