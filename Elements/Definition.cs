using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn","definitions")]
    [RequiredAttribute("id")]
    [ValidParent(null)]
    internal class Definition : AParentElement
    {
        public Definition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public Diagram[] Diagrams
        {
            get
            {
                List<Diagram> ret = new List<Diagram>();
                foreach (IElement elem in Children)
                {
                    if (elem is Diagram)
                        ret.Add((Diagram)elem);
                }
                return ret.ToArray();
            }
        }

        public MessageFlow[] MessageFlows
        {
            get
            {
                List<MessageFlow> ret = new List<MessageFlow>();
                foreach (IElement elem in Children)
                {
                    if (elem is Collaboration)
                    {
                        Collaboration collab = (Collaboration)elem;
                        foreach (IElement msg in collab.Children)
                        {
                            if (msg is MessageFlow)
                                ret.Add((MessageFlow)msg);
                        }
                        break;
                    }
                }
                return ret.ToArray();
            }
        }

        public IElement LocateElement(string id)
        {
            return _RecurLocateElement(this, id);
        }

        private IElement _RecurLocateElement(IElement elem, string id)
        {
            if (elem.id == id)
                return elem;
            IElement ret = null;
            if (elem is IParentElement)
            {
                foreach (IElement selem in ((IParentElement)elem).Children)
                {
                    ret = _RecurLocateElement(selem, id);
                    if (ret != null)
                        break;
                }
            }
            return ret;
        }

        public IElement[] LocateElementsOfType(Type type)
        {
            return _RecurLocateElementsOfType(this,type);
        }

        private IElement[] _RecurLocateElementsOfType(IElement elem, Type type)
        {
            List<IElement> ret = new List<Interfaces.IElement>();
            if (type.Equals(elem.GetType()))
                ret.Add(elem);
            if (elem is IParentElement)
            {
                foreach (IElement selem in ((IParentElement)elem).Children)
                {
                    ret.AddRange(_RecurLocateElementsOfType(selem, type));
                }
            }
            return ret.ToArray();
        }

        public override bool IsValid(out string[] err)
        {
            if (Children.Length == 0)
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }

        private BusinessProcess _owningProcess;
        internal BusinessProcess OwningProcess
        {
            set { _owningProcess = value; }
        }

        internal void Debug(IElement element,string message)
        {
            if (_owningProcess != null) 
                _owningProcess.WriteLogLine(element, LogLevels.Debug, new StackFrame(2, true), DateTime.Now, message);
        }

        internal void Debug(IElement element, string message, object[] pars)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogLine(element, LogLevels.Debug, new StackFrame(2, true), DateTime.Now, string.Format(message,pars));
        }
        internal void Info(IElement element,string message)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogLine(element, LogLevels.Info, new StackFrame(2, true), DateTime.Now, message);
        }
        internal void Info(IElement element, string message, object[] pars)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogLine(element, LogLevels.Info, new StackFrame(2, true), DateTime.Now, string.Format(message,pars));
        }
        internal void Error(IElement element, string message)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogLine(element, LogLevels.Error, new StackFrame(2, true), DateTime.Now, message);
        }
        internal void Error(IElement element, string message, object[] pars)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogLine(element, LogLevels.Error, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        }
        internal void Fatal(IElement element, string message)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogLine(element, LogLevels.Fatal, new StackFrame(2, true), DateTime.Now, message);
        }
        internal void Fatal(IElement element, string message, object[] pars)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogLine(element, LogLevels.Fatal, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        }
        internal Exception Exception(IElement element, Exception exception)
        {
            if (_owningProcess != null)
                _owningProcess.WriteLogException(element, new StackFrame(2, true), DateTime.Now, exception);
            return exception;
        }
    }
}
