using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.State;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.Scheduling;
using BPMNEngine.State;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;

namespace BPMNEngine
{
    internal sealed class ProcessState : IDisposable
    {
        private const string PROCESS_STATE_ELEMENT = "ProcessState";
        private const string PROCESS_SUSPENDED_ATTRIBUTE = "isSuspended";

        internal delegate void delTriggerStateChange();

        private class ReadOnlyProcessState : IInternalState
        {
            private readonly bool isSuspended;
            private readonly IReadOnlyStateVariablesContainer variables;
            private readonly IReadonlyProcessPathContainer path;
            private readonly IReadonlyStateLogContainer log;
            private readonly BusinessProcess process;

            public ReadOnlyProcessState(ProcessState state)
            {
                state.stateEvent.EnterReadLock();
                isSuspended=state.IsSuspended;
                variables=(IReadOnlyStateVariablesContainer)state.variables.Clone();
                path=(IReadonlyProcessPathContainer)state.Path.Clone();
                log=(IReadonlyStateLogContainer)state.log.Clone();
                process=state.Process;
                state.stateEvent.ExitReadLock();
            }
            public object this[string name] => variables[name];

            public IEnumerable<string> Keys => variables.Keys;

            private IEnumerable<IReadOnlyStateContainer> Components => new IReadOnlyStateContainer[] {path,variables,log};

            public string AsXMLDocument
            {
                get
                {
                    using var ms = new MemoryStream();
                    var writer = XmlWriter.Create(ms, new XmlWriterSettings()
                    {
                        Indent = true
                    });
                    writer.WriteStartDocument();
                    writer.WriteStartElement(PROCESS_STATE_ELEMENT);
                    writer.WriteAttributeString(PROCESS_SUSPENDED_ATTRIBUTE, isSuspended.ToString());

                    Components.ForEach(comp =>
                    {
                        writer.WriteStartElement(comp.GetType().Name.Replace("ReadOnly", ""));
                        comp.Append(writer);
                        writer.WriteEndElement();
                    });

                    writer.WriteEndElement();
                    writer.Flush();

                    ms.Position=0;
                    var result = new StreamReader(ms).ReadToEnd();
                    return result;
                }
            }

            public string AsJSONDocument
            {
                get
                {
                    using var ms = new MemoryStream();
                    var writer = new Utf8JsonWriter(ms);
                    writer.WriteStartObject();
                    writer.WritePropertyName(PROCESS_SUSPENDED_ATTRIBUTE);
                    writer.WriteBooleanValue(isSuspended);

                    Components.ForEach(comp =>
                    {
                        writer.WritePropertyName(comp.GetType().Name.Replace("ReadOnly", ""));
                        comp.Append(writer);
                    });

                    writer.WriteEndObject();
                    writer.Flush();

                    ms.Position=0;
                    var result = new StreamReader(ms).ReadToEnd();
                    return result;
                }
            }

            public IEnumerable<IElement> ActiveElements
                => (process==null 
                    ? throw new NotSupportedException("Unable to access Active Elements when loaded without a business process") 
                    :  path.ActiveSteps.Select(id => process.GetElement(id))
                );

            public IEnumerable<IStateStep> Steps => path.Steps;

            public string Log => log.Log;

            public Dictionary<string, object> Variables => variables.AsExtract;
        }

        private readonly ProcessLog log;
        private readonly ProcessVariables variables;
        private readonly ReaderWriterLockSlim stateEvent = new(LockRecursionPolicy.NoRecursion);
        private readonly OnStateChange onStateChange;
        internal ProcessPath Path { get; private init; }
        internal bool IsSuspended { get; set; }
        internal BusinessProcess Process { get; private init; }

        internal ProcessState(BusinessProcess process,ProcessStepComplete complete, ProcessStepError error,OnStateChange onStateChange)
        {
            Process = process;
            log = new ProcessLog(stateEvent);
            variables = new ProcessVariables(stateEvent);
            Path = new ProcessPath(complete, error,process,stateEvent,new delTriggerStateChange(StateChanged));
            this.onStateChange = onStateChange;
        }

        private ProcessState(int? stepIndex=null)
        {
            log = new ProcessLog(stateEvent);
            variables = new ProcessVariables(stateEvent,stepIndex:stepIndex);
            Path = new ProcessPath(null, null, null, stateEvent, new delTriggerStateChange(StateChanged));
        }

        public void Dispose()
        {
            log.Dispose();
            variables.Dispose();
            Path.Dispose();
            stateEvent.EnterReadLock();
            while (stateEvent.CurrentReadCount>1)
                System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            stateEvent.ExitReadLock();
            stateEvent.EnterWriteLock();
            while (stateEvent.WaitingWriteCount>0)
            {
                stateEvent.ExitWriteLock();
                System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                stateEvent.EnterWriteLock();
            }
            stateEvent.ExitWriteLock();
            try
            {
                stateEvent.Dispose();
            }
            catch (Exception) { }
        }

        internal bool Load(XmlDocument doc)
        {
            if (doc.GetElementsByTagName(PROCESS_STATE_ELEMENT).Count == 0)
                return false;
            var result = true;
            stateEvent.EnterWriteLock();
            var reader = XmlReader.Create(new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(doc.OuterXml)));
            while(reader.NodeType!=XmlNodeType.Element)
            {
                if(!reader.Read())
                    return false;
            }
            if (reader.NodeType ==XmlNodeType.Element && reader.Name==PROCESS_STATE_ELEMENT)
            {
                IsSuspended = bool.Parse(reader.GetAttribute(PROCESS_SUSPENDED_ATTRIBUTE));
                reader.Read();
                try
                {
                    while(!reader.EOF)
                    {
                        if (reader.NodeType==XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "ProcessLog":
                                    log.Load(reader);
                                    break;
                                case "ProcessPath":
                                    Path.Load(reader);
                                    break;
                                case "ProcessVariables":
                                    variables.Load(reader);
                                    break;
                                default:
                                    throw new Exception("Reading error...");
                            }
                            if (reader.NodeType==XmlNodeType.EndElement)
                                reader.Read();
                        }
                        else
                            reader.Read();
                    }
                }
                catch (Exception)
                {
                    result=false;
                }
            }
            else
                result=false;
            reader.Close();
            stateEvent.ExitWriteLock();
            return result;
        }

        internal bool Load(Utf8JsonReader reader)
        {
            stateEvent.EnterWriteLock();
            try
            {
                reader.Read();
                while (reader.Read())
                {
                    if (reader.TokenType==JsonTokenType.PropertyName)
                    {
                        switch (reader.GetString())
                        {
                            case PROCESS_SUSPENDED_ATTRIBUTE:
                                reader.Read();
                                IsSuspended = reader.GetBoolean();
                                break;
                            case "ProcessLog":
                                log.Load(reader);
                                break;
                            case "ProcessPath":
                                Path.Load(reader);
                                break;
                            case "ProcessVariables":
                                variables.Load(reader);
                                break;
                        }
                    }
                }
            }
            catch (Exception) { return false; }
            stateEvent.ExitWriteLock();
            return true;
        }

        public static IInternalState LoadState(XmlDocument doc,int? stepIndex=null)
        {
            var state = new ProcessState(stepIndex:stepIndex);
            if (state.Load(doc))
                return new ReadOnlyProcessState(state);
            return null;
        }

        public static IInternalState LoadState(Utf8JsonReader reader, int? stepIndex=null)
        {
            var state = new ProcessState(stepIndex:stepIndex);
            if (state.Load(reader))
                return new ReadOnlyProcessState(state);
            return null;
        }

        internal IEnumerable<SSuspendedStep> ResumeSteps
            => !IsSuspended ? Array.Empty<SSuspendedStep>() : Path.ResumeSteps;

        internal IEnumerable<SDelayedStartEvent> DelayedEvents 
            => Path.DelayedEvents;

        internal IEnumerable<string> AbortableSteps 
            => Path.AbortableSteps;

        internal IEnumerable<string> ActiveSteps 
            => Path.ActiveSteps;

        internal void MergeVariables(ITask task, IVariables vars)
        {
            int stepIndex = Path.CurrentStepIndex(task.ID);
            variables.MergeVariables(stepIndex, vars);
        }

        internal object this[string elementID, string variableName]
        {
            get
            {
                int stepIndex;
                if (elementID == null)
                    stepIndex = Path.LastStep;
                else
                    stepIndex = Path.CurrentStepIndex(elementID);
                if (elementID!=null && stepIndex==-1)
                    stepIndex=Path.LastStep;
                object ret = variables[variableName, stepIndex];
                return ret;
            }
            set
            {
                variables[variableName, Path.CurrentStepIndex(elementID)] = value;
            }
        }

        internal IEnumerable<string> this[string elementID]
        {
            get
            {
                _ = Array.Empty<string>();
                int stepIndex;
                if (elementID == null)
                    stepIndex = Path.LastStep;
                else
                    stepIndex = Path.CurrentStepIndex(elementID);
                if (elementID!=null && stepIndex==-1)
                    stepIndex=Path.LastStep;
                IEnumerable<string> result = variables[stepIndex];
                return result;
            }
        }

        internal void SuspendStep(string sourceID,string elementID, TimeSpan span)
        {
            Process.WriteLogLine(elementID, LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Suspending Step for {0}", new object[] { span }));
            Path.SuspendElement(sourceID, elementID, span);
            StateChanged();
        }

        internal IEnumerable<SStepSuspension> SuspendedSteps
            => Path.SuspendedSteps;

        public IInternalState CurrentState 
            => new ReadOnlyProcessState(this);

        private void StateChanged()
        {
            if (onStateChange != null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        onStateChange(CurrentState);
                    }catch(Exception ex)
                    {
                        Process.WriteLogException((string)null, new StackFrame(2, true), DateTime.Now, ex);
                    }
                });
            }
        }

        internal void Suspend()
        {
            Process.WriteLogLine((string)null,LogLevel.Debug,new StackFrame(1,true),DateTime.Now,"Suspending Process State");
            IsSuspended = true;
        }

        internal void Resume(ProcessInstance instance,Action<string,string> processStepComplete,Action<AEvent> completeTimedEvent)
        {
            Process.WriteLogLine((string)null, LogLevel.Debug, new StackFrame(1, true), DateTime.Now, "Resuming Process State");
            var resumes = ResumeSteps.ToArray();
            var suspendedSteps = SuspendedSteps.ToArray();
            var delayedEvents = DelayedEvents.ToArray();
            stateEvent.EnterWriteLock();
            IsSuspended = false;
            System.Threading.Tasks.Task.Run(() =>
            {
                resumes.ForEach(ss => processStepComplete(ss.IncomingID, ss.ElementID));
            });
            System.Threading.Tasks.Task.Run(() =>
            {
                suspendedSteps.ForEach(ss =>
                {
                    if (DateTime.Now.Ticks < ss.EndTime.Ticks)
                        StepScheduler.Instance.Sleep(ss.EndTime.Subtract(DateTime.Now), instance, (AEvent)Process.GetElement(ss.ID));
                    else
                        completeTimedEvent((AEvent)Process.GetElement(ss.ID));
                });
            });
            System.Threading.Tasks.Task.Run(() =>
            {
                delayedEvents.ForEach(sdse =>
                {
                    if (sdse.Delay.Ticks<0)
                        Process.ProcessEvent(instance, sdse.IncomingID, (AEvent)Process.GetElement(sdse.ElementID));
                    else
                        StepScheduler.Instance.DelayStart(sdse.Delay, instance, (BoundaryEvent)Process.GetElement(sdse.ElementID), sdse.IncomingID);
                });
            });
            stateEvent.ExitWriteLock();
            StateChanged();
        }

        internal void LogLine(string elementID,AssemblyName assembly, string fileName, int lineNumber, LogLevel level, DateTime timestamp, string message)
            => log.LogLine(elementID, assembly, fileName, lineNumber, level, timestamp, message);

        internal void LogException(string elementID, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception)
            => log.LogException(elementID, assembly, fileName, lineNumber, timestamp, exception);
    }
}
