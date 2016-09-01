using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal sealed class ProcessPath
    {
        private ProcessStepComplete _complete;
        private ProcessStepError _error;
        private List<sPathEntry> _pathEntries;

        private int _lastStep;
        internal int LastStep { get { return _lastStep; } }

        public ProcessPath(ProcessStepComplete complete,ProcessStepError error)
        {
            _complete = complete;
            _error = error;
            _pathEntries = new List<sPathEntry>();
            _lastStep = int.MaxValue;
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
            foreach (sPathEntry spe in _pathEntries)
                spe.Append(writer);
            writer.WriteEndElement();
        }

        private void _AsyncCallback(IAsyncResult result) { }

        internal bool Load(XmlElement element)
        {
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

        internal void MoveToNextStep()
        {
            _lastStep++;
        }

        internal bool HasNext()
        {
            return _pathEntries.Count+1 > _lastStep;
        }

        internal void FinishAnimation()
        {
            _lastStep = int.MaxValue;
        }

        internal void StartEvent(AEvent Event, string incoming)
        {
            lock (_pathEntries)
            {
                _pathEntries.Add(new sPathEntry(Event.id, StepStatuses.Waiting, DateTime.Now, incoming));
            }
        }

        internal void SucceedEvent(AEvent Event)
        {
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
                    _pathEntries.Add(new sPathEntry(Event.id, StepStatuses.Succeeded, start, incoming, DateTime.Now));
                else
                    _pathEntries.Add(new sPathEntry(Event.id, StepStatuses.Succeeded, start, incoming, Event.Outgoing[0], DateTime.Now));
                _complete.BeginInvoke(Event.id,(Event.Outgoing==null ? null : Event.Outgoing[0]), new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void FailEvent(AEvent Event)
        {
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
                _pathEntries.Add(new sPathEntry(Event.id, StepStatuses.Failed, start, incoming, DateTime.Now));
                _error.BeginInvoke(Event, new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void ProcessSequenceFlow(SequenceFlow flow)
        {
            lock (_pathEntries)
            {
                _pathEntries.Add(new sPathEntry(flow.id, StepStatuses.Succeeded, DateTime.Now, flow.sourceRef, flow.targetRef, DateTime.Now));
                _complete.BeginInvoke(flow.id,flow.targetRef, new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void StartTask(ATask task, string incoming)
        {
            lock (_pathEntries)
            {
                _pathEntries.Add(new sPathEntry(task.id, StepStatuses.Waiting, DateTime.Now, incoming));
            }
        }

        internal void FailTask(ATask task)
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
                _pathEntries.Add(new sPathEntry(task.id, StepStatuses.Failed, start, incoming, DateTime.Now));
                _error.BeginInvoke(task, new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void SucceedTask(ATask task)
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
                    _pathEntries.Add(new sPathEntry(task.id, StepStatuses.Succeeded, start, incoming, DateTime.Now));
                else
                    _pathEntries.Add(new sPathEntry(task.id, StepStatuses.Succeeded, start, incoming, task.Outgoing[0], DateTime.Now));
                _complete.BeginInvoke(task.id,(task.Outgoing==null ? null : task.Outgoing[0]), new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void StartGateway(AGateway gateway,string incoming){
            lock (_pathEntries)
            {
                _pathEntries.Add(new sPathEntry(gateway.id, StepStatuses.Waiting, DateTime.Now, incoming));
            }
        }

        internal void FailGateway(AGateway gateway)
        {
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
                _pathEntries.Add(new sPathEntry(gateway.id, StepStatuses.Failed, start, incoming, DateTime.Now));
                _error.BeginInvoke(gateway, new AsyncCallback(_AsyncCallback), null);
            }
        }

        internal void SuccessGateway(AGateway gateway,string[] chosenExits)
        {
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
                    _pathEntries.Add(new sPathEntry(gateway.id, StepStatuses.Succeeded, start, incoming, outgoing, DateTime.Now));
                    _complete.BeginInvoke(gateway.id,outgoing, new AsyncCallback(_AsyncCallback), null);
                }
            }
        }
    }
}
