using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
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

        public void Debug(string message)
        {
            _task.Debug(message);
        }

        public void Debug(string message, object[] pars)
        {
            _task.Debug(message, pars);
        }

        public void Error(string message)
        {
            _task.Error(message);
        }

        public void Error(string message, object[] pars)
        {
            _task.Error(message, pars);
        }

        public Exception Exception(Exception exception)
        {
            return _task.Exception(exception);
        }

        public void Fatal(string message)
        {
            _task.Fatal(message);
        }

        public void Fatal(string message, object[] pars)
        {
            _task.Fatal(message, pars);
        }

        public void Info(string message)
        {
            _task.Info(message);
        }

        public void Info(string message, object[] pars)
        {
            _task.Info(message, pars);
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
