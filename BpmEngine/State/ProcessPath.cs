using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using System;
using System.Collections.Generic;
using System.Xml;
using Org.Reddragonit.BpmEngine.Interfaces;
using System.Globalization;
using Org.Reddragonit.BpmEngine.Elements;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Org.Reddragonit.BpmEngine.State
{
    internal sealed class ProcessPath : AStateContainer
    {
        private const string _PATH_ENTRY_ELEMENT = "sPathEntry";
        private const string _STEP_STATUS = "status";
        private const string _ELEMENT_ID = "elementID";
        private const string _START_TIME = "startTime";
        private const string _END_TIME = "endTime";
        private const string _OUTGOING_ID = "outgoingID";
        private const string _INCOMING_ID = "incomingID";
        private const string _OUTGOING_ELEM = "outgoing";
        private const string _COMPLETED_BY = "CompletedByID";

        protected override string _ContainerName
        {
            get
            {
                return "ProcessPath";
            }
        }

        private readonly ProcessStepComplete _complete;
        private readonly ProcessStepError _error;
        private readonly processStateChanged _stateChanged;

        private int _lastStep;
        internal int LastStep { get { return _lastStep; } }

        public ProcessPath(ProcessStepComplete complete, ProcessStepError error, processStateChanged stateChanged, ProcessState state)
            : base(state)
        {
            _complete = complete;
            _error = error;
            _stateChanged = stateChanged;
            _lastStep = int.MaxValue;
        }

        internal IEnumerable<sSuspendedStep> ResumeSteps
        {
            get
            {
                return ChildNodes.Cast<XmlNode>()
                    .GroupBy(elem => elem.Attributes[_ELEMENT_ID].Value)
                    .Where(grp => (StepStatuses)Enum.Parse(typeof(StepStatuses), grp.Last().Attributes[_STEP_STATUS].Value)==StepStatuses.Suspended)
                    .Select(grp => new sSuspendedStep(grp.Last().Attributes[_INCOMING_ID].Value, grp.Last().Attributes[_ELEMENT_ID].Value));
            }
        }

        internal IEnumerable<sDelayedStartEvent> DelayedEvents
        {
            get
            {
                return ChildNodes.Cast<XmlNode>()
                    .GroupBy(elem => elem.Attributes[_ELEMENT_ID].Value)
                    .Where(grp => (StepStatuses)Enum.Parse(typeof(StepStatuses), grp.Last().Attributes[_STEP_STATUS].Value)==StepStatuses.WaitingStart)
                    .Select(grp => new sDelayedStartEvent(
                        grp.Last().Attributes[_INCOMING_ID].Value,
                        grp.Last().Attributes[_ELEMENT_ID].Value, 
                        DateTime.ParseExact((grp.Last().Attributes[_END_TIME]!=null ? grp.Last().Attributes[_END_TIME].Value : grp.Last().Attributes[_START_TIME].Value), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture))
                    );
            }
        }

        public IEnumerable<string> ActiveSteps
        {
            get
            {
                return ChildNodes.Cast<XmlNode>()
                    .GroupBy(elem => elem.Attributes[_ELEMENT_ID].Value)
                    .Where(grp => (StepStatuses)Enum.Parse(typeof(StepStatuses), grp.Last().Attributes[_STEP_STATUS].Value)==StepStatuses.WaitingStart
                        || (StepStatuses)Enum.Parse(typeof(StepStatuses), grp.Last().Attributes[_STEP_STATUS].Value)==StepStatuses.Waiting)
                    .Select(grp => grp.Key);
            }
        }

        internal bool IsStepWaiting(string id, int stepIndex)
        {
            return ChildNodes.Cast<XmlNode>()
                .Skip(stepIndex)
                .Where(n => n.Attributes[_ELEMENT_ID].Value==id)
                .Select(n => n.Attributes[_STEP_STATUS].Value)
                .DefaultIfEmpty("")
                .FirstOrDefault() != StepStatuses.Succeeded.ToString();
        }

        public StepStatuses GetStatus(string elementid)
        {
            return ChildNodes.Cast<XmlNode>()
                .Where(n => n.Attributes[_ELEMENT_ID].Value==elementid)
                .Select(n => (StepStatuses)Enum.Parse(typeof(StepStatuses), n.Attributes[_STEP_STATUS].Value))
                .DefaultIfEmpty(StepStatuses.NotRun)
                .LastOrDefault();
        }

        public int GetStepSuccessCount(string elementid)
        {
            return ChildNodes.Cast<XmlNode>()
                .Count(n => n.Attributes[_ELEMENT_ID].Value==elementid && (StepStatuses)Enum.Parse(typeof(StepStatuses), n.Attributes[_STEP_STATUS].Value) == StepStatuses.Succeeded);
        }

        public int CurrentStepIndex(string elementid)
        {
            return ChildNodes.Cast<XmlNode>()
                .Any(n => n.Attributes[_ELEMENT_ID].Value == elementid)
                ? ChildNodes.Cast<XmlNode>()
                    .Select((node, index) => new { node, index })
                    .Where(pair=>pair.node.Attributes[_ELEMENT_ID].Value == elementid)
                    .Max(pair=>pair.index)
                : -1;
        }

        internal void StartAnimation()
        {
            _lastStep = -1;
        }

        internal string MoveToNextStep()
        {
            _lastStep++;
            XmlElement[] nodes = ChildNodes;
            return ((_lastStep==0 || (_lastStep>nodes.Length)) ? null : nodes[_lastStep-1].Attributes[_ELEMENT_ID].Value);
        }

        internal bool HasNext()
        {
            return ChildNodes.Length+1 > _lastStep;
        }

        internal void FinishAnimation()
        {
            _lastStep = int.MaxValue;
        }

        private void _addPathEntry(string elementID, string incomingID, StepStatuses status, DateTime start, DateTime end)
        {
            _addPathEntry(elementID, incomingID, null, status, start, end, null);
        }

        private void _addPathEntry(string elementID, string incomingID, StepStatuses status, DateTime start)
        {
            _addPathEntry(elementID, incomingID, null, status, start, null, null);
        }

        private void _addPathEntry(string elementID, string incomingID, IEnumerable<string> outgoingID, StepStatuses status, DateTime start, DateTime end)
        {
            _addPathEntry(elementID, incomingID, outgoingID, status, start, end, null);
        }

        private void _addPathEntry(string elementID, string incomingID, string outgoingID, StepStatuses status, DateTime start, DateTime end)
        {
            _addPathEntry(elementID, incomingID, new string[] { outgoingID }, status, start, end, null);
        }

        private void _addPathEntry(string elementID, string incomingID, IEnumerable<string> outgoingID, StepStatuses status, DateTime start, DateTime? end, string completedBy)
        {
            XmlElement elem = _ProduceElement(_PATH_ENTRY_ELEMENT);
            _SetAttribute(elem, _ELEMENT_ID, elementID);
            _SetAttribute(elem,_STEP_STATUS,status.ToString());
            _SetAttribute(elem, _START_TIME, start.ToString(Constants.DATETIME_FORMAT));
            if (incomingID != null)
                _SetAttribute(elem, _INCOMING_ID, incomingID);
            if (end.HasValue)
                _SetAttribute(elem, _END_TIME, end.Value.ToString(Constants.DATETIME_FORMAT));
            if (completedBy != null)
                _SetAttribute(elem, _COMPLETED_BY, completedBy);
            if (outgoingID != null)
            {
                if (outgoingID.Count() == 1)
                    _SetAttribute(elem, _OUTGOING_ID, outgoingID.First());
                else
                {
                    foreach (string str in outgoingID)
                    {
                        elem.AppendChild(_ProduceElement(_OUTGOING_ELEM));
                        elem.ChildNodes[elem.ChildNodes.Count - 1].InnerText = str;
                    }
                }
            }
            //Console.WriteLine(string.Format("Adding Path Entry for {0}-{1}", elementID,status));
            _AppendElement(elem);
            _stateChanged();
        }

        private void _GetIncomingIDAndStart(string elementID,out DateTime start,out string incoming)
        {
            start = DateTime.Now;
            incoming = null;
            var result = ChildNodes.Cast<XmlNode>()
                .Where(n => n.Attributes[_ELEMENT_ID].Value == elementID && n.Attributes[_STEP_STATUS].Value == StepStatuses.Waiting.ToString())
                .Select(n => new { incoming = (n.Attributes[_INCOMING_ID]==null ? null : n.Attributes[_INCOMING_ID].Value), start = DateTime.ParseExact(n.Attributes[_START_TIME].Value, Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture) })
                .LastOrDefault();
            if (result!=null)
            {
                start=result.start;
                incoming=result.incoming;
            }
        }

        internal void DelayEventStart(AEvent Event,string incoming,TimeSpan delay)
        {
            _WriteLogLine(Event.id, LogLevels.Debug, "Delaying start of event in Process Path");
            _addPathEntry(Event.id, incoming, StepStatuses.WaitingStart, DateTime.Now,DateTime.Now.Add(delay));
        }

        internal void StartEvent(AEvent Event, string incoming)
        {
            _WriteLogLine(Event.id,LogLevels.Debug,"Starting Event in Process Path");
            _addPathEntry(Event.id,incoming,StepStatuses.Waiting, DateTime.Now);
        }
        private async void Complete(string incoming, string outgoing) => await System.Threading.Tasks.Task.Run(() => _complete.Invoke(incoming, outgoing));

        private async void Error(IElement step, Exception ex) => await System.Threading.Tasks.Task.Run(() => _error.Invoke(step,ex));

        internal void SucceedEvent(AEvent Event)
        {
            _WriteLogLine(Event.id,LogLevels.Debug,"Succeeding Event in Process Path");
            string incoming;
            DateTime start;
            _GetIncomingIDAndStart(Event.id, out start, out incoming);
            if (!Event.Outgoing.Any())
            {
                _addPathEntry(Event.id,incoming,StepStatuses.Succeeded, start, DateTime.Now);
                Complete(Event.id, null);
            }
            else
            {
                _addPathEntry(Event.id, incoming, Event.Outgoing, StepStatuses.Succeeded, start, DateTime.Now);
                foreach (string id in Event.Outgoing)
                    Complete(Event.id, id);
            }
        }

        internal void FailEvent(AEvent Event)
        {
            _WriteLogLine(Event.id, LogLevels.Debug, "Failing Event in Process Path");
            string incoming;
            DateTime start;
            _GetIncomingIDAndStart(Event.id, out start, out incoming);
            _addPathEntry(Event.id,incoming,StepStatuses.Failed, start, DateTime.Now);
            Error(Event, null);
        }

        internal void StartSubProcess(SubProcess SubProcess, string incoming)
        {
            _WriteLogLine(SubProcess.id, LogLevels.Debug, "Starting SubProcess in Process Path");
            _addPathEntry(SubProcess.id, incoming, StepStatuses.Waiting, DateTime.Now);
        }

        internal void SucceedSubProcess(SubProcess SubProcess)
        {
            _WriteLogLine(SubProcess.id, LogLevels.Debug, "Succeeding SubProcess in Process Path");
            string incoming;
            DateTime start;
            _GetIncomingIDAndStart(SubProcess.id, out start, out incoming);
            IEnumerable<string> outgoing = SubProcess.Outgoing;
            if (!outgoing.Any())
            {
                _addPathEntry(SubProcess.id, incoming, StepStatuses.Succeeded, start, DateTime.Now);
                Complete(SubProcess.id, null);
            }
            else
            {
                _addPathEntry(SubProcess.id, incoming, outgoing, StepStatuses.Succeeded, start, DateTime.Now);
                foreach (string id in outgoing)
                    Complete(SubProcess.id, id);
            }
        }

        internal void ProcessFlowElement(IFlowElement flowElement)
        {
            _WriteLogLine(flowElement.id, LogLevels.Debug, "Processing Flow Element in Process Path");
            _addPathEntry(flowElement.id, flowElement.sourceRef, flowElement.targetRef, StepStatuses.Succeeded, DateTime.Now, DateTime.Now);
            Complete(flowElement.id, flowElement.targetRef);
        }

        internal void StartTask(ATask task, string incoming)
        {
            _WriteLogLine(task.id, LogLevels.Debug, "Starting Task in Process Path");
            _addPathEntry(task.id,incoming, StepStatuses.Waiting, DateTime.Now);
        }

        internal void FailTask(ATask task, Exception ex)
        {
            _WriteLogLine(task.id, LogLevels.Debug, "Failing Task in Process Path");
            string incoming;
            DateTime start;
            _GetIncomingIDAndStart(task.id, out start, out incoming);
            _addPathEntry(task.id,incoming,StepStatuses.Failed, start, DateTime.Now);
            Error(task, ex);
        }

        internal void SucceedTask(UserTask task,string completedByID)
        {
            _WriteLogLine(task.id, LogLevels.Debug, string.Format("Succeeding Task in Process Path with Completed By ID {0}", new object[] { completedByID }));
            _SucceedTask(task, completedByID);
        }

        internal void SucceedTask(ATask task)
        {
            _WriteLogLine(task.id, LogLevels.Debug, "Succeeding Task in Process Path");
            _SucceedTask(task, null);
        }

        private void _SucceedTask(ATask task, string completedByID)
        {
            string incoming;
            DateTime start;
            _GetIncomingIDAndStart(task.id, out start, out incoming);
            _addPathEntry(task.id, incoming, task.Outgoing, StepStatuses.Succeeded, start, DateTime.Now, (task is UserTask ? completedByID : null));
            Complete(task.id, (!task.Outgoing.Any()  ? null : task.Outgoing.First()));
        }

        internal void StartGateway(AGateway gateway, string incoming)
        {
            _WriteLogLine(gateway.id, LogLevels.Debug, "Starting Gateway in Process Path");
            _addPathEntry(gateway.id,incoming,StepStatuses.Waiting, DateTime.Now);
        }

        internal void FailGateway(AGateway gateway)
        {
            _WriteLogLine(gateway.id, LogLevels.Debug, "Failing Gateway in Process Path");
            string incoming;
            DateTime start;
            _GetIncomingIDAndStart(gateway.id, out start, out incoming);
            _addPathEntry(gateway.id, incoming, StepStatuses.Failed, start, DateTime.Now);
            Error(gateway,null);
        }

        internal void SuccessGateway(AGateway gateway, IEnumerable<string> chosenExits)
        {
            _WriteLogLine(gateway.id, LogLevels.Debug, "Succeeding Gateway in Process Path");
            string incoming;
            DateTime start;
            _GetIncomingIDAndStart(gateway.id, out start, out incoming);
            _addPathEntry(gateway.id, incoming, chosenExits, StepStatuses.Succeeded, start, DateTime.Now);
            foreach (string outgoing in chosenExits)
                Complete(gateway.id, outgoing);
        }

        internal void SuspendElement(string sourceID, IElement elem)
        {
            _WriteLogLine(elem.id, LogLevels.Debug, "Suspending Element in Process Path");
            _addPathEntry(elem.id,sourceID,StepStatuses.Suspended, DateTime.Now);
        }

        internal void AbortStep(string sourceID,string attachedToID)
        {
            _WriteLogLine(attachedToID, LogLevels.Debug, "Aborting Process Step");
            _addPathEntry(attachedToID, sourceID, StepStatuses.Aborted, DateTime.Now);

        }
    }
}
