using Microsoft.Maui.Graphics;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces;
using System.Threading;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.State;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.Scheduling;
using System.Text.Json;
using System.Collections.Concurrent;

namespace BPMNEngine
{
    internal sealed class ProcessInstance : IProcessInstance
    {
        public BusinessProcess Process { get; private init; }
        public Guid ID { get; private init; }
        public ProcessState State { get; private init; }

        private ManualResetEventSlim processLock =null;
        private ManualResetEventSlim ProcessLock
        {
            get
            {
                processLock ??= new ManualResetEventSlim(false);
                return processLock;
            }
        }
        private ManualResetEventSlim mreSuspend =null;
        public ManualResetEventSlim MreSuspend
        {
            get
            {
                mreSuspend ??= new ManualResetEventSlim(false);
                return mreSuspend;
            }
        }

        private readonly ConcurrentDictionary<string, ManualResetEventSlim> waitingTasks;

        private readonly LogLevel stateLogLevel;
        public DelegateContainer Delegates { get; private init; }
        public bool IsSuspended { get; private set; }
        private bool isComplete = false;
        private bool disposedValue;

        internal ProcessInstance(BusinessProcess process, DelegateContainer delegates, LogLevel stateLogLevel)
        {
            ID = Guid.NewGuid();
            Process = process;
            waitingTasks = new();
            Delegates=DelegateContainer.Merge(delegates,new DelegateContainer()
            {
                Events= new DelegateContainers.ProcessEvents()
                {
                    Tasks = new DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started = (IStepElement element, IReadonlyVariables variables) =>
                        {
                            if (waitingTasks.Remove(element.ID,out var task))
                            {
                                try
                                {
                                    task.Set();
                                    task.Dispose();
                                }catch(Exception) { }
                            }
                        }
                    }
                }
            });
            State = new ProcessState(ID,Process, new ProcessStepComplete(ProcessStepComplete), new ProcessStepError(ProcessStepError), delegates.Events.OnStateChange);
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

        internal bool LoadState(Utf8JsonReader reader, bool autoResume)
        {
            if (State.Load(reader))
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

        
        public override bool Equals(object obj)
            => obj is ProcessInstance pi && pi.ID == ID && pi.Process.Equals(Process);
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
                if (!MreSuspend.Wait(5000))
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

        private void WaitForTask(string taskID,TimeSpan? timeout=null)
        {
            if (!State.WaitingSteps.Contains(taskID))
            {
                if (!waitingTasks.TryGetValue(taskID, out var manualResetEventSlim))
                {
                    manualResetEventSlim = new ManualResetEventSlim(false);
                    waitingTasks.TryAdd(taskID, manualResetEventSlim);
                }
                if (timeout.HasValue)
                    manualResetEventSlim.Wait(timeout.Value);
                else
                    manualResetEventSlim.Wait();
            }
        }

        #region UserTasks
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

        bool IProcessInstance.WaitForUserTask(string taskID, out IUserTask task)
            => ((IProcessInstance)this).WaitForUserTask(TimeSpan.Zero, taskID, out task);

        bool IProcessInstance.WaitForUserTask(int millisecondsTimeout, string taskID, out IUserTask task)
            => ((IProcessInstance)this).WaitForUserTask(TimeSpan.FromMilliseconds(millisecondsTimeout), taskID, out task);

        bool IProcessInstance.WaitForUserTask(TimeSpan timeout, string taskID, out IUserTask task)
        {
            WaitForTask(taskID, timeout.Equals(TimeSpan.Zero) ? null : timeout);
            task = ((IProcessInstance)this).GetUserTask(taskID);
            return task!=null;
        }
        #endregion

        #region ManualTasks
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

        bool IProcessInstance.WaitForManualTask(string taskID, out IManualTask task)
            => ((IProcessInstance)this).WaitForManualTask(TimeSpan.Zero, taskID, out task);

        bool IProcessInstance.WaitForManualTask(int millisecondsTimeout, string taskID, out IManualTask task)
            => ((IProcessInstance)this).WaitForManualTask(TimeSpan.FromMilliseconds(millisecondsTimeout), taskID, out task);

        bool IProcessInstance.WaitForManualTask(TimeSpan timeout, string taskID, out IManualTask task)
        {
            WaitForTask(taskID, timeout.Equals(TimeSpan.Zero) ? null : timeout);
            task = ((IProcessInstance)this).GetManualTask(taskID);
            return task!=null;
        }
        #endregion

        #region Logging
        internal void WriteLogLine(string elementID, LogLevel level, StackFrame sf, DateTime timestamp, string message)
            => WriteLogLine((IElement)(elementID == null ? null : Process.GetElement(elementID)), level, sf, timestamp, message) ;
        internal void WriteLogLine(IElement element, LogLevel level, StackFrame sf, DateTime timestamp, string message)
        {
            if ((int)level >= (int)stateLogLevel && State!=null)
                State.LogLine(element?.ID, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
            Delegates.Logging.LogLine?.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

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
            => ((IProcessInstance)this).WaitForCompletion(TimeSpan.Zero);
        bool IProcessInstance.WaitForCompletion(int millisecondsTimeout)
            => ((IProcessInstance)this).WaitForCompletion(TimeSpan.FromMilliseconds(millisecondsTimeout));
        bool IProcessInstance.WaitForCompletion(TimeSpan timeout)
        {
            var result = true;
            if (!isComplete)
            {
                if (timeout.Equals(TimeSpan.Zero))
                    ProcessLock.Wait();
                else 
                    result = ProcessLock.Wait(timeout);
                result = result || isComplete;
            }
            return result;
        }
        Dictionary<string, object> IProcessInstance.CurrentVariables
            => State.CurrentState.Variables;
        #endregion

        byte[] IProcessInstance.Diagram(bool outputVariables,ImageFormat type)
            => Process.Diagram(outputVariables, State, type);

        byte[] IProcessInstance.Animate(bool outputVariables)
            => Process.Animate(outputVariables, State);

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (State.ActiveSteps.Any())
                        throw new ActiveStepsException();
                    StepScheduler.Instance.UnloadProcess(this);
                    State.Dispose();
                    processLock?.Dispose();
                    mreSuspend?.Dispose();
                    waitingTasks.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProcessInstance()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
