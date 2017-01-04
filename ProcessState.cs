using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    public sealed class ProcessState
    {
        private object _lock;
        private object _docLock;

        private ProcessPath _path;
        internal ProcessPath Path { get { return _path; } }

        private List<sStepSuspension> _suspensions;
        internal void SuspendStep(string elementID,TimeSpan span)
        {
            lock (_suspensions)
            {
                _suspensions.Add(new BpmEngine.sStepSuspension(elementID, _path.CurrentStepIndex(elementID), span));
                _stateChanged();
            }
        }

        internal sStepSuspension[] SuspendedSteps
        {
            get
            {
                List<sStepSuspension> ret = new List<sStepSuspension>();
                foreach (sStepSuspension ss in _suspensions)
                {
                    if (_path.IsStepWaiting(ss.id, ss.StepIndex))
                        ret.Add(ss);
                }
                return ret.ToArray();
            }
        }

        private List<sVariableEntry> _variables;

        internal object this[string elementID, string variableName]
        {
            get
            {
                object ret = null;
                int stepIndex =-1;
                if (elementID == null)
                    stepIndex = _path.LastStep;
                else
                {
                    lock (_path)
                    {
                        stepIndex = _path.CurrentStepIndex(elementID);
                    }
                }
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (sve.Name == variableName && sve.PathStepIndex <= stepIndex)
                            ret = sve.Value;
                    }
                }
                return ret;
            }
            set
            {
                lock (_variables)
                {
                    _variables.Add(new sVariableEntry(variableName, _path.CurrentStepIndex(elementID), value));
                    _variables.Sort();
                    _stateChanged();
                }
            }
        }

        internal string[] this[string elementID]
        {
            get
            {
                List<string> ret = new List<string>();
                int stepIndex = -1;
                if (elementID == null)
                    stepIndex = _path.LastStep;
                else
                {
                    lock (_path)
                    {
                        stepIndex = _path.CurrentStepIndex(elementID);
                    }
                }
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (sve.PathStepIndex <= stepIndex)
                        {
                            if (sve.Value == null)
                                ret.Remove(sve.Name);
                            else if (!ret.Contains(sve.Name))
                                ret.Add(sve.Name);
                        }
                    }
                }
                return ret.ToArray();
            }
        }

        private OnStateChange _onStateChange;
        internal OnStateChange OnStateChange { set { _onStateChange = value; } }

        internal ProcessState(ProcessStepComplete complete, ProcessStepError error)
        {
            _lock = new object();
            _docLock = new object();
            _path = new ProcessPath(complete,error,new processStateChanged(_stateChanged));
            _variables = new List<sVariableEntry>();
            _suspensions = new List<BpmEngine.sStepSuspension>();
        }

        internal bool Load(XmlDocument doc)
        {
            try
            {
                foreach (XmlNode n in doc.ChildNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        switch (n.Name)
                        {
                            case "ProcessState":
                                foreach (XmlNode nd in n.ChildNodes)
                                {
                                    if (nd.NodeType == XmlNodeType.Element)
                                    {
                                        switch (nd.Name)
                                        {
                                            case "ProcessPath":
                                                _path.Load((XmlElement)nd);
                                                break;
                                            case "ProcessVariables":
                                                foreach (XmlNode pnd in nd.ChildNodes)
                                                {
                                                    if (pnd.NodeType == XmlNodeType.Element)
                                                        _variables.Add(new sVariableEntry((XmlElement)pnd));
                                                }
                                                _variables.Sort();
                                                break;
                                            case "SuspendedSteps":
                                                foreach (XmlNode ssnd in nd.ChildNodes)
                                                {
                                                    if (ssnd.NodeType == XmlNodeType.Element)
                                                        _suspensions.Add(new BpmEngine.sStepSuspension((XmlElement)ssnd));
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument ret = new XmlDocument();
                lock (_docLock)
                {
                    try
                    {
                        MemoryStream ms = new MemoryStream();
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.Encoding = Encoding.UTF8;
                        XmlWriter writer = XmlWriter.Create(ms, settings);
                        writer.WriteStartDocument();
                        writer.WriteStartElement("ProcessState");
                        _path.Append(writer);
                        writer.WriteStartElement("ProcessVariables");
                        foreach (sVariableEntry sve in _variables)
                            sve.Append(writer);
                        writer.WriteEndElement();
                        writer.WriteStartElement("SuspendedSteps");
                        foreach (sStepSuspension ss in _suspensions)
                            ss.Append(writer);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.Flush();
                        System.Diagnostics.Debug.WriteLine(UTF8Encoding.UTF8.GetString(ms.ToArray()));
                        ms.Position = 0;
                        ret.Load(ms);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
                return ret;
            }
        }

        private void _stateChanged()
        {
            lock (_lock)
            {
                if (_onStateChange!=null)
                { try { _onStateChange(this.Document); } catch (Exception ex) { } }
            }
        }
    }
}
