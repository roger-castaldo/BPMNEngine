using Microsoft.Maui.Graphics;
using BpmEngine.Elements.Processes.Events;
using BpmEngine.Elements.Processes.Tasks;
using BpmEngine.Interfaces;
using BpmEngine.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace BpmEngine
{
    internal sealed class ProcessInstance : IProcessInstance
    {
        private BusinessProcess _process;
        public BusinessProcess Process => _process;

        private Guid _id;
        public Guid ID => _id;

        private ProcessState _state;
        public ProcessState State => _state;

        private ManualResetEvent _processLock=null;
        private ManualResetEvent ProcessLock
        {
            get
            {
                if (_processLock==null)
                    _processLock = new ManualResetEvent(false);
                return _processLock;
            }
        }
        private ManualResetEvent _mreSuspend=null;
        public ManualResetEvent MreSuspend
        {
            get
            {
                if (_mreSuspend==null)
                    _mreSuspend = new ManualResetEvent(false);
                return _mreSuspend;
            }
        }
        private LogLevels _stateLogLevel;

        private DelegateContainer _delegates;
        public DelegateContainer Delegates => _delegates;

        private bool _isSuspended = false;
        public bool IsSuspended => _isSuspended;

        private bool _isComplete = false;

        internal ProcessInstance(BusinessProcess process, DelegateContainer delegates, LogLevels stateLogLevel)
        {
            _id = Utility.NextRandomGuid();
            _process = process;
            _delegates=delegates;
            _state = new ProcessState(_process, new ProcessStepComplete(_ProcessStepComplete), new ProcessStepError(_ProcessStepError), delegates.Events.OnStateChange);
            _stateLogLevel=stateLogLevel;
        }

        internal bool LoadState(XmlDocument doc, bool autoResume)
        {
            if (_state.Load(doc))
            {
                WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, "State loaded for Business Process");
                _isSuspended = _state.IsSuspended;
                if (autoResume&&_isSuspended)
                    ((IProcessInstance)this).Resume();
                return true;
            }
            return false;
        }

        internal void CompleteProcess()
        {
            _isComplete=true;
            if (_processLock!=null)
                _processLock.Set();
        }

        private void _ProcessStepComplete(string sourceID, string outgoingID)
        {
            _process.ProcessStepComplete(this, sourceID, outgoingID);
        }

        private void _ProcessStepError(IElement step, Exception ex)
        {
            _process.ProcessStepError(this, step, ex);
        }

        private static void _InvokeElementEventDelegate(Delegate @delegate,IElement element,IReadonlyVariables variables)
        {
            if (@delegate!=null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        @delegate.DynamicInvoke(element, variables);
                    }
                    catch (Exception) { }
                });
            }
        }

        internal void CompleteTimedEvent(AEvent evnt)
        {
            _state.Path.SucceedFlowNode(evnt);
            _InvokeElementEventDelegate(_delegates.Events.Events.Completed, evnt, new ReadOnlyProcessVariablesContainer(evnt.id, this));
        }

        internal void StartTimedEvent(BoundaryEvent evnt, string sourceID)
        {
            _process.ProcessEvent(this, sourceID, evnt);
        }

        internal void EmitTaskError(Tasks.ExternalTask externalTask, Exception error, out bool isAborted)
        {
            _InvokeElementEventDelegate(_delegates.Events.Tasks.Error, externalTask, new ReadOnlyProcessVariablesContainer(externalTask.id, this));
            _process.HandleTaskEmission(this, externalTask, error, EventSubTypes.Error, out isAborted);
        }

        internal void EmitTaskMessage(Tasks.ExternalTask externalTask, string message,out bool isAborted)
        {
            _process.HandleTaskEmission(this, externalTask, message, Elements.Processes.Events.EventSubTypes.Message, out isAborted);
        }

        internal void EscalateTask(Tasks.ExternalTask externalTask, out bool isAborted)
        {
            _process.HandleTaskEmission(this, externalTask, null, Elements.Processes.Events.EventSubTypes.Escalation, out isAborted);
        }

        internal void EmitTaskSignal(Tasks.ExternalTask externalTask, string signal, out bool isAborted)
        {
            _process.HandleTaskEmission(this, externalTask, signal, Elements.Processes.Events.EventSubTypes.Signal, out isAborted);
        }

        internal void CompleteTask(Tasks.ManualTask manualTask)
        {
            MergeVariables(manualTask);
        }

        public void MergeVariables(ITask task)
        {
            if (!((Tasks.ExternalTask)task).Aborted)
            {
                WriteLogLine(task, LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Merging variables from Task[{0}] complete by {1} into the state", new object[] { task.id, (task is IUserTask ? ((IUserTask)task).UserID : null) }));
                IVariables vars = task.Variables;
                _state.MergeVariables(task,vars);
                _InvokeElementEventDelegate(_delegates.Events.Tasks.Completed, task, new ReadOnlyProcessVariablesContainer(task.id, this));
                ATask tsk = _process.GetTask(task.id);
                if (tsk is UserTask)
                    _state.Path.SucceedFlowNode((UserTask)tsk, completedByID:((IUserTask)task).UserID);
                else
                    _state.Path.SucceedFlowNode(tsk);
            }
        }

        public void Dispose()
        {
            if (_state.ActiveSteps.Any())
                throw new ActiveStepsException();
            Utility.UnloadProcess(this);
            _state.Dispose();
            if (_processLock!=null)
                _processLock.Dispose();
            if (_mreSuspend!=null)
                _mreSuspend.Dispose();
        }
        public override bool Equals(object obj)
        {
            if (obj is ProcessInstance)
            {
                ProcessInstance pi = (ProcessInstance)obj;
                return pi._id == _id && pi._process.Equals(_process);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _id.GetHashCode()&_process.GetHashCode();
        }

        #region IProcessInstance
        XmlDocument IProcessInstance.Document
        {
            get { return _process.Document; }
        }

        object IProcessInstance.this[string name] { get { return _process[name]; } }

        IEnumerable<string> IProcessInstance.Keys { get { return _process.Keys; } }

        IState IProcessInstance.CurrentState { get { return _state.CurrentState; } }

        LogLevels IProcessInstance.StateLogLevel { get { return _stateLogLevel; } set { _stateLogLevel=value; } }

        void IProcessInstance.Resume()
        {
            WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Attempting to resmue Business Process");
            if (_isSuspended)
            {
                _isSuspended = false;
                _state.Resume(this,(string incomingID,string elementID) =>
                {
                    _ProcessStepComplete(incomingID, elementID);
                },
                (AEvent delayedEvent) =>
                {
                    CompleteTimedEvent(delayedEvent);
                });
                WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Business Process Resume Complete");
            }
            else
            {
                Exception ex = new NotSuspendedException();
                WriteLogException((IElement)null, new StackFrame(1, true), DateTime.Now, ex);
                throw ex;
            }
        }

        void IProcessInstance.Suspend()
        {
            WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Suspending Business Process");
            _isSuspended = true;
            _state.Suspend();
            Utility.UnloadProcess(this);
            var cnt = 0;
            while (_state.ActiveSteps.Any() && cnt<10)
            {
                if (!MreSuspend.WaitOne(5000))
                    break;
                cnt++;
            }
            if (_delegates.Events.OnStateChange!=null)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _delegates.Events.OnStateChange.Invoke(_state.CurrentState);
                    }
                    catch (Exception) { }
                });
        }

        IManualTask IProcessInstance.GetManualTask(string taskID)
        {
            IManualTask ret = null;
            if (_state.Path.GetStatus(taskID)==StepStatuses.Waiting)
            {
                ATask elem = _process.GetTask(taskID);
                if (elem != null && elem is ManualTask)
                    ret = new BpmEngine.Tasks.ManualTask((ATask)elem, new ProcessVariablesContainer(taskID, this), this);
            }
            return ret;
        }

        IUserTask IProcessInstance.GetUserTask(string taskID)
        {
            IUserTask ret = null;
            if (_state.Path.GetStatus(taskID)==StepStatuses.Waiting)
            {
                ATask elem = _process.GetTask(taskID);
                if (elem != null && elem is UserTask)
                    ret = new BpmEngine.Tasks.UserTask((ATask)elem, new ProcessVariablesContainer(taskID, this), this);
            }
            return ret;
        }

        #region Logging
        internal void WriteLogLine(string elementID, LogLevels level, StackFrame sf, DateTime timestamp, string message)
        {
            WriteLogLine((IElement)(elementID==null ? null : _process.GetElement(elementID)), level, sf, timestamp, message);
        }
        internal void WriteLogLine(IElement element, LogLevels level, StackFrame sf, DateTime timestamp, string message)
        {
            if ((int)level <= (int)_stateLogLevel && _state!=null)
                _state.LogLine((element==null ? null : element.id), sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
            if (_delegates.Logging.LogLine != null)
                _delegates.Logging.LogLine.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

        internal Exception WriteLogException(string elementID, StackFrame sf, DateTime timestamp, Exception exception)
        {
            return WriteLogException((IElement)(elementID == null ? null : _process.GetElement(elementID)), sf, timestamp, exception);
        }

        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if ((int)LogLevels.Error <= (int)_stateLogLevel)
                _state.LogException((element==null ? null : element.id), sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            if (_delegates.Logging.LogException != null)
                _delegates.Logging.LogException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            return exception;
        }
        #endregion

        #region ProcessLock
        bool IProcessInstance.WaitForCompletion()
        {
            var result = true;
            if (!_isComplete)
            {
                result = ProcessLock.WaitOne();
                result = result || _isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(int millisecondsTimeout)
        {
            var result = true;
            if (!_isComplete)
            {
                result = ProcessLock.WaitOne(millisecondsTimeout);
                result = result || _isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(TimeSpan timeout)
        {
            var result = true;
            if (!_isComplete)
            {
                result = ProcessLock.WaitOne(timeout);
                result = result || _isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(int millisecondsTimeout, bool exitContext)
        {
            var result = true;
            if (!_isComplete)
            {
                result = ProcessLock.WaitOne(millisecondsTimeout,exitContext);
                result = result || _isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(TimeSpan timeout, bool exitContext)
        {
            var result = true;
            if (!_isComplete)
            {
                result = ProcessLock.WaitOne(timeout, exitContext);
                result = result || _isComplete;
            }
            return result;
        }
        Dictionary<string, object> IProcessInstance.CurrentVariables { get { return ProcessVariables.ExtractVariables(((IProcessInstance)this).CurrentState); } }
        #endregion

        byte[] IProcessInstance.Diagram(bool outputVariables,ImageFormat type) { return _process.Diagram(outputVariables, _state, type); }

        byte[] IProcessInstance.Animate(bool outputVariables) { return _process.Animate(outputVariables, _state); }

        #endregion
    }
}
