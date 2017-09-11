using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Org.Reddragonit.BpmEngine.Interfaces;

namespace Org.Reddragonit.BpmEngine
{
    internal sealed class ProcessPath
    {
        private ProcessStepComplete _complete;
        private ProcessStepError _error;
        private processStateChanged _stateChanged;
        private List<sPathEntry> _pathEntries;

        private int _lastStep;
        internal int LastStep { get { return _lastStep; } }

        public ProcessPath(ProcessStepComplete complete,ProcessStepError error, processStateChanged stateChanged)
        {
            _complete = complete;
            _error = error;
            _stateChanged = stateChanged;
            _pathEntries = new List<sPathEntry>();
            _lastStep = int.MaxValue;
        }

        internal sSuspendedStep[] ResumeSteps
        {
            get
            {
                List<sSuspendedStep> ret = new List<sSuspendedStep>();
                foreach (sPathEntry spe in _pathEntries)
                {
                    switch (spe.Status)
                    {
                        case StepStatuses.Suspended:
                            bool add = true;
                            foreach (sSuspendedStep ss in ret)
                            {
                                if (ss.ElementID == spe.ElementID)
                                {
                                    add = false;
                                    break;
                                }
                            }
                            if (add)
                                ret.Add(new sSuspendedStep(spe.IncomingID,spe.ElementID));
                            break;
                        case StepStatuses.Succeeded:
                        case StepStatuses.Failed:
                        case StepStatuses.Waiting:
                            for(int x = 0; x < ret.Count; x++)
                            {
                                if (ret[x].ElementID == spe.ElementID)
                                {
                                    ret.RemoveAt(x);
                                    break;
                                }
                            }
                            if (spe.OutgoingID != null)
                            {
                                foreach (string str in spe.OutgoingID)
                                {
                                    bool addNext = true;
                                    foreach (sSuspendedStep ss in ret)
                                    {
                                        if (ss.ElementID == str)
                                        {
                                            addNext = false;
                                            break;
                                        }
                                    }
                                    if (addNext)
                                        ret.Add(new sSuspendedStep(spe.ElementID, str));
                                }
                            }
                            break;
                    }
                }
                return ret.ToArray();
            }
        }

        internal bool IsStepWaiting(string id, int stepIndex)
        {
            for (int x = stepIndex+1; x < _pathEntries.Count; x++)
            {
                if (_pathEntries[x].ElementID == id)
                {
                    if (_pathEntries[x].Status == StepStatuses.Succeeded)
                        return false;
                }
            }
            return true;
        }

        public StepStatuses GetStatus(string elementid)
        {
            StepStatuses ret = StepStatuses.NotRun;
            for (int x = 0; x < Math.Min(_pathEntries.Count,_lastStep);x++ )
            {
                if (_pathEntries[x].ElementID == elementid)
                    ret = _pathEntries[x].Status;
            }
            return ret;
        }

        public int CurrentStepIndex(string elementid)
        {
            int ret = -1;
            for (int x = 0; x < Math.Min(_pathEntries.Count, _lastStep); x++)
            {
                if (_pathEntries[x].ElementID == elementid)
                    ret = x;
            }
            return ret;
        }

        public void Append(XmlWriter writer)
        {
            writer.WriteStartElement("ProcessPath");
            sPathEntry[] paths = new sPathEntry[0];
            lock (_pathEntries)
            {
                paths = _pathEntries.ToArray();
            }
            foreach (sPathEntry spe in paths)
                spe.Append(writer);
            writer.WriteEndElement();
        }

        private void _AsyncCallback(IAsyncResult result) { }

        internal bool Load(XmlElement element)
        {
            Log.Debug("Loading Process Path from XML Element");
            try
            {
                foreach (XmlNode n in element.ChildNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                        _pathEntries.Add(new sPathEntry((XmlElement)n));
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        internal void StartAnimation()
        {
            _lastStep = -1;
        }

        internal string MoveToNextStep()
        {
            _lastStep++;
            return ((_lastStep==0 || (_lastStep>_pathEntries.Count)) ? null : _pathEntries[_lastStep-1].ElementID);
        }

        internal bool HasNext()
        {
            return _pathEntries.Count+1 > _lastStep;
        }

        internal void FinishAnimation()
        {
            _lastStep = int.MaxValue;
        }

        private void _addPathEntry(sPathEntry pathEntry,bool locked)
        {
            if (locked)
            {
                _pathEntries.Add(pathEntry);
                _stateChanged();
            }
            else
            {
                lock (_pathEntries)
                {
                    _pathEntries.Add(pathEntry);
                    _stateChanged();
                }
            }
        }

        internal void StartEvent(AEvent Event, string incoming)
        {
            Log.Debug("Starting Event {0} in Process Path", new object[] { Event.id });
            _addPathEntry(new sPathEntry(Event.id, StepStatuses.Waiting, DateTime.Now, incoming),false);
        }

        internal void SucceedEvent(AEvent Event)
        {
            Log.Debug("Succeeding Event {0} in Process Path", new object[] { Event.id });
            lock (_pathEntries)
            {
                string incoming = null;
                DateTime start = DateTime.Now;
                for(int x=_pathEntries.Count-1;x>=0;x--)
                {
                    if (_pathEntries[x].ElementID == Event.id && _pathEntries[x].Status == StepStatuses.Waiting)
                    {
                        incoming = _pathEntries[x].IncomingID;
                        start = _pathEntries[x].StartTime;
                        break;
                    }
                }
                if (Event.Outgoing==null)
                    _addPathEntry(new sPathEntry(Event.id, StepStatuses.Succeeded, start, incoming, DateTime.Now), true);
                else
                    _addPathEntry(new sPathEntry(Event.id, StepStatuses.Succeeded, start, incoming, Event.Outgoing[0], DateTime.Now), true);
                _complete.BeginInvoke(Event.id,(Event.Outgoing==null ? null : Event.Outgoing[0]), new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void FailEvent(AEvent Event)
        {
            Log.Debug("Failing Event {0} in Process Path", new object[] { Event.id });
            lock (_pathEntries)
            {
                string incoming = null;
                DateTime start = DateTime.Now;
                for (int x = _pathEntries.Count - 1; x >= 0; x--)
                {
                    if (_pathEntries[x].ElementID == Event.id && _pathEntries[x].Status == StepStatuses.Waiting)
                    {
                        incoming = _pathEntries[x].IncomingID;
                        start = _pathEntries[x].StartTime;
                        break;
                    }
                }
                _addPathEntry(new sPathEntry(Event.id, StepStatuses.Failed, start, incoming, DateTime.Now),true);
                _error.BeginInvoke(Event,null, new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void ProcessMessageFlow(MessageFlow flow)
        {
            Log.Debug("Processing Message Flow {0} in Process Path", new object[] { flow.id });
            _addPathEntry(new sPathEntry(flow.id, StepStatuses.Succeeded, DateTime.Now, flow.sourceRef, flow.targetRef, DateTime.Now), false);
            _complete.BeginInvoke(flow.id, flow.targetRef, new AsyncCallback(_AsyncCallback), null);
        }

        internal void ProcessSequenceFlow(SequenceFlow flow)
        {
            Log.Debug("Processing Sequence Flow {0} in Process Path", new object[] { flow.id });
            _addPathEntry(new sPathEntry(flow.id, StepStatuses.Succeeded, DateTime.Now, flow.sourceRef, flow.targetRef, DateTime.Now), false);
            _complete.BeginInvoke(flow.id, flow.targetRef, new AsyncCallback(_AsyncCallback), null);
        }

        internal void StartTask(ATask task, string incoming)
        {
            Log.Debug("Starting Task {0} in Process Path", new object[] { task.id });
            _addPathEntry(new sPathEntry(task.id, StepStatuses.Waiting, DateTime.Now, incoming), false);
        }

        internal void FailTask(ATask task,Exception ex)
        {
            Log.Debug("Failing Task {0} in Process Path", new object[] { task.id });
            lock (_pathEntries)
            {
                string incoming = null;
                DateTime start = DateTime.Now;
                for (int x = _pathEntries.Count - 1; x >= 0; x--)
                {
                    if (_pathEntries[x].ElementID == task.id && _pathEntries[x].Status == StepStatuses.Waiting)
                    {
                        incoming = _pathEntries[x].IncomingID;
                        start = _pathEntries[x].StartTime;
                        break;
                    }
                }
                _addPathEntry(new sPathEntry(task.id, StepStatuses.Failed, start, incoming, DateTime.Now),true);
                _error.BeginInvoke(task,ex, new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void SucceedTask(UserTask task,string completedByID)
        {
            Log.Debug("Succeeding Task {0} in Process Path with Completed By ID {1}", new object[] { task.id,completedByID });
            _SucceedTask(task, completedByID);
        }

        internal void SucceedTask(ATask task)
        {
            Log.Debug("Succeeding Task {0} in Process Path", new object[] { task.id });
            _SucceedTask(task, null);
        }

        private void _SucceedTask(ATask task,string completedByID)
        {
            lock (_pathEntries)
            {
                string incoming = null;
                DateTime start = DateTime.Now;
                for (int x = _pathEntries.Count - 1; x >= 0; x--)
                {
                    if (_pathEntries[x].ElementID == task.id && _pathEntries[x].Status == StepStatuses.Waiting)
                    {
                        incoming = _pathEntries[x].IncomingID;
                        start = _pathEntries[x].StartTime;
                        break;
                    }
                }
                if (task.Outgoing == null)
                    _addPathEntry(new sPathEntry(task.id, StepStatuses.Succeeded, start, incoming, DateTime.Now,(task is UserTask ? completedByID : null)),true);
                else
                    _addPathEntry(new sPathEntry(task.id, StepStatuses.Succeeded, start, incoming, task.Outgoing[0], DateTime.Now, (task is UserTask ? completedByID : null)),true);
                _complete.BeginInvoke(task.id,(task.Outgoing==null ? null : task.Outgoing[0]), new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void StartGateway(AGateway gateway, string incoming)
        {
            Log.Debug("Starting Gateway {0} in Process Path", new object[] { gateway.id });
            _addPathEntry(new sPathEntry(gateway.id, StepStatuses.Waiting, DateTime.Now, incoming), false);
        }

        internal void FailGateway(AGateway gateway)
        {
            Log.Debug("Failing Gateway {0} in Process Path", new object[] { gateway.id });
            lock (_pathEntries)
            {
                string incoming = null;
                DateTime start = DateTime.Now;
                for (int x = _pathEntries.Count - 1; x >= 0; x--)
                {
                    if (_pathEntries[x].ElementID == gateway.id && _pathEntries[x].Status == StepStatuses.Waiting)
                    {
                        incoming = _pathEntries[x].IncomingID;
                        start = _pathEntries[x].StartTime;
                        break;
                    }
                }
                _addPathEntry(new sPathEntry(gateway.id, StepStatuses.Failed, start, incoming, DateTime.Now),true);
                _error.BeginInvoke(gateway,null, new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void SuccessGateway(AGateway gateway,string[] chosenExits)
        {
            Log.Debug("Succeeding Gateway {0} in Process Path", new object[] { gateway.id });
            lock (_pathEntries)
            {
                string incoming = null;
                DateTime start = DateTime.Now;
                for (int x = _pathEntries.Count - 1; x >= 0; x--)
                {
                    if (_pathEntries[x].ElementID == gateway.id && _pathEntries[x].Status == StepStatuses.Waiting)
                    {
                        incoming = _pathEntries[x].IncomingID;
                        start = _pathEntries[x].StartTime;
                        break;
                    }
                }
                foreach (string outgoing in chosenExits)
                {
                    _addPathEntry(new sPathEntry(gateway.id, StepStatuses.Succeeded, start, incoming, outgoing, DateTime.Now),true);
                    _complete.BeginInvoke(gateway.id,outgoing, new AsyncCallback(_AsyncCallback), null);
                }
            }
        }

        internal void SuspendElement(string sourceID, IElement elem)
        {
            Log.Debug("Suspending Element {0} in Process Path", new object[] { elem.id });
            _addPathEntry(new sPathEntry(elem.id, StepStatuses.Suspended, DateTime.Now, sourceID),false);
        }
    }
}
