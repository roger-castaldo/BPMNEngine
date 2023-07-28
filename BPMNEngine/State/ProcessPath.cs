using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Gateways;
using BPMNEngine.Elements.Processes.Tasks;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using BPMNEngine.Elements;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using SkiaSharp;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.State;

namespace BPMNEngine.State
{
    internal sealed class ProcessPath : IStateContainer
    {
        private readonly struct SPathEntry
        {
            public string ElementID { get; init; }
            public StepStatuses Status { get; init; }
            public DateTime StartTime { get; init; }
            public string IncomingID { get; init; }
            public DateTime? EndTime { get; init; }
            public string CompletedBy { get; init; }
            public IEnumerable<string> OutgoingID { get; init; }
        }

        private class ReadOnlyProcessPath : IReadonlyProcessPathContainer
        {
            private readonly ProcessPath _path;
            private readonly int _stepCount;

            public ReadOnlyProcessPath(ProcessPath path,int stepCount)
            {
                _path = path;
                _stepCount = stepCount;
            }

            public IEnumerable<string> ActiveSteps => Steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.Started || grp.Last().Status==StepStatuses.Waiting)
                    .Select(grp => grp.Key);

            private IEnumerable<SPathEntry> Steps
                => _path.RunQuery<SPathEntry>((IEnumerable<SPathEntry> Steps) =>
                {
                    return Steps.Take(_stepCount);
                });

            public void Append(XmlWriter writer)
            {
                Steps.ForEach(step =>
                {
                    writer.WriteStartElement(_PATH_ENTRY_ELEMENT);
                    writer.WriteAttributeString(_ELEMENT_ID, step.ElementID);
                    writer.WriteAttributeString(_STEP_STATUS, step.Status.ToString());
                    writer.WriteAttributeString(_START_TIME, step.StartTime.ToString(Constants.DATETIME_FORMAT));
                    if (step.IncomingID!=null)
                        writer.WriteAttributeString(_INCOMING_ID, step.IncomingID);
                    if (step.EndTime.HasValue)
                        writer.WriteAttributeString(_END_TIME, step.EndTime.Value.ToString(Constants.DATETIME_FORMAT));
                    if (step.CompletedBy!=null)
                        writer.WriteAttributeString(_COMPLETED_BY, step.CompletedBy);
                    if (step.OutgoingID!=null)
                    {
                        if (step.OutgoingID.Count()==1)
                            writer.WriteAttributeString(_OUTGOING_ID, step.OutgoingID.First());
                        else
                            step.OutgoingID.ForEach(outid =>
                            {
                                writer.WriteStartElement(_OUTGOING_ELEM);
                                writer.WriteValue(outid);
                                writer.WriteEndElement();
                            });
                    }
                    writer.WriteEndElement();
                });
            }

            public void Append(Utf8JsonWriter writer)
            {
                writer.WriteStartArray();
                Steps.ForEach(step =>
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(_ELEMENT_ID);
                    writer.WriteStringValue(step.ElementID);
                    writer.WritePropertyName(_STEP_STATUS);
                    writer.WriteStringValue(step.Status.ToString());
                    writer.WritePropertyName(_STEP_STATUS);
                    writer.WriteStringValue(step.StartTime.ToString(Constants.DATETIME_FORMAT));
                    if (step.IncomingID!=null)
                    {
                        writer.WritePropertyName(_INCOMING_ID);
                        writer.WriteStringValue(step.IncomingID);
                    }
                    if (step.EndTime.HasValue)
                    {
                        writer.WritePropertyName(_END_TIME);
                        writer.WriteStringValue(step.EndTime.Value.ToString(Constants.DATETIME_FORMAT));
                    }
                    if (step.CompletedBy!=null)
                    {
                        writer.WritePropertyName(_COMPLETED_BY);
                        writer.WriteStringValue(step.CompletedBy);
                    }
                    if (step.OutgoingID!=null)
                    {
                        writer.WritePropertyName(_OUTGOING_ID);
                        if (step.OutgoingID.Count()==1)
                            writer.WriteStringValue(step.OutgoingID.First());
                        else
                        {
                            writer.WriteStartArray();
                            step.OutgoingID.ForEach(outid =>
                            {
                                writer.WriteStringValue(outid);
                            });
                            writer.WriteEndArray();
                        }
                    }
                    writer.WriteEndObject();
                });
                writer.WriteEndArray();
            }
        }

        private readonly List<SPathEntry> _steps = new();

        private readonly ProcessStepComplete _complete;
        private readonly ProcessStepError _error;
        internal int LastStep { get; private set; }

        private readonly ReaderWriterLockSlim _stateLock;
        private readonly BusinessProcess _process;
        private readonly ProcessState.delTriggerStateChange _triggerChange;

        public ProcessPath(ProcessStepComplete complete, ProcessStepError error, BusinessProcess process,ReaderWriterLockSlim stateLock, ProcessState.delTriggerStateChange triggerChange)
        {
            _complete = complete;
            _error = error;
            LastStep = int.MaxValue;
            _process=process;
            _stateLock=stateLock;
            _triggerChange=triggerChange;
        }

        #region IStateContainer
        private const string _PATH_ENTRY_ELEMENT = "sPathEntry";
        private const string _STEP_STATUS = "status";
        private const string _ELEMENT_ID = "elementID";
        private const string _START_TIME = "startTime";
        private const string _END_TIME = "endTime";
        private const string _OUTGOING_ID = "outgoingID";
        private const string _INCOMING_ID = "incomingID";
        private const string _OUTGOING_ELEM = "outgoing";
        private const string _COMPLETED_BY = "CompletedByID";

        public void Load(XmlReader reader)
        {
            _steps.Clear();
            reader.Read();
            while (reader.NodeType==XmlNodeType.Element && reader.Name==_PATH_ENTRY_ELEMENT)
            {
                var elementID = reader.GetAttribute(_ELEMENT_ID);
                var stepStatus = (StepStatuses)Enum.Parse(typeof(StepStatuses), reader.GetAttribute(_STEP_STATUS));
                var startTime = DateTime.ParseExact(reader.GetAttribute(_START_TIME), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
                var incomingID = reader.GetAttribute(_INCOMING_ID);
                var endTime = (reader.GetAttribute(_END_TIME)==null ? (DateTime?)null : DateTime.ParseExact(reader.GetAttribute(_END_TIME), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture));
                IEnumerable<string> outgoing;
                if (reader.GetAttribute(_OUTGOING_ID)==null)
                {
                    outgoing = new List<string>();
                    reader.Read();
                    while (reader.NodeType==XmlNodeType.Element && reader.Name==_OUTGOING_ELEM)
                    {
                        reader.Read();
                        ((List<string>)outgoing).Add(reader.Value);
                        reader.Read();
                        if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_OUTGOING_ELEM)
                            reader.Read();
                    }
                    if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_PATH_ENTRY_ELEMENT)
                        reader.Read();
                }
                else
                {
                    outgoing = new string[] { reader.GetAttribute(_OUTGOING_ID) };
                    reader.Read();
                }
                _steps.Add(new SPathEntry()
                {
                    ElementID=elementID,
                    Status=stepStatus,
                    StartTime=startTime,
                    EndTime=endTime,
                    IncomingID=incomingID,
                    OutgoingID=outgoing
                });
            }
        }

        public void Load(Utf8JsonReader reader)
        {
            _steps.Clear();
            reader.Read();
            while (reader.TokenType!=JsonTokenType.EndArray)
            {
                if (reader.TokenType==JsonTokenType.StartObject)
                {
                    var elementID = string.Empty;
                    var stepStatus = StepStatuses.NotRun;
                    var startTime = DateTime.MinValue;
                    var incomingID = String.Empty;
                    var endTime = (DateTime?)null;
                    IEnumerable<string> outgoing = null;

                    while (reader.TokenType!=JsonTokenType.EndObject)
                    {
                        reader.Read();
                        var propName = reader.GetString();
                        reader.Read();
                        switch (propName)
                        {
                            case _ELEMENT_ID:
                                elementID=reader.GetString();
                                break;
                            case _STEP_STATUS:
                                stepStatus = (StepStatuses)Enum.Parse(typeof(StepStatuses), reader.GetString());
                                break;
                            case _INCOMING_ID:
                                incomingID=reader.GetString();
                                break;
                            case _START_TIME:
                                startTime = DateTime.ParseExact(reader.GetString(), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
                                break;
                            case _END_TIME:
                                endTime = DateTime.ParseExact(reader.GetString(), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
                                break;
                            case _OUTGOING_ID:
                                outgoing = new List<string>();
                                reader.Read();
                                while (reader.TokenType!=JsonTokenType.EndArray)
                                {
                                    ((List<string>)outgoing).Add(reader.GetString());
                                    reader.Read();
                                }
                                break;
                        }
                    }

                    _steps.Add(new SPathEntry()
                    {
                        ElementID=elementID,
                        Status=stepStatus,
                        StartTime=startTime,
                        EndTime=endTime,
                        IncomingID=incomingID,
                        OutgoingID=outgoing
                    });
                }
                reader.Read();
            }
            if (reader.TokenType==JsonTokenType.EndArray)
                reader.Read();
        }

        private IEnumerable<T> RunQuery<T>(Func<IEnumerable<SPathEntry>,IEnumerable<T>> filter)
        {
            _stateLock.EnterReadLock();
            var results = filter(_steps).ToImmutableList();
            _stateLock.ExitReadLock();
            return results;
        }

        public IReadOnlyStateContainer Clone()
        {
            return new ReadOnlyProcessPath(this, _steps.Count);
        }

        public void Dispose()
        {
            _steps.Clear();
        }
        #endregion

        internal IEnumerable<sSuspendedStep> ResumeSteps
            => RunQuery<sSuspendedStep>((IEnumerable<SPathEntry> steps)=> {
                return steps
                            .GroupBy(step => step.ElementID)
                            .Where(grp => grp.Last().Status==StepStatuses.Suspended && !grp.Last().EndTime.HasValue)
                            .Select(grp => new sSuspendedStep()
                            {
                                IncomingID=grp.Last().IncomingID,
                                ElementID=grp.Last().ElementID
                            });
            });

        internal IEnumerable<sDelayedStartEvent> DelayedEvents
        => RunQuery<sDelayedStartEvent>((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.WaitingStart)
                    .Select(grp => new sDelayedStartEvent()
                    {
                        IncomingID=grp.Last().IncomingID,
                        ElementID=grp.Last().ElementID,
                        StartTime=grp.Last().EndTime.Value
                    });
        });

        public IEnumerable<string> AbortableSteps
        => RunQuery<string>((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.WaitingStart
                        || grp.Last().Status==StepStatuses.Waiting
                        || grp.Last().Status==StepStatuses.Started
                        || (grp.Last().Status==StepStatuses.Suspended && grp.Last().EndTime.HasValue))
                    .Select(grp => grp.Key);
        });

        public IEnumerable<string> ActiveSteps
        => RunQuery<string>((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.Started)
                    .Select(grp => grp.Key);
        });

        public IEnumerable<sStepSuspension> SuspendedSteps
        => RunQuery<sStepSuspension>((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                    .Select((step, index) => new { step, index })
                    .GroupBy(step => step.step.ElementID)
                    .Where(grp => grp.Last().step.Status==StepStatuses.Suspended && grp.Last().step.EndTime.HasValue)
                    .Select(grp => new sStepSuspension()
                    {
                        EndTime=grp.Last().step.EndTime.Value,
                        Id=grp.Key,
                        StepIndex=grp.Last().index
                    });
        });

        public StepStatuses GetStatus(string elementid)
        {
            return RunQuery<StepStatuses>((IEnumerable<SPathEntry> steps) =>
            {
                return steps
                    .Take((LastStep==int.MaxValue ? _steps.Count : LastStep+1))
                    .Where(step => step.ElementID==elementid)
                    .Select(step => step.Status)
                    .DefaultIfEmpty(StepStatuses.NotRun);
            }).LastOrDefault();
        }

        public int GetStepSuccessCount(string elementid)
        {
            _stateLock.EnterReadLock();
            var result = _steps
                .Count(step => step.ElementID==elementid && step.Status == StepStatuses.Succeeded);
            _stateLock.ExitReadLock();
            return result;
        }

        public int CurrentStepIndex(string elementid)
        {
            _stateLock.EnterReadLock();
            var result = _steps
                .Any(step=>step.ElementID==elementid)
                ? _steps
                    .Select((step, index) => new { step, index })
                    .Where(pair=>pair.step.ElementID== elementid)
                    .Max(pair=>pair.index)
                : -1;
            _stateLock.ExitReadLock();
            return result;
        }

        public bool ProcessGateway(AGateway gw,string sourceID)
        {
            var result = true;
            var changed = false;
            _stateLock.EnterWriteLock();
            if (gw is ParallelGateway)
            {
                if (gw.Incoming.Count()>1)
                {
                    if (_steps
                        .Take((LastStep==int.MaxValue ? _steps.Count : LastStep+1))
                        .Where(step => step.ElementID==gw.ID)
                        .Select(step => step.Status)
                        .DefaultIfEmpty(StepStatuses.NotRun)
                        .FirstOrDefault()!=StepStatuses.Waiting)
                        result = false;
                    else
                    {
                        var counts = gw.Incoming
                            .Select(i =>
                                _steps
                                .Count(step => step.ElementID==i && step.Status == StepStatuses.Succeeded)
                             );
                        if (counts.Any(c => c!=counts.Max()))
                            result=false;
                    }
                    if (!result && _steps
                            .Take((LastStep==int.MaxValue ? _steps.Count : LastStep+1))
                            .Where(step => step.ElementID==gw.ID)
                            .Select(step => step.Status)
                            .DefaultIfEmpty(StepStatuses.NotRun)
                            .FirstOrDefault()!=StepStatuses.Waiting)
                    {
                        _steps.Add(new SPathEntry()
                        {
                            ElementID=gw.ID,
                            Status = StepStatuses.Waiting,
                            StartTime = DateTime.Now,
                            IncomingID=sourceID
                        });
                        changed=true;
                    }
                }
            }
            else if (gw is ExclusiveGateway)
            {
                if (gw.Incoming.Count()>1)
                {
                    var currentStatus = _steps
                        .Take((LastStep==int.MaxValue ? _steps.Count : LastStep+1))
                        .Where(step => step.ElementID==gw.ID)
                        .Select(step => step.Status)
                        .DefaultIfEmpty(StepStatuses.NotRun)
                        .FirstOrDefault();
                    if (currentStatus==StepStatuses.Started)
                        result=false;
                    else if (currentStatus==StepStatuses.Succeeded)
                    {
                        var counts = gw.Incoming
                            .Select(
                                i => _steps
                                    .Count(step => step.ElementID==i && step.Status == StepStatuses.Succeeded)
                             );
                        result = counts.Count(c => c==counts.Max())==1;
                    }
                }
            }
            if (result)
            {
                _steps.Add(new SPathEntry()
                {
                    ElementID=gw.ID,
                    Status = StepStatuses.Started,
                    StartTime = DateTime.Now,
                    IncomingID=sourceID
                });
                changed=true;
            }
            _stateLock.ExitWriteLock();
            if (changed)
                _triggerChange();
            return result;
        }

        internal void StartAnimation()
        {
            LastStep = -1;
        }

        internal string MoveToNextStep()
        {
            LastStep++;
            _stateLock.EnterReadLock();
            var result =  ((LastStep==0 || (LastStep>_steps.Count)) ? null : _steps.Skip(LastStep-1).First().ElementID);
            _stateLock.ExitReadLock();
            return result;
        }

        internal bool HasNext()
        {
            _stateLock.EnterReadLock();
            var result = _steps.Count+1>LastStep;
            _stateLock.ExitReadLock();
            return result;
        }

        internal void FinishAnimation()
        {
            LastStep = int.MaxValue;
        }

        private void AddPathEntry(string elementID, StepStatuses status, DateTime start, string incomingID = null, IEnumerable<string> outgoingID = null, DateTime? end = null, string completedBy = null)
        {
            _stateLock.EnterWriteLock();
            if (_steps.Any(step => step.ElementID==elementID))
            {
                var lastStep = _steps.Last(step=>step.ElementID==elementID);
                if (lastStep.Status==StepStatuses.WaitingStart)
                    start=lastStep.StartTime;
            }
            _steps.Add(new SPathEntry()
            {
                ElementID=elementID,
                IncomingID=incomingID,
                OutgoingID=outgoingID,
                Status=status,
                StartTime=start,
                EndTime=end,
                CompletedBy=completedBy
            });
            _stateLock.ExitWriteLock();
            _triggerChange();
        }

        private void GetIncomingIDAndStart(string elementID,out DateTime start,out string incoming)
        {
            start = DateTime.Now;
            incoming = null;
            _stateLock.EnterReadLock();
            var result = _steps
                .Where(step => step.ElementID == elementID && (step.Status == StepStatuses.Waiting||step.Status==StepStatuses.Started))
                .Select(step => new { incoming = step.IncomingID, start = step.StartTime })
                .LastOrDefault();
            _stateLock.ExitReadLock();
            if (result!=null)
            {
                start=result.start;
                incoming=result.incoming;
            }
        }

        private void WriteLogLine(string elementID, LogLevel level, string message)
        {
            _process.WriteLogLine(elementID, level, new System.Diagnostics.StackFrame(1, true), DateTime.Now, message);
        }

        internal void DelayEventStart(AEvent Event,string incoming,TimeSpan delay)
        {
            WriteLogLine(Event.ID, LogLevel.Debug, "Delaying start of event in Process Path");
            AddPathEntry(Event.ID, StepStatuses.WaitingStart, DateTime.Now,incomingID:incoming,end:DateTime.Now.Add(delay));
        }

        internal void StartFlowNode(AFlowNode node,string incoming)
        {
            WriteLogLine(node.ID, LogLevel.Debug, string.Format("Starting {0} in Process Path", node.GetType().Name));
            AddPathEntry(node.ID, (node is UserTask || node is ManualTask ? StepStatuses.Waiting : StepStatuses.Started),DateTime.Now,incomingID:incoming);
        }

        internal void SucceedFlowNode(ATask task, IEnumerable<string> outgoing = null, string completedByID = null)
        {
            SucceedFlowNode((AFlowNode)task,outgoing:outgoing??task.Outgoing,completedByID:completedByID);
        }

        internal void SucceedFlowNode(AEvent evnt, IEnumerable<string> outgoing = null, string completedByID = null)
        {
            if (evnt is BoundaryEvent @event)
                SucceedFlowNode((AFlowNode)evnt, outgoing: outgoing??@event.Outgoing, completedByID: completedByID);
            else
                SucceedFlowNode((AFlowNode)evnt, outgoing: outgoing??evnt.Outgoing, completedByID: completedByID);
        }

        internal void SucceedFlowNode(AFlowNode node,IEnumerable<string> outgoing=null, string completedByID=null)
        {
            WriteLogLine(node.ID, LogLevel.Debug, string.Format("Succeeding {0} in Process Path {1}",node.GetType().Name,(completedByID==null ? "" : string.Format(" as completed by {0}",completedByID))));
            GetIncomingIDAndStart(node.ID, out DateTime start, out string incoming);
            outgoing ??= node.Outgoing;
            if (!outgoing.Any())
            {
                AddPathEntry(node.ID,StepStatuses.Succeeded,start,incomingID:incoming,end:DateTime.Now,completedBy:completedByID);
                Complete(node.ID, null);
            }
            else
            {
                AddPathEntry(node.ID, StepStatuses.Succeeded, start, incomingID: incoming, end: DateTime.Now,outgoingID:node.Outgoing,completedBy:completedByID);
                outgoing.Distinct().ForEach(id => Complete(node.ID, id));
            }
        }

        internal void FailFlowNode(AFlowNode node,Exception error=null)
        {
            WriteLogLine(node.ID, LogLevel.Debug, string.Format("Failing {0} in Process Path", node.GetType().Name));
            GetIncomingIDAndStart(node.ID, out DateTime start, out string incoming);
            AddPathEntry(node.ID,StepStatuses.Failed,start,incomingID:incoming, end:DateTime.Now);
            Error(node, error);
        }

        private async void Complete(string incoming, string outgoing) => await System.Threading.Tasks.Task.Run(() => _complete.Invoke(incoming, outgoing));

        private async void Error(IElement step, Exception ex) => await System.Threading.Tasks.Task.Run(() => _error.Invoke(step,ex));

        internal void ProcessFlowElement(IFlowElement flowElement)
        {
            WriteLogLine(flowElement.ID, LogLevel.Debug, "Processing Flow Element in Process Path");
            AddPathEntry(flowElement.ID,StepStatuses.Succeeded,DateTime.Now,incomingID:flowElement.sourceRef, outgoingID:new string[] { flowElement.targetRef }, end:DateTime.Now);
            Complete(flowElement.ID, flowElement.targetRef);
        }

        internal void SuspendElement(string sourceID, IElement elem)
        {
            WriteLogLine(elem.ID, LogLevel.Debug, "Suspending Element in Process Path");
            AddPathEntry(elem.ID,StepStatuses.Suspended,DateTime.Now,incomingID:sourceID);
        }

        internal void SuspendElement(string sourceID, string elementID,TimeSpan span)
        {
            WriteLogLine(elementID, LogLevel.Debug, "Suspending Element in Process Path");
            AddPathEntry(elementID, StepStatuses.Suspended, DateTime.Now, incomingID: sourceID,end:DateTime.Now.Add(span));
        }

        internal void AbortStep(string sourceID,string attachedToID)
        {
            WriteLogLine(attachedToID, LogLevel.Debug, "Aborting Process Step");
            AddPathEntry(attachedToID, StepStatuses.Aborted, DateTime.Now, incomingID: sourceID);

        }
    }
}
