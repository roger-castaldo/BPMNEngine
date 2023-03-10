using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Tasks
{
    internal class ExternalTask : ITask
    {
        private ATask _task;
        private ProcessVariablesContainer _variables;
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
            get { return _task.Process; }
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

        private void WriteLogLine(LogLevels level,string message)
        {
            _businessProcess.WriteLogLine(_task, level, new StackFrame(2, true), DateTime.Now, message);
        }

        public void Debug(string message)
        {
            WriteLogLine(LogLevels.Debug, message);
        }

        public void Debug(string message, object[] pars)
        {
            WriteLogLine(LogLevels.Debug, string.Format(message,pars));
        }

        public void Error(string message)
        {
            WriteLogLine(LogLevels.Error, message);
        }

        public void Error(string message, object[] pars)
        {
            WriteLogLine(LogLevels.Error, string.Format(message, pars));
        }

        public Exception Exception(Exception exception)
        {
            return _businessProcess.WriteLogException(_task,new StackFrame(1),DateTime.Now,exception);
        }

        public void Fatal(string message)
        {
            WriteLogLine(LogLevels.Fatal, message);
        }

        public void Fatal(string message, object[] pars)
        {
            WriteLogLine(LogLevels.Fatal, string.Format(message, pars));
        }

        public void Info(string message)
        {
            WriteLogLine(LogLevels.Info, message);
        }

        public void Info(string message, object[] pars)
        {
            WriteLogLine(LogLevels.Info, string.Format(message, pars));
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
