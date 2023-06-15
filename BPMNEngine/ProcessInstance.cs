using Microsoft.Maui.Graphics;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces;
using BPMNEngine.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace BPMNEngine
{
    internal sealed class ProcessInstance : IProcessInstance
    {
        public BusinessProcess Process { get; private init; }
        public Guid ID { get; private init; }
        public ProcessState State { get; private init; }

        private ManualResetEvent _processLock=null;
        private ManualResetEvent ProcessLock
        {
            get
            {
                _processLock ??= new ManualResetEvent(false);
                return _processLock;
            }
        }
        private ManualResetEvent _mreSuspend=null;
        public ManualResetEvent MreSuspend
        {
            get
            {
                _mreSuspend ??= new ManualResetEvent(false);
                return _mreSuspend;
            }
        }
        private LogLevels _stateLogLevel;
        public DelegateContainer Delegates { get; private init; }

        private bool _isSuspended = false;
        public bool IsSuspended => _isSuspended;

        private bool _isComplete = false;

        internal ProcessInstance(BusinessProcess process, DelegateContainer delegates, LogLevels stateLogLevel)
        {
            ID = Utility.NextRandomGuid();
            Process = process;
            Delegates=delegates;
            State = new ProcessState(Process, new ProcessStepComplete(ProcessStepComplete), new ProcessStepError(ProcessStepError), delegates.Events.OnStateChange);
            _stateLogLevel=stateLogLevel;
        }

        internal bool LoadState(XmlDocument doc, bool autoResume)
        {
            if (State.Load(doc))
            {
                WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, "State loaded for Business Process");
                _isSuspended = State.IsSuspended;
                if (autoResume&&_isSuspended)
                    ((IProcessInstance)this).Resume();
                return true;
            }
            return false;
        }

        internal void CompleteProcess()
        {
            _isComplete=true;
            _processLock?.Set();
        }

        private void ProcessStepComplete(string sourceID, string outgoingID)
        {
            Process.ProcessStepComplete(this, sourceID, outgoingID);
        }

        private void ProcessStepError(IElement step, Exception ex)
        {
            Process.ProcessStepError(this, step, ex);
        }

        private static void InvokeElementEventDelegate(Delegate @delegate,IElement element,IReadonlyVariables variables)
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
            State.Path.SucceedFlowNode(evnt);
            InvokeElementEventDelegate(Delegates.Events.Events.Completed, evnt, new ReadOnlyProcessVariablesContainer(evnt.id, this));
        }

        internal void StartTimedEvent(BoundaryEvent evnt, string sourceID)
        {
            Process.ProcessEvent(this, sourceID, evnt);
        }

        internal void EmitTaskError(Tasks.ExternalTask externalTask, Exception error, out bool isAborted)
        {
            InvokeElementEventDelegate(Delegates.Events.Tasks.Error, externalTask, new ReadOnlyProcessVariablesContainer(externalTask.id, this));
            Process.HandleTaskEmission(this, externalTask, error, EventSubTypes.Error, out isAborted);
        }

        internal void EmitTaskMessage(Tasks.ExternalTask externalTask, string message,out bool isAborted)
        {
            Process.HandleTaskEmission(this, externalTask, message, Elements.Processes.Events.EventSubTypes.Message, out isAborted);
        }

        internal void EscalateTask(Tasks.ExternalTask externalTask, out bool isAborted)
        {
            Process.HandleTaskEmission(this, externalTask, null, Elements.Processes.Events.EventSubTypes.Escalation, out isAborted);
        }

        internal void EmitTaskSignal(Tasks.ExternalTask externalTask, string signal, out bool isAborted)
        {
            Process.HandleTaskEmission(this, externalTask, signal, Elements.Processes.Events.EventSubTypes.Signal, out isAborted);
        }

        internal void CompleteTask(Tasks.ManualTask manualTask)
        {
            MergeVariables(manualTask);
        }

        public void MergeVariables(ITask task)
        {
            if (!((Tasks.ExternalTask)task).Aborted)
            {
                WriteLogLine(task, LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Merging variables from Task[{0}] complete by {1} into the state", new object[] { task.id, task is IUserTask task1 ? task1.UserID : null }));
                IVariables vars = task.Variables;
                State.MergeVariables(task,vars);
                InvokeElementEventDelegate(Delegates.Events.Tasks.Completed, task, new ReadOnlyProcessVariablesContainer(task.id, this));
                ATask tsk = Process.GetTask(task.id);
                if (tsk is UserTask task2)
                    State.Path.SucceedFlowNode(task2, completedByID:((IUserTask)task).UserID);
                else
                    State.Path.SucceedFlowNode(tsk);
            }
        }

        public void Dispose()
        {
            if (State.ActiveSteps.Any())
                throw new ActiveStepsException();
            Utility.UnloadProcess(this);
            State.Dispose();
            _processLock?.Dispose();
            _mreSuspend?.Dispose();
        }
        public override bool Equals(object obj)
        {
            if (obj is ProcessInstance pi)
            {
                return pi.ID == ID && pi.Process.Equals(Process);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode()&Process.GetHashCode();
        }

        #region IProcessInstance
        XmlDocument IProcessInstance.Document
        {
            get { return Process.Document; }
        }

        object IProcessInstance.this[string name] { get { return Process[name]; } }

        IEnumerable<string> IProcessInstance.Keys { get { return Process.Keys; } }

        IState IProcessInstance.CurrentState { get { return State.CurrentState; } }

        LogLevels IProcessInstance.StateLogLevel { get { return _stateLogLevel; } set { _stateLogLevel=value; } }

        void IProcessInstance.Resume()
        {
            WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Attempting to resmue Business Process");
            if (_isSuspended)
            {
                _isSuspended = false;
                State.Resume(this,(string incomingID,string elementID) =>
                {
                    ProcessStepComplete(incomingID, elementID);
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
            State.Suspend();
            Utility.UnloadProcess(this);
            var cnt = 0;
            while (State.ActiveSteps.Any() && cnt<10)
            {
                if (!MreSuspend.WaitOne(5000))
                    break;
                cnt++;
            }
            if (Delegates.Events.OnStateChange!=null)
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        Delegates.Events.OnStateChange.Invoke(State.CurrentState);
                    }
                    catch (Exception) { }
                });
        }

        IManualTask IProcessInstance.GetManualTask(string taskID)
        {
            IManualTask ret = null;
            if (State.Path.GetStatus(taskID)==StepStatuses.Waiting)
            {
                ATask elem = Process.GetTask(taskID);
                if (elem != null && elem is ManualTask)
                    ret = new BPMNEngine.Tasks.ManualTask((ATask)elem, new ProcessVariablesContainer(taskID, this), this);
            }
            return ret;
        }

        IUserTask IProcessInstance.GetUserTask(string taskID)
        {
            IUserTask ret = null;
            if (State.Path.GetStatus(taskID)==StepStatuses.Waiting)
            {
                ATask elem = Process.GetTask(taskID);
                if (elem != null && elem is UserTask)
                    ret = new BPMNEngine.Tasks.UserTask((ATask)elem, new ProcessVariablesContainer(taskID, this), this);
            }
            return ret;
        }

        #region Logging
        internal void WriteLogLine(string elementID, LogLevels level, StackFrame sf, DateTime timestamp, string message)
        {
            WriteLogLine((IElement)(elementID==null ? null : Process.GetElement(elementID)), level, sf, timestamp, message);
        }
        internal void WriteLogLine(IElement element, LogLevels level, StackFrame sf, DateTime timestamp, string message)
        {
            if ((int)level <= (int)_stateLogLevel && State!=null)
                State.LogLine(element?.id, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
            Delegates.Logging.LogLine?.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

        internal Exception WriteLogException(string elementID, StackFrame sf, DateTime timestamp, Exception exception)
        {
            return WriteLogException((IElement)(elementID == null ? null : Process.GetElement(elementID)), sf, timestamp, exception);
        }

        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if ((int)LogLevels.Error <= (int)_stateLogLevel)
                State.LogException(element?.id, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            Delegates.Logging.LogException?.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
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

        byte[] IProcessInstance.Diagram(bool outputVariables,ImageFormat type) { return Process.Diagram(outputVariables, State, type); }

        byte[] IProcessInstance.Animate(bool outputVariables) { return Process.Animate(outputVariables, State); }

        #endregion
    }
}
