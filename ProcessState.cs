using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    public sealed class ProcessState
    {
        private const string _BASE_STATE = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ProcessState isSuspended=\"False\"></ProcessState>";

        private const string _PROCESS_STATE_ELEMENT = "ProcessState";
        private const string _PROCESS_LOG_ELEMENT = "ProcessLog";

        private AutoResetEvent _evnt;
        private XmlDocument _doc;
        private XmlElement _stateElement;
        private XmlElement _log;
        private SuspendedStepContainer _suspensions;
        private StateVariableContainer _variables;
        private ProcessPath _path;
        internal ProcessPath Path { get { return _path; } }

        internal void SuspendStep(string elementID, TimeSpan span)
        {
            Log.Debug("Suspending Step[{0}] for {1}", new object[] { elementID, span });
            _suspensions.SuspendStep(elementID, _path.CurrentStepIndex(elementID), span);
            _stateChanged();
        }

        internal sStepSuspension[] SuspendedSteps
        {
            get
            {
                List<sStepSuspension> ret = new List<sStepSuspension>();
                foreach (sStepSuspension ss in _suspensions.Steps)
                {
                    if (_path.IsStepWaiting(ss.id, ss.StepIndex))
                        ret.Add(ss);
                }
                return ret.ToArray();
            }
        }

        internal bool IsSuspended { get { return (_stateElement.Attributes["isSuspended"]==null ? false : bool.Parse(_stateElement.Attributes["isSuspended"].Value)); }
            private set
            {
                if (_stateElement.Attributes["isSuspended"] != null)
                    _stateElement.Attributes.Append(_doc.CreateAttribute("isSuspended"));
                _stateElement.Attributes["isSuspended"].Value = value.ToString();
            }
        }

        internal sSuspendedStep[] ResumeSteps {
            get
            {
                if (!IsSuspended)
                    return null;
                return _path.ResumeSteps;
            }
        }
        
        internal object this[string elementID, string variableName]
        {
            get
            {
                object ret = null;
                int stepIndex =-1;
                if (elementID == null)
                    stepIndex = _path.LastStep;
                else
                    stepIndex = _path.CurrentStepIndex(elementID);
                ret = _variables[variableName, stepIndex];
                return ret;
            }
            set
            {
                _variables[variableName, _path.CurrentStepIndex(elementID)] = value;
                _stateChanged();
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
                    stepIndex = _path.CurrentStepIndex(elementID);
                return _variables[stepIndex];
            }
        }

        private OnStateChange _onStateChange;
        internal OnStateChange OnStateChange { set { _onStateChange = value; } get { return _onStateChange; } }

        internal ProcessState(ProcessStepComplete complete, ProcessStepError error)
        {
            _evnt = new AutoResetEvent(true);
            _doc = new XmlDocument();
            _doc.LoadXml(_BASE_STATE);
            _stateElement = (XmlElement)_doc.GetElementsByTagName(_PROCESS_STATE_ELEMENT)[0];
            _log = (XmlElement)_doc.GetElementsByTagName(_PROCESS_LOG_ELEMENT)[0];
            _variables = new StateVariableContainer(this);
            _suspensions = new SuspendedStepContainer(this);
            _path = new ProcessPath(complete, error, new processStateChanged(_stateChanged),this);
        }

        internal bool Load(XmlDocument doc)
        {
            if (doc.GetElementsByTagName(_PROCESS_STATE_ELEMENT).Count == 0)
                return false;
            _evnt.WaitOne();
            try
            {
                _doc.LoadXml(doc.OuterXml);
                _stateElement = (XmlElement)_doc.GetElementsByTagName(_PROCESS_STATE_ELEMENT)[0];
                if (_doc.GetElementsByTagName(_PROCESS_LOG_ELEMENT).Count > 0)
                    _log = (XmlElement)_doc.GetElementsByTagName(_PROCESS_LOG_ELEMENT)[0];
                else
                    _log = null;
            }catch(Exception e)
            {
                _evnt.Set();
                return false;
            }
            _evnt.Set();
            _stateChanged();
            return true;
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument ret = new XmlDocument();
                _evnt.WaitOne();
                ret.LoadXml(_doc.OuterXml);
                _evnt.Set();
                return ret;
            }
        }

        private void _stateChanged()
        {
            if (_onStateChange != null)
            { try { _onStateChange(this.Document); } catch (Exception ex) { } }
        }

        internal void Suspend()
        {
            Log.Debug("Suspending Process State");
            IsSuspended = true;
        }

        internal void Resume()
        {
            Log.Debug("Resuming Process State");
            IsSuspended = false;
        }

        internal void LogLine(AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message)
        {
            _evnt.WaitOne();
            if (_log == null)
            {
                _stateElement.AppendChild(_doc.CreateElement(_PROCESS_LOG_ELEMENT));
                _log = (XmlElement)_stateElement.GetElementsByTagName(_PROCESS_LOG_ELEMENT)[0];
            }
            if (_log.ChildNodes.Count == 0)
                _log.AppendChild(_doc.CreateCDataSection(""));
            ((XmlCDataSection)_log.ChildNodes[0]).Value += string.Format("{0}|{1}|{2}|{3}[{4}]|{5}\r\n", new object[]
                {
                    timestamp.ToString(Constants.DATETIME_FORMAT),
                    level,
                    assembly.Name,
                    fileName,
                    lineNumber,
                    message
                });
            _evnt.Set();
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

        #region StateContainerDelegates
        private XmlElement _FindContainer(string containerName)
        {
            foreach (XmlNode n in _doc.ChildNodes)
            {
                XmlElement tmp = _FindContainer(containerName, n);
                if (tmp != null)
                    return tmp;
            }
            XmlElement ret = _doc.CreateElement(containerName);
            _stateElement.AppendChild(ret);
            return ret;
        }
        private XmlElement _FindContainer(string containerName,XmlNode parent)
        {
            if (parent.NodeType == XmlNodeType.Element)
            {
                if (parent.Name == containerName)
                    return (XmlElement)parent;
            }
            foreach (XmlNode n in parent.ChildNodes)
            {
                XmlElement tmp = _FindContainer(containerName, n);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }
        internal XmlElement[] GetChildNodes(string containerName)
        {
            List<XmlElement> ret = new List<XmlElement>();
            _evnt.WaitOne();
            XmlElement cont = _FindContainer(containerName);
            foreach (XmlNode n in cont.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                    ret.Add((XmlElement)n);
            }
            _evnt.Set();
            return ret.ToArray();
        }
        internal void InsertBefore(string containerName, XmlElement element,XmlElement child)
        {
            _evnt.WaitOne();
            _FindContainer(containerName).InsertBefore(element, child);
            _evnt.Set();
        }
        internal void SetAttribute(XmlElement elem, string name, string value)
        {
            XmlAttribute att = _doc.CreateAttribute(name);
            att.Value = value;
            elem.Attributes.Append(att);
        }
        internal void AppendElement(string ContainerName, XmlElement element)
        {
            _evnt.WaitOne();
            _FindContainer(ContainerName).AppendChild(element);
            _evnt.Set();
        }
        internal XmlElement CreateElement(string elementName)
        {
            return _doc.CreateElement(elementName);
        }
        internal void EncodeVariableValue(object value, XmlElement elem)
        {
            Utility.EncodeVariableValue(value, elem, _doc);
        }
        #endregion
    }
}
