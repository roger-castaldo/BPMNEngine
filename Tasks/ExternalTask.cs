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
        protected BusinessProcess _businessProcess;

        public ExternalTask(ATask task,ProcessVariablesContainer variables,BusinessProcess process)
        {
            _task=task;
            _variables=variables;
            _businessProcess=process;
        }

        #region IVariables
        public object this[string name] { 
            get { 
                return _variables[name]; 
            } 
            set { 
                _variables[name]=value; 
            } 
        }

        public string[] Keys { get { return _variables.Keys; } }

        public string[] FullKeys
        {
            get { return _variables.FullKeys; }
        }
        #endregion

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

        public XmlNode[] SubNodes
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

        public void EmitError(Exception error)
        {
            _businessProcess.ErrorTask(this, error);
        }

        public void EmitMessage(string message)
        {
            _businessProcess.EmitTaskMessage(this, message);
        }


        public void Escalate()
        {
            _businessProcess.EscalateTask(this);
        }

        public void Signal(string signal)
        {
            _businessProcess.EmitTaskSignal(this, signal);
        }
    }
}
