using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Gateways;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.State;
using BPMNEngine.State.Objects;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Xml.Serialization;

namespace BPMNEngine.State
{
    internal sealed class ProcessPath(ProcessStepComplete complete, ProcessStepError error, BusinessProcess process, StateLock stateLock, ProcessState.delTriggerStateChange triggerChange) 
        : IStateContainer
    {
        private sealed record ReadOnlyProcessPath : IReadonlyProcessPathContainer
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
                => path.RunQuery<IStateStep>((IEnumerable<IStateStep> Steps)
                    => Steps.Take(stepCount)
                ).ToImmutableList();

            public void Append(XmlWriter writer)
                => Steps.OfType<IStateComponent>()
                    .ForEach(step => step.Write(writer));

            public void Append(Utf8JsonWriter writer)
            {
                writer.WriteStartArray();
                Steps.OfType<IStateComponent>()
                    .ForEach(step => step.Write(writer));
                writer.WriteEndArray();
            }
        }

        private readonly List<IStateStep> steps = [];
        private bool disposedValue;

        internal int LastStep { get; private set; } = int.MaxValue;

        #region IStateContainer
        public XmlReader Load(XmlReader reader, Version version)
        {
            steps.Clear();
            reader.Read();
            while (true)
            {
                var step = new StateStep();
                if (!step.CanRead(reader,version))
                    break;
                reader=step.Read(reader,version);
                steps.Add(step);
            }
            return reader;
        }

        public Utf8JsonReader Load(Utf8JsonReader reader, Version version)
        {
            steps.Clear();
            reader.Read();
            if (reader.TokenType==JsonTokenType.StartArray)
                reader.Read();
            while (reader.TokenType!=JsonTokenType.EndArray)
            {
                var step = new StateStep();
                if (!step.CanRead(reader,version))
                    break;
                reader=step.Read(reader,version);
                steps.Add(step);
            }
            return reader;
        }

        private IEnumerable<T> RunQuery<T>(Func<IEnumerable<IStateStep>,IEnumerable<T>> filter)
        {
            stateLock.EnterReadLock();
            var results = filter(steps).ToImmutableArray();
            stateLock.ExitReadLock();
            return results;
        }

        public IReadOnlyStateContainer Clone()
            =>new ReadOnlyProcessPath(this, steps.Count);

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    steps.Clear();
                }
                disposedValue=true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        internal IEnumerable<SSuspendedStep> ResumeSteps
            => RunQuery((IEnumerable<IStateStep> steps)=> {
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
        => RunQuery((IEnumerable<IStateStep> steps) =>
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
        => RunQuery((IEnumerable<IStateStep> steps) =>
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
        => RunQuery((IEnumerable<IStateStep> steps) =>
        {
            return steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.Started)
                    .Select(grp => grp.Key);
        });

        public IEnumerable<string> WaitingSteps
        => RunQuery((IEnumerable<IStateStep> steps) =>
        {
            return steps
                    .GroupBy(step => step.ElementID)
                    .Where(grp => grp.Last().Status==StepStatuses.Started
                    || grp.Last().Status==StepStatuses.Waiting)
                    .Select(grp => grp.Key);
        });

        public IEnumerable<SStepSuspension> SuspendedSteps
        => RunQuery((IEnumerable<IStateStep> steps) =>
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
            return RunQuery((IEnumerable<IStateStep> steps) =>
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
            var results = RunQuery<int>((IEnumerable<IStateStep> steps) =>
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
            if (gw is ParallelGateway && gw.Incoming.Count()>1)
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
                    steps.Add(new StateStep(gw.ID, StepStatuses.Waiting, DateTime.Now, sourceID));
                    changed=true;
                }
            }
            else if (gw is ExclusiveGateway && gw.Incoming.Count()>1)
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
            if (result)
            {
                steps.Add(new StateStep(gw.ID,StepStatuses.Started,DateTime.Now,sourceID));
                changed=true;
            }
            stateLock.ExitWriteLock();
            if (changed)
                triggerChange();
            return result;
        }

        internal void StartAnimation()
            =>LastStep = -1;

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
            =>LastStep = int.MaxValue;

        private void AddPathEntry(string elementID, StepStatuses status, DateTime start, string incomingID = null, IEnumerable<string> outgoingID = null, DateTime? end = null, string completedBy = null)
        {
            stateLock.EnterWriteLock();
            if (steps.Exists(step => step.ElementID==elementID))
            {
                var lastStep = steps.Last(step=>step.ElementID==elementID);
                if (lastStep.Status==StepStatuses.WaitingStart)
                    start=lastStep.StartTime;
            }
            steps.Add(new StateStep(elementID, status, start, incomingID, end, completedBy, outgoingID?.ToImmutableArray()??null));
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

        private void Complete(string incoming, string outgoing) 
            => System.Threading.Tasks.Task.Run(() => complete.Invoke(incoming, outgoing));

        private void Error(IElement step, Exception ex) 
            => System.Threading.Tasks.Task.Run(() => error.Invoke(step,ex));

        internal void ProcessFlowElement(IFlowElement flowElement)
        {
            WriteLogLine(flowElement.ID, LogLevel.Debug, "Processing Flow Element in Process Path");
            AddPathEntry(flowElement.ID,StepStatuses.Succeeded,DateTime.Now,incomingID:flowElement.SourceRef, outgoingID:[flowElement.TargetRef], end:DateTime.Now);
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
