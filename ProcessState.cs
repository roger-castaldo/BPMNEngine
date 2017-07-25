using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        private StringBuilder _sbLog;

        private List<sStepSuspension> _suspensions;
        internal void SuspendStep(string elementID,TimeSpan span)
        {
            Log.Debug("Suspending Step[{0}] for {1}", new object[] { elementID, span });
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

        private bool _isSuspended = false;
        internal bool IsSuspended { get { return _isSuspended; } }

        internal sSuspendedStep[] ResumeSteps {
            get
            {
                if (!_isSuspended)
                    return null;
                return _path.ResumeSteps;
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
        internal OnStateChange OnStateChange { set { _onStateChange = value; } get { return _onStateChange; } }

        internal ProcessState(ProcessStepComplete complete, ProcessStepError error)
        {
            _lock = new object();
            _docLock = new object();
            _path = new ProcessPath(complete,error,new processStateChanged(_stateChanged));
            _variables = new List<sVariableEntry>();
            _suspensions = new List<BpmEngine.sStepSuspension>();
            _sbLog = new StringBuilder();
        }

        internal bool Load(XmlDocument doc)
        {
            Log.Debug("Loading Process State");
            try
            {
                foreach (XmlNode n in doc.ChildNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        switch (n.Name)
                        {
                            case "ProcessState":
                                if (n.Attributes["isSuspended"] != null)
                                    _isSuspended = bool.Parse(n.Attributes["isSuspended"].Value);
                                foreach (XmlNode nd in n.ChildNodes)
                                {
                                    if (nd.NodeType == XmlNodeType.Element)
                                    {
                                        switch (nd.Name)
                                        {
                                            case "ProcessPath":
                                                Log.Debug("Loading Process Path...");
                                                _path.Load((XmlElement)nd);
                                                break;
                                            case "ProcessVariables":
                                                Log.Debug("Loading Process Variables...");
                                                foreach (XmlNode pnd in nd.ChildNodes)
                                                {
                                                    if (pnd.NodeType == XmlNodeType.Element)
                                                        _variables.Add(new sVariableEntry((XmlElement)pnd));
                                                }
                                                _variables.Sort();
                                                break;
                                            case "SuspendedSteps":
                                                Log.Debug("Loading Suspended Steps...");
                                                foreach (XmlNode ssnd in nd.ChildNodes)
                                                {
                                                    if (ssnd.NodeType == XmlNodeType.Element)
                                                        _suspensions.Add(new BpmEngine.sStepSuspension((XmlElement)ssnd));
                                                }
                                                break;
                                            case "ProcessLog":
                                                Log.Debug("Loading Process Log...");
                                                lock (_sbLog)
                                                {
                                                    if (_sbLog.Length > 0)
                                                        _sbLog = new StringBuilder(((XmlCDataSection)nd.ChildNodes[0]).InnerText+_sbLog.ToString());
                                                    else
                                                        _sbLog = new StringBuilder(((XmlCDataSection)nd.ChildNodes[0]).InnerText);
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
                Log.Exception(e);
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
                        writer.WriteAttributeString("isSuspended", _isSuspended.ToString());
                        lock (_path)
                        {
                            _path.Append(writer);
                        }
                        writer.WriteStartElement("ProcessVariables");
                        sVariableEntry[] vars = new sVariableEntry[0];
                        lock (_variables) {
                            vars = _variables.ToArray();
                        }
                        foreach (sVariableEntry sve in vars)
                            sve.Append(writer);
                        writer.WriteEndElement();
                        writer.WriteStartElement("SuspendedSteps");
                        sStepSuspension[] ssteps = new sStepSuspension[0];
                        lock (_suspensions)
                        {
                            ssteps = _suspensions.ToArray();
                        }
                        foreach (sStepSuspension ss in ssteps)
                            ss.Append(writer);
                        writer.WriteEndElement();
                        if (_sbLog.Length > 0)
                        {
                            writer.WriteStartElement("ProcessLog");
                            lock (_lock)
                            {
                                writer.WriteCData(_sbLog.ToString());
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.Flush();
                        ms.Position = 0;
                        ret.Load(ms);
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
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

        internal void Suspend()
        {
            Log.Debug("Suspending Process State");
            _isSuspended = true;
        }

        internal void Resume()
        {
            Log.Debug("Resuming Process State");
            _isSuspended = false;
        }

        internal void LogLine(AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message)
        {
            lock (_lock)
            {
                _sbLog.AppendLine(string.Format("{0}|{1}|{2}|{3}[{4}]|{5}", new object[]
                {
                    timestamp.ToString(Constants.DATETIME_FORMAT),
                    level,
                    assembly.Name,
                    fileName,
                    lineNumber,
                    message
                }));
            }
        }

        internal void LogException(AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            if (exception is InvalidProcessDefinitionException)
            {
                sb.AppendLine(string.Format(@"MESSAGE:{0}
STACKTRACE:{1}", new object[]
                {
                exception.Message,
                exception.StackTrace
                }));
                foreach (Exception e in ((InvalidProcessDefinitionException)exception).ProcessExceptions)
                {
                    sb.AppendLine(string.Format(@"MESSAGE:{0}
STACKTRACE:{1}", new object[]
                {
                e.Message,
                e.StackTrace
                }));
                }
            }
            else
            {
                Exception ex = exception;
                bool isInner = false;
                while (ex != null)
                {
                    sb.AppendLine(string.Format(@"{2}MESSAGE:{0}
STACKTRACE:{1}", new object[]
                {
                ex.Message,
                ex.StackTrace,
                (isInner ? "INNER_EXCEPTION:" : "")
                }));
                    isInner = true;
                    ex = ex.InnerException;
                }
            }
            LogLine(assembly, fileName, lineNumber, LogLevels.Error, timestamp, sb.ToString());   
        }
    }
}
