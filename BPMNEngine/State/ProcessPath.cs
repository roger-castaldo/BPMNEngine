using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Gateways;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.State;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;

namespace BPMNEngine.State
{
    internal sealed class ProcessPath : IStateContainer
    {
        private record SPathEntry 
            : IStateStep
        {
            public string ElementID { get; init; }
            public StepStatuses Status { get; init; }
            public DateTime StartTime { get; init; }
            public string IncomingID { get; init; }
            public DateTime? EndTime { get; init; }
            public string CompletedBy { get; init; }
            public IImmutableList<string> OutgoingID { get; init; }
        }

        private class ReadOnlyProcessPath : IReadonlyProcessPathContainer
        {
            private readonly ProcessPath path;
            private readonly int stepCount;
            public ReadOnlyProcessPath(ProcessPath path,int stepCount)
            {
                this.path= path;
                this.stepCount= stepCount;
            }

            public IImmutableList<string> ActiveSteps
                => Steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.Started || grp.Last().Status==StepStatuses.Waiting)
                    .Select(grp => grp.Key)
                    .ToImmutableList();

            public IImmutableList<IStateStep> Steps 
                => path.RunQuery<IStateStep>((IEnumerable<SPathEntry> Steps) 
                    =>Steps.Take(stepCount).Cast<IStateStep>()
                ).ToImmutableList();

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
                    if (!string.IsNullOrEmpty(step.CompletedBy))
                        writer.WriteAttributeString(_COMPLETED_BY, step.CompletedBy);
                    if (step.OutgoingID!=null)
                    {
                        if (step.OutgoingID.Count==1)
                            writer.WriteAttributeString(_OUTGOING_ID, step.OutgoingID[0]);
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
                    writer.WritePropertyName(_START_TIME);
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
                    if (!string.IsNullOrEmpty(step.CompletedBy))
                    {
                        writer.WritePropertyName(_COMPLETED_BY);
                        writer.WriteStringValue(step.CompletedBy);
                    }
                    if (step.OutgoingID!=null)
                    {
                        writer.WritePropertyName(_OUTGOING_ID);
                        writer.WriteStartArray();
                        step.OutgoingID.ForEach(outid =>
                        {
                            writer.WriteStringValue(outid);
                        });
                        writer.WriteEndArray();
                    }
                    writer.WriteEndObject();
                });
                writer.WriteEndArray();
            }
        }

        private readonly List<SPathEntry> steps = new();

        private readonly ProcessStepComplete complete;
        private readonly ProcessStepError error;
        internal int LastStep { get; private set; }

        private readonly StateLock stateLock;
        private readonly BusinessProcess process;
        private readonly ProcessState.delTriggerStateChange triggerChange;

        public ProcessPath(ProcessStepComplete complete, ProcessStepError error, BusinessProcess process, StateLock stateLock, ProcessState.delTriggerStateChange triggerChange)
        {
            this.complete = complete;
            this.error = error;
            LastStep = int.MaxValue;
            this.process=process;
            this.stateLock=stateLock;
            this.triggerChange=triggerChange;
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
            steps.Clear();
            reader.Read();
            while (reader.NodeType==XmlNodeType.Element && reader.Name==_PATH_ENTRY_ELEMENT)
            {
                var elementID = reader.GetAttribute(_ELEMENT_ID);
                var stepStatus = (StepStatuses)Enum.Parse(typeof(StepStatuses), reader.GetAttribute(_STEP_STATUS));
                var startTime = DateTime.ParseExact(reader.GetAttribute(_START_TIME), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
                var incomingID = reader.GetAttribute(_INCOMING_ID);
                var endTime = (reader.GetAttribute(_END_TIME)==null ? (DateTime?)null : DateTime.ParseExact(reader.GetAttribute(_END_TIME), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture));
                var completedBy = reader.GetAttribute(_COMPLETED_BY);
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
                steps.Add(new()
                {
                    ElementID=elementID,
                    Status=stepStatus,
                    StartTime=startTime,
                    IncomingID=incomingID,
                    EndTime=endTime,
                    CompletedBy=completedBy,
                    OutgoingID=outgoing.ToImmutableArray()
                });
            }
        }

        public void Load(Utf8JsonReader reader)
        {
            steps.Clear();
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
                    string completedBy = null;
                    IEnumerable<string> outgoing = null;

                    reader.Read();

                    while (reader.TokenType!=JsonTokenType.EndObject)
                    {
                        var propName = reader.GetString();
                        reader.Read();
                        switch (propName)
                        {
                            case _ELEMENT_ID:
                                elementID=reader.GetString();
                                break;
                            case _STEP_STATUS:
                                stepStatus = Enum.Parse<StepStatuses>(reader.GetString());
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
                            case _COMPLETED_BY:
                                completedBy=reader.GetString();
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
                        reader.Read();
                    }

                    steps.Add(new()
                    {
                        ElementID=elementID,
                        Status=stepStatus,
                        StartTime=startTime,
                        IncomingID=incomingID,
                        EndTime=endTime,
                        CompletedBy=completedBy,
                        OutgoingID=outgoing?.ToImmutableArray()??null
                    });
                }
                reader.Read();
            }
            if (reader.TokenType==JsonTokenType.EndArray)
                reader.Read();
        }

        private IEnumerable<T> RunQuery<T>(Func<IEnumerable<SPathEntry>,IEnumerable<T>> filter)
        {
            stateLock.EnterReadLock();
            var results = filter(steps).ToImmutableArray();
            stateLock.ExitReadLock();
            return results;
        }

        public IReadOnlyStateContainer Clone()
        {
            return new ReadOnlyProcessPath(this, steps.Count);
        }

        public void Dispose()
        {
            steps.Clear();
        }
        #endregion

        internal IEnumerable<SSuspendedStep> ResumeSteps
            => RunQuery((IEnumerable<SPathEntry> steps)=> {
                return steps
                            .GroupBy(step => step.ElementID)
                            .Where(grp => grp.Last().Status==StepStatuses.Suspended && !grp.Last().EndTime.HasValue)
                            .Select(grp => new SSuspendedStep()
                            {
                                IncomingID=grp.Last().IncomingID,
                                ElementID=grp.Last().ElementID
                            });
            });

        internal IEnumerable<SDelayedStartEvent> DelayedEvents
        => RunQuery((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.WaitingStart)
                    .Select(grp => new SDelayedStartEvent()
                    {
                        IncomingID=grp.Last().IncomingID,
                        ElementID=grp.Last().ElementID,
                        StartTime=grp.Last().EndTime.Value
                    });
        });

        public IEnumerable<string> AbortableSteps
        => RunQuery((IEnumerable<SPathEntry> steps) =>
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
        => RunQuery((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.Started)
                    .Select(grp => grp.Key);
        });

        public IEnumerable<string> WaitingSteps
        => RunQuery((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.Started
                    || grp.Last().Status==StepStatuses.Waiting)
                    .Select(grp => grp.Key);
        });

        public IEnumerable<SStepSuspension> SuspendedSteps
        => RunQuery((IEnumerable<SPathEntry> steps) =>
        {
            return steps
                    .Select((step, index) => new { step, index })
                    .GroupBy(step => step.step.ElementID)
                    .Where(grp => grp.Last().step.Status==StepStatuses.Suspended && grp.Last().step.EndTime.HasValue)
                    .Select(grp => new SStepSuspension()
                    {
                        EndTime=grp.Last().step.EndTime.Value,
                        ID=grp.Key
                    });
        });

        public StepStatuses GetStatus(string elementid)
        {
            return RunQuery((IEnumerable<SPathEntry> steps) =>
            {
                return steps
                    .Take(LastStep==int.MaxValue ? steps.Count() : LastStep+1)
                    .Where(step => step.ElementID==elementid)
                    .Select(step => step.Status)
                    .DefaultIfEmpty(StepStatuses.NotRun);
            }).LastOrDefault();
        }

        public int CurrentStepIndex(string elementid)
        {
            var results = RunQuery<int>((IEnumerable<SPathEntry> steps) =>
            {
                return steps
                    .Select((step, index) => new { step, index })
                    .Where(pair => pair.step.ElementID== elementid)
                    .Select(step => step.index);
            });
            return (results.Any() ? results.Max() : -1);
        }

        public bool ProcessGateway(AGateway gw,string sourceID)
        {
            var result = true;
            var changed = false;
            stateLock.EnterWriteLock();
            if (gw is ParallelGateway)
            {
                if (gw.Incoming.Count()>1)
                {
                    if (steps
                        .Take((LastStep==int.MaxValue ? steps.Count : LastStep+1))
                        .Where(step => step.ElementID==gw.ID)
                        .Select(step => step.Status)
                        .DefaultIfEmpty(StepStatuses.NotRun)
                        .FirstOrDefault()!=StepStatuses.Waiting)
                        result = false;
                    else
                    {
                        var counts = gw.Incoming
                            .Select(i =>
                                steps
                                .Count(step => step.ElementID==i && step.Status == StepStatuses.Succeeded)
                             );
                        if (counts.Any(c => c!=counts.Max()))
                            result=false;
                    }
                    if (!result && steps
                            .Take((LastStep==int.MaxValue ? steps.Count : LastStep+1))
                            .Where(step => step.ElementID==gw.ID)
                            .Select(step => step.Status)
                            .DefaultIfEmpty(StepStatuses.NotRun)
                            .FirstOrDefault()!=StepStatuses.Waiting)
                    {
                        steps.Add(new(){
                            ElementID=gw.ID,
                            Status=StepStatuses.Waiting,
                            StartTime=DateTime.Now,
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
                    var currentStatus = steps
                        .Take((LastStep==int.MaxValue ? steps.Count : LastStep+1))
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
                                i => steps
                                    .Count(step => step.ElementID==i && step.Status == StepStatuses.Succeeded)
                             );
                        result = counts.All(c => c==counts.Max());
                    }
                }
            }
            if (result)
            {
                steps.Add(new()
                {
                    ElementID=gw.ID,
                    Status=StepStatuses.Started,
                    StartTime=DateTime.Now,
                    IncomingID=sourceID
                });
                changed=true;
            }
            stateLock.ExitWriteLock();
            if (changed)
                triggerChange();
            return result;
        }

        internal void StartAnimation()
        {
            LastStep = -1;
        }

        internal string MoveToNextStep()
        {
            LastStep++;
            stateLock.EnterReadLock();
            var result =  ((LastStep==0 || (LastStep>steps.Count)) ? null : steps.Skip(LastStep-1).First().ElementID);
            stateLock.ExitReadLock();
            return result;
        }

        internal bool HasNext()
        {
            stateLock.EnterReadLock();
            var result = steps.Count+1>LastStep;
            stateLock.ExitReadLock();
            return result;
        }

        internal void FinishAnimation()
        {
            LastStep = int.MaxValue;
        }

        private void AddPathEntry(string elementID, StepStatuses status, DateTime start, string incomingID = null, IEnumerable<string> outgoingID = null, DateTime? end = null, string completedBy = null)
        {
            stateLock.EnterWriteLock();
            if (steps.Any(step => step.ElementID==elementID))
            {
                var lastStep = steps.Last(step=>step.ElementID==elementID);
                if (lastStep.Status==StepStatuses.WaitingStart)
                    start=lastStep.StartTime;
            }
            steps.Add(new()
            {
                ElementID=elementID,
                Status=status,
                StartTime=start,
                IncomingID=incomingID,
                EndTime=end,
                CompletedBy=completedBy,
                OutgoingID=outgoingID?.ToImmutableArray()??null
            });
            stateLock.ExitWriteLock();
            triggerChange();
        }

        private void GetIncomingIDAndStart(string elementID,out DateTime start,out string incoming)
        {
            start = DateTime.Now;
            incoming = null;
            stateLock.EnterReadLock();
            var result = steps
                .Where(step => step.ElementID == elementID && (step.Status == StepStatuses.Waiting||step.Status==StepStatuses.Started))
                .Select(step => new { incoming = step.IncomingID, start = step.StartTime })
                .LastOrDefault();
            stateLock.ExitReadLock();
            if (result!=null)
            {
                start=result.start;
                incoming=result.incoming;
            }
        }

        private void WriteLogLine(string elementID, LogLevel level, string message)
            => process.WriteLogLine(elementID, level, new System.Diagnostics.StackFrame(1, true), DateTime.Now, message);

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
            => SucceedFlowNode((AFlowNode)task,outgoing:outgoing??task.Outgoing,completedByID:completedByID);

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

        private async void Complete(string incoming, string outgoing) 
            => await System.Threading.Tasks.Task.Run(() => complete.Invoke(incoming, outgoing));

        private async void Error(IElement step, Exception ex) 
            => await System.Threading.Tasks.Task.Run(() => error.Invoke(step,ex));

        internal void ProcessFlowElement(IFlowElement flowElement)
        {
            WriteLogLine(flowElement.ID, LogLevel.Debug, "Processing Flow Element in Process Path");
            AddPathEntry(flowElement.ID,StepStatuses.Succeeded,DateTime.Now,incomingID:flowElement.SourceRef, outgoingID:new string[] { flowElement.TargetRef }, end:DateTime.Now);
            Complete(flowElement.ID, flowElement.TargetRef);
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
