using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal sealed class ProcessState : IDisposable
    {
        private const string _PROCESS_STATE_ELEMENT = "ProcessState";
        private const string _PROCESS_SUSPENDED_ATTRIBUTE = "isSuspended";

        internal delegate void delTriggerStateChange();

        private class ReadOnlyProcessState : IState
        {
            private readonly bool _isSuspended;
            private readonly IReadOnlyStateVariablesContainer _variables;
            private readonly IReadOnlyStateContainer _path;
            private readonly IReadOnlyStateContainer _log;

            public ReadOnlyProcessState(ProcessState state)
            {
                state._stateEvent.EnterReadLock();
                _isSuspended=state.IsSuspended;
                _variables=(IReadOnlyStateVariablesContainer)state._variables.Clone();
                _path=state._path.Clone();
                _log=state._log.Clone();
                state._stateEvent.ExitReadLock();
            }
            public object this[string name] => _variables[name];

            public IEnumerable<string> Keys => _variables.Keys;

            private IEnumerable<IReadOnlyStateContainer> _Components => new IReadOnlyStateContainer[] {_path,_variables,_log};

            public string AsXMLDocument
            {
                get
                {
                    using(var ms = new MemoryStream())
                    {
                        var writer = XmlWriter.Create(ms);
                        writer.WriteStartDocument();
                        writer.WriteStartElement(_PROCESS_STATE_ELEMENT);
                        writer.WriteAttributeString(_PROCESS_SUSPENDED_ATTRIBUTE, _isSuspended.ToString());

                        _Components.ForEach(comp =>
                        {
                            writer.WriteStartElement(comp.GetType().Name.Replace("ReadOnly",""));
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
            }

            public string AsJSONDocument
            {
                get
                {
                    using (var ms = new MemoryStream())
                    {
                        var writer = new Utf8JsonWriter(ms);
                        writer.WriteStartObject();
                        writer.WritePropertyName(_PROCESS_SUSPENDED_ATTRIBUTE);
                        writer.WriteBooleanValue(_isSuspended);

                        _Components.ForEach(comp =>
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
            }
        }

        private ProcessLog _log;
        private ProcessVariables _variables;
        private ProcessPath _path;
        internal ProcessPath Path { get { return _path; } }
        private ReaderWriterLockSlim _stateEvent = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        internal bool IsSuspended { get; set; }
        private readonly OnStateChange _onStateChange;
        private readonly BusinessProcess _process;
        internal BusinessProcess Process { get { return _process; } }

        internal ProcessState(BusinessProcess process,ProcessStepComplete complete, ProcessStepError error,OnStateChange onStateChange)
        {
            _process = process;
            _log = new ProcessLog(_stateEvent);
            _variables = new ProcessVariables(_stateEvent);
            _path = new ProcessPath(complete, error,process,_stateEvent,new delTriggerStateChange(_stateChanged));
            _onStateChange = onStateChange;
        }

        public void Dispose()
        {
            _log.Dispose();
            _variables.Dispose();
            _path.Dispose();
            _stateEvent.EnterReadLock();
            while (_stateEvent.CurrentReadCount>1)
            {
                System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            }
            _stateEvent.ExitReadLock();
            _stateEvent.EnterWriteLock();
            while (_stateEvent.WaitingWriteCount>0)
            {
                _stateEvent.ExitWriteLock();
                System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                _stateEvent.EnterWriteLock();
            }
            _stateEvent.ExitWriteLock();
            try
            {
                _stateEvent.Dispose();
            }
            catch (Exception) { }
        }

        internal bool Load(XmlDocument doc)
        {
            if (doc.GetElementsByTagName(_PROCESS_STATE_ELEMENT).Count == 0)
                return false;
            var result = true;
            _stateEvent.EnterWriteLock();
            var reader = XmlReader.Create(new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(doc.OuterXml)));
            while(reader.NodeType!=XmlNodeType.Element)
            {
                if(!reader.Read())
                    return false;
            }
            if (reader.NodeType ==XmlNodeType.Element && reader.Name==_PROCESS_STATE_ELEMENT)
            {
                IsSuspended = bool.Parse(reader.GetAttribute(_PROCESS_SUSPENDED_ATTRIBUTE));
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
                                    _log.Load(reader);
                                    break;
                                case "ProcessPath":
                                    _path.Load(reader);
                                    break;
                                case "ProcessVariables":
                                    _variables.Load(reader);
                                    break;
                                default:
                                    throw new Exception("Reading error...");
                                    break;
                            }
                            if (reader.NodeType==XmlNodeType.EndElement)
                                reader.Read();
                        }
                        else
                            reader.Read();
                    }
                }
                catch (Exception e)
                {
                    result=false;
                }
            }
            else
                result=false;
            reader.Close();
            _stateEvent.ExitWriteLock();
            return result;
        }


        internal IEnumerable<sSuspendedStep> ResumeSteps
            => !IsSuspended ? new sSuspendedStep[] { } : _path.ResumeSteps;

        internal IEnumerable<sDelayedStartEvent> DelayedEvents => _path.DelayedEvents;

        internal IEnumerable<string> AbortableSteps => _path.AbortableSteps;

        internal IEnumerable<string> ActiveSteps => _path.ActiveSteps;

        internal void MergeVariables(ITask task, IVariables vars)
        {
            int stepIndex = _path.CurrentStepIndex(task.id);
            _variables.MergeVariables(stepIndex, vars);
        }

        internal object this[string elementID, string variableName]
        {
            get
            {
                object ret = null;
                int stepIndex = -1;
                if (elementID == null)
                    stepIndex = _path.LastStep;
                else
                    stepIndex = _path.CurrentStepIndex(elementID);
                if (elementID!=null && stepIndex==-1)
                    stepIndex=_path.LastStep;
                ret = _variables[variableName, stepIndex];
                return ret;
            }
            set
            {
                _variables[variableName, _path.CurrentStepIndex(elementID)] = value;
            }
        }

        internal IEnumerable<string> this[string elementID]
        {
            get
            {
                IEnumerable<string> result = Array.Empty<string>();
                int stepIndex = -1;
                if (elementID == null)
                    stepIndex = _path.LastStep;
                else
                    stepIndex = _path.CurrentStepIndex(elementID);
                if (elementID!=null && stepIndex==-1)
                    stepIndex=_path.LastStep;
                result = _variables[stepIndex];
                return result;
            }
        }

        internal void SuspendStep(string sourceID,string elementID, TimeSpan span)
        {
            _process.WriteLogLine(elementID, LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Suspending Step for {0}", new object[] { span }));
            _path.SuspendElement(sourceID, elementID, span);
            _stateChanged();
        }

        internal IEnumerable<sStepSuspension> SuspendedSteps
            => _path.SuspendedSteps;

        public IState CurrentState => new ReadOnlyProcessState(this);

        private void _stateChanged()
        {
            if (_onStateChange != null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _onStateChange(CurrentState);
                    }catch(Exception ex)
                    {
                        _process.WriteLogException((string)null, new StackFrame(2, true), DateTime.Now, ex);
                    }
                });
            }
        }

        internal void Suspend()
        {
            _process.WriteLogLine((string)null,LogLevels.Debug,new StackFrame(1,true),DateTime.Now,"Suspending Process State");
            IsSuspended = true;
        }

        internal void Resume(ProcessInstance instance,Action<string,string> processStepComplete,Action<AEvent> completeTimedEvent)
        {
            _process.WriteLogLine((string)null, LogLevels.Debug, new StackFrame(1, true), DateTime.Now, "Resuming Process State");
            var resumes = ResumeSteps.ToArray();
            var suspendedSteps = SuspendedSteps.ToArray();
            var delayedEvents = DelayedEvents.ToArray();
            _stateEvent.EnterWriteLock();
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
                        Utility.Sleep(ss.EndTime.Subtract(DateTime.Now), instance, (AEvent)_process.GetElement(ss.Id));
                    else
                        completeTimedEvent((AEvent)_process.GetElement(ss.Id));
                });
            });
            System.Threading.Tasks.Task.Run(() =>
            {
                delayedEvents.ForEach(sdse =>
                {
                    if (sdse.Delay.Ticks<0)
                        _process.ProcessEvent(instance, sdse.IncomingID, (AEvent)_process.GetElement(sdse.ElementID));
                    else
                        Utility.DelayStart(sdse.Delay, instance, (BoundaryEvent)_process.GetElement(sdse.ElementID), sdse.IncomingID);
                });
            });
            _stateEvent.ExitWriteLock();
            _stateChanged();
        }

        internal void LogLine(string elementID,AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message)
        {
            _log.LogLine(elementID, assembly, fileName, lineNumber, level, timestamp, message);
        }

        internal void LogException(string elementID, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception)
        {
            _log.LogException(elementID, assembly, fileName, lineNumber, timestamp, exception);
        }
    }
}
