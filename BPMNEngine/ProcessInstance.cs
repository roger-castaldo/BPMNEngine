using Microsoft.Maui.Graphics;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces;
using BPMNEngine.State;
using System.Threading;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.State;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.Scheduling;

namespace BPMNEngine
{
    internal sealed class ProcessInstance : IProcessInstance
    {
        public BusinessProcess Process { get; private init; }
        public Guid ID { get; private init; }
        public ProcessState State { get; private init; }

        private ManualResetEvent processLock=null;
        private ManualResetEvent ProcessLock
        {
            get
            {
                processLock ??= new ManualResetEvent(false);
                return processLock;
            }
        }
        private ManualResetEvent mreSuspend=null;
        public ManualResetEvent MreSuspend
        {
            get
            {
                mreSuspend ??= new ManualResetEvent(false);
                return mreSuspend;
            }
        }

        private readonly LogLevel stateLogLevel;
        public DelegateContainer Delegates { get; private init; }
        public bool IsSuspended { get; private set; }
        private bool isComplete = false;

        internal ProcessInstance(BusinessProcess process, DelegateContainer delegates, LogLevel stateLogLevel)
        {
            ID = Guid.NewGuid();
            Process = process;
            Delegates=delegates;
            State = new ProcessState(Process, new ProcessStepComplete(ProcessStepComplete), new ProcessStepError(ProcessStepError), delegates.Events.OnStateChange);
            this.stateLogLevel=stateLogLevel;
        }

        internal bool LoadState(XmlDocument doc, bool autoResume)
        {
            if (State.Load(doc))
            {
                WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, "State loaded for Business Process");
                IsSuspended = State.IsSuspended;
                if (autoResume&&IsSuspended)
                    ((IProcessInstance)this).Resume();
                return true;
            }
            return false;
        }

        internal void CompleteProcess()
        {
            isComplete=true;
            processLock?.Set();
        }

        private void ProcessStepComplete(string sourceID, string outgoingID)
            => Process.ProcessStepComplete(this, sourceID, outgoingID);

        private void ProcessStepError(IElement step, Exception ex)
            => Process.ProcessStepError(this, step, ex);

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
            InvokeElementEventDelegate(Delegates.Events.Events.Completed, evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, this));
        }

        internal void StartTimedEvent(BoundaryEvent evnt, string sourceID)
            => Process.ProcessEvent(this, sourceID, evnt);

        internal void EmitTaskError(Tasks.ExternalTask externalTask, Exception error, out bool isAborted)
        {
            InvokeElementEventDelegate(Delegates.Events.Tasks.Error, externalTask, new ReadOnlyProcessVariablesContainer(externalTask.ID, this));
            Process.HandleTaskEmission(this, externalTask, error, EventSubTypes.Error, out isAborted);
        }

        internal void EmitTaskMessage(Tasks.ExternalTask externalTask, string message,out bool isAborted)
            => Process.HandleTaskEmission(this, externalTask, message, Elements.Processes.Events.EventSubTypes.Message, out isAborted);

        internal void EscalateTask(Tasks.ExternalTask externalTask, out bool isAborted)
            => Process.HandleTaskEmission(this, externalTask, null, Elements.Processes.Events.EventSubTypes.Escalation, out isAborted);

        internal void EmitTaskSignal(Tasks.ExternalTask externalTask, string signal, out bool isAborted)
            => Process.HandleTaskEmission(this, externalTask, signal, Elements.Processes.Events.EventSubTypes.Signal, out isAborted);

        internal void CompleteTask(Tasks.ManualTask manualTask)
            => MergeVariables(manualTask);

        public void MergeVariables(ITask task)
        {
            if (!((Tasks.ExternalTask)task).Aborted)
            {
                WriteLogLine(task, LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Merging variables from Task[{0}] complete by {1} into the state", new object[] { task.ID, task is IUserTask task1 ? task1.UserID : null }));
                IVariables vars = task.Variables;
                State.MergeVariables(task,vars);
                InvokeElementEventDelegate(Delegates.Events.Tasks.Completed, task, new ReadOnlyProcessVariablesContainer(task.ID, this));
                ATask tsk = Process.GetTask(task.ID);
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
            StepScheduler.Instance.UnloadProcess(this);
            State.Dispose();
            processLock?.Dispose();
            mreSuspend?.Dispose();
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
            => ID.GetHashCode()&Process.GetHashCode();

        #region IProcessInstance
        XmlDocument IProcessInstance.Document
            => Process.Document; 
        object IProcessInstance.this[string name]
            =>Process[name];

        IEnumerable<string> IProcessInstance.Keys
            =>Process.Keys;

        IState IProcessInstance.CurrentState 
            =>State.CurrentState;

        public LogLevel StateLogLevel { get; private init; }

        void IProcessInstance.Resume()
        {
            WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, "Attempting to resmue Business Process");
            if (IsSuspended)
            {
                IsSuspended = false;
                State.Resume(this,(string incomingID,string elementID) =>
                {
                    ProcessStepComplete(incomingID, elementID);
                },
                (AEvent delayedEvent) =>
                {
                    CompleteTimedEvent(delayedEvent);
                });
                WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, "Business Process Resume Complete");
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
            WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, "Suspending Business Process");
            IsSuspended = true;
            State.Suspend();
            StepScheduler.Instance.UnloadProcess(this);
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
        internal void WriteLogLine(string elementID, LogLevel level, StackFrame sf, DateTime timestamp, string message)
            => WriteLogLine((IElement)(elementID == null ? null : Process.GetElement(elementID)), level, sf, timestamp, message) ;
        internal void WriteLogLine(IElement element, LogLevel level, StackFrame sf, DateTime timestamp, string message)
        {
            if ((int)level >= (int)stateLogLevel && State!=null)
                State.LogLine(element?.ID, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
            Delegates.Logging.LogLine?.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

        internal Exception WriteLogException(string elementID, StackFrame sf, DateTime timestamp, Exception exception)
            => WriteLogException((IElement)(elementID == null ? null : Process.GetElement(elementID)), sf, timestamp, exception);

        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if ((int)LogLevel.Error>= (int)stateLogLevel)
                State.LogException(element?.ID, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            Delegates.Logging.LogException?.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            return exception;
        }
        #endregion

        #region ProcessLock
        bool IProcessInstance.WaitForCompletion()
        {
            var result = true;
            if (!isComplete)
            {
                result = ProcessLock.WaitOne();
                result = result || isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(int millisecondsTimeout)
        {
            var result = true;
            if (!isComplete)
            {
                result = ProcessLock.WaitOne(millisecondsTimeout);
                result = result || isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(TimeSpan timeout)
        {
            var result = true;
            if (!isComplete)
            {
                result = ProcessLock.WaitOne(timeout);
                result = result || isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(int millisecondsTimeout, bool exitContext)
        {
            var result = true;
            if (!isComplete)
            {
                result = ProcessLock.WaitOne(millisecondsTimeout,exitContext);
                result = result || isComplete;
            }
            return result;
        }
        bool IProcessInstance.WaitForCompletion(TimeSpan timeout, bool exitContext)
        {
            var result = true;
            if (!isComplete)
            {
                result = ProcessLock.WaitOne(timeout, exitContext);
                result = result || isComplete;
            }
            return result;
        }
        Dictionary<string, object> IProcessInstance.CurrentVariables 
            => ProcessVariables.ExtractVariables(((IProcessInstance)this).CurrentState);
        #endregion

        byte[] IProcessInstance.Diagram(bool outputVariables,ImageFormat type)
            => Process.Diagram(outputVariables, State, type);

        byte[] IProcessInstance.Animate(bool outputVariables)
            => Process.Animate(outputVariables, State);

        #endregion
    }
}
