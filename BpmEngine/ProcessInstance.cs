using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal sealed class ProcessInstance : IProcessInstance
    {
        private BusinessProcess _process;
        public BusinessProcess Process { get { return _process; } }

        private Guid _id;
        public Guid ID { get { return _id; } }

        private ProcessState _state;
        public ProcessState State { get { return _state; } }

        private ManualResetEvent _processLock;
        internal ManualResetEvent ProcessLock { get { return _processLock; } }
        private ManualResetEvent _mreSuspend;
        public ManualResetEvent MreSuspend { get { return _mreSuspend; } }
        private LogLevels _stateLogLevel;

        private DelegateContainer _delegates;
        public DelegateContainer Delegates { get { return _delegates; } }
        private AutoResetEvent _stateEvent = new AutoResetEvent(true);
        public AutoResetEvent StateEvent { get { return _stateEvent; } }
        private bool _isSuspended = false;
        public bool IsSuspended { get { return _isSuspended; } }

        internal ProcessInstance(BusinessProcess process, DelegateContainer delegates, LogLevels stateLogLevel)
        {
            _id = Utility.NextRandomGuid();
            _processLock = new ManualResetEvent(false);
            _mreSuspend = new ManualResetEvent(false);
            _process = process;
            _delegates=delegates;
            _state = new ProcessState(_process, new ProcessStepComplete(_ProcessStepComplete), new ProcessStepError(_ProcessStepError), new OnStateChange(_StateChange));
            _stateLogLevel=stateLogLevel;
        }

        private void _StateChange(XmlDocument stateDocument)
        {
            _stateEvent.WaitOne();
            if (_delegates.OnStateChange!=null)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _delegates.OnStateChange.Invoke(stateDocument);
                    }
                    catch (Exception ex) { }
                });
            _stateEvent.Set();
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

        private void _ProcessStepComplete(string sourceID, string outgoingID)
        {
            _process.ProcessStepComplete(this, sourceID, outgoingID);
        }

        private void _ProcessStepError(IElement step, Exception ex)
        {
            _process.ProcessStepError(this, step, ex);
        }

        internal void CompleteTimedEvent(AEvent evnt)
        {
            _stateEvent.WaitOne();
            _state.Path.SucceedEvent(evnt);
            _stateEvent.Set();
            if (_delegates.OnEventCompleted!=null)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _delegates.OnEventCompleted.Invoke(evnt, new ReadOnlyProcessVariablesContainer(evnt.id, this));
                    }
                    catch (Exception ex) { }
                });
        }

        internal void StartTimedEvent(BoundaryEvent evnt, string sourceID)
        {
            _process.ProcessEvent(this, sourceID, evnt);
        }

        internal void ErrorTask(Tasks.ExternalTask externalTask, Exception error)
        {
            if (_delegates.OnTaskError!=null)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _delegates.OnTaskError.Invoke(externalTask, new ReadOnlyProcessVariablesContainer(externalTask.id, this));
                    }
                    catch (Exception ex) { }
                });
            _stateEvent.WaitOne();
            _state.Path.FailTask(_process.GetTask(externalTask.id), error);
            _stateEvent.Set();
        }

        internal void EmitTaskMessage(Tasks.ExternalTask externalTask, string message)
        {
            _process.HandleTaskEmission(this, externalTask, message, Elements.Processes.Events.EventSubTypes.Message);
        }

        internal void EscalateTask(Tasks.ExternalTask externalTask)
        {
            _process.HandleTaskEmission(this, externalTask, null, Elements.Processes.Events.EventSubTypes.Escalation);
        }

        internal void EmitTaskSignal(Tasks.ExternalTask externalTask, string signal)
        {
            _process.HandleTaskEmission(this, externalTask, signal, Elements.Processes.Events.EventSubTypes.Signal);
        }

        internal void CompleteTask(Tasks.ManualTask manualTask)
        {
            MergeVariables(manualTask);
        }

        public void MergeVariables(ITask task)
        {
            WriteLogLine(task, LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Merging variables from Task[{0}] complete by {1} into the state", new object[] { task.id, (task is IUserTask ? ((IUserTask)task).UserID : null) }));
            _stateEvent.WaitOne();
            IVariables vars = task.Variables;
            foreach (string str in vars.Keys)
            {
                object left = vars[str];
                object right = _state[task.id, str];
                if (!_IsVariablesEqual(left, right))
                    _state[task.id, str] = left;
            }
            if (_delegates.OnTaskCompleted!=null)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _delegates.OnTaskCompleted.Invoke(task, new ReadOnlyProcessVariablesContainer(task.id, this));
                    }
                    catch (Exception ex) { }
                });
            ATask tsk = _process.GetTask(task.id);
            if (tsk is UserTask)
                _state.Path.SucceedTask((UserTask)tsk, ((IUserTask)task).UserID);
            else
                _state.Path.SucceedTask(tsk);
            _stateEvent.Set();
        }

        private bool _IsVariablesEqual(object left, object right)
        {
            if (left == null && right != null)
                return false;
            else if (left != null && right == null)
                return false;
            else if (left == null && right == null)
                return true;
            else
            {
                if (left is Array)
                {
                    if (!(right is Array))
                        return false;
                    else
                    {
                        Array aleft = (Array)left;
                        Array aright = (Array)right;
                        if (aleft.Length != aright.Length)
                            return false;
                        for (int x = 0; x < aleft.Length; x++)
                        {
                            if (!_IsVariablesEqual(aleft.GetValue(x), aright.GetValue(x)))
                                return false;
                        }
                        return true;
                    }
                }
                else if (left is Hashtable)
                {
                    if (!(right is Hashtable))
                        return false;
                    else
                    {
                        Hashtable hleft = (Hashtable)left;
                        Hashtable hright = (Hashtable)right;
                        if (hleft.Count != hright.Count)
                            return false;
                        foreach (object key in hleft.Keys)
                        {
                            if (!hright.ContainsKey(key))
                                return false;
                            else if (!_IsVariablesEqual(hleft[key], hright[key]))
                                return false;
                        }
                        foreach (object key in hright.Keys)
                        {
                            if (!hleft.ContainsKey(key))
                                return false;
                        }
                        return true;
                    }
                }
                else
                {
                    try { return left.Equals(right); }
                    catch (Exception e) { WriteLogException((string)null, new StackFrame(2, true), DateTime.Now, e); return false; }
                }
            }
        }

        public void Dispose()
        {
            Utility.UnloadProcess(this);
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

        string[] IProcessInstance.Keys { get { return _process.Keys; } }

        XmlDocument IProcessInstance.CurrentState { get { return _state.Document; } }

        LogLevels IProcessInstance.StateLogLevel { get { return _stateLogLevel; } set { _stateLogLevel=value; } }

        void IProcessInstance.Resume()
        {
            WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Attempting to resmue Business Process");
            if (_isSuspended)
            {
                _isSuspended = false;
                sSuspendedStep[] resumeSteps = _state.ResumeSteps;
                _state.Resume();
                if (resumeSteps != null)
                {
                    foreach (sSuspendedStep ss in resumeSteps)
                        _ProcessStepComplete(ss.IncomingID, ss.ElementID);
                }
                foreach (sStepSuspension ss in _state.SuspendedSteps)
                {
                    if (DateTime.Now.Ticks < ss.EndTime.Ticks)
                        Utility.Sleep(ss.EndTime.Subtract(DateTime.Now), this, (AEvent)_process.GetElement(ss.id));
                    else
                        CompleteTimedEvent((AEvent)_process.GetElement(ss.id));
                }
                foreach (sDelayedStartEvent sdse in _state.DelayedEvents)
                {
                    if (sdse.Delay.Ticks<0)
                        _process.ProcessEvent(this, sdse.IncomingID, (AEvent)_process.GetElement(sdse.ElementID));
                    else
                        Utility.DelayStart(sdse.Delay, this, (BoundaryEvent)_process.GetElement(sdse.ElementID), sdse.IncomingID);
                }
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
            _mreSuspend.WaitOne(5000);
            Utility.UnloadProcess(this);
            if (_delegates.OnStateChange!=null)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _delegates.OnStateChange.Invoke(_state.Document);
                    }
                    catch (Exception ex) { }
                });
        }

        IManualTask IProcessInstance.GetManualTask(string taskID)
        {
            IManualTask ret = null;
            if (_state.Path.GetStatus(taskID)==StepStatuses.Waiting)
            {
                ATask elem = _process.GetTask(taskID);
                if (elem != null && elem is ManualTask)
                    ret = new Org.Reddragonit.BpmEngine.Tasks.ManualTask((ATask)elem, new ProcessVariablesContainer(taskID, this), this);
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
                    ret = new Org.Reddragonit.BpmEngine.Tasks.UserTask((ATask)elem, new ProcessVariablesContainer(taskID, this), this);
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
            if (_delegates.LogLine != null)
                _delegates.LogLine.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

        internal Exception WriteLogException(string elementID, StackFrame sf, DateTime timestamp, Exception exception)
        {
            return WriteLogException((IElement)(elementID == null ? null : _process.GetElement(elementID)), sf, timestamp, exception);
        }

        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if ((int)LogLevels.Error <= (int)_stateLogLevel)
                _state.LogException((element==null ? null : element.id), sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            if (_delegates.LogException != null)
                _delegates.LogException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            return exception;
        }
        #endregion

        #region ProcessLock
        bool IProcessInstance.WaitForCompletion()
        {
            return _processLock.WaitOne();
        }
        bool IProcessInstance.WaitForCompletion(int millisecondsTimeout)
        {
            return _processLock.WaitOne(millisecondsTimeout);
        }
        bool IProcessInstance.WaitForCompletion(TimeSpan timeout)
        {
            return _processLock.WaitOne(timeout);
        }
        bool IProcessInstance.WaitForCompletion(int millisecondsTimeout, bool exitContext)
        {
            return _processLock.WaitOne(millisecondsTimeout, exitContext);
        }
        bool IProcessInstance.WaitForCompletion(TimeSpan timeout, bool exitContext)
        {
            return _processLock.WaitOne(timeout, exitContext);
        }
        Dictionary<string, object> IProcessInstance.CurrentVariables { get { return StateVariableContainer.ExtractVariables(((IProcessInstance)this).CurrentState); } }
        #endregion

        byte[] IProcessInstance.Diagram(bool outputVariables,ImageOuputTypes type) { return _process.Diagram(outputVariables, _state, type); }

        byte[] IProcessInstance.Animate(bool outputVariables) { return _process.Animate(outputVariables, _state); }

        #endregion
    }
}
