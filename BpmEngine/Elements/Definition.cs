using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public IEnumerable<Diagram> Diagrams => Children.OfType<Diagram>();

        public IEnumerable<MessageFlow> MessageFlows => LocateElementsOfType<MessageFlow>();

        public IElement LocateElement(string id)
        {
            return _RecurLocateElement(this, id);
        }

        private IElement _RecurLocateElement(IElement elem, string id)
        {
            if (elem.id == id)
                return elem;
            IElement ret = null;
            if (elem is IParentElement element)
            {
                foreach (IElement selem in element.Children)
                {
                    ret = _RecurLocateElement(selem, id);
                    if (ret != null)
                        break;
                }
            }
            return ret;
        }

        public IEnumerable<T> LocateElementsOfType<T>() where T: IElement
        {
            return _RecurLocateElementsOfType<T>(this);
        }

        private IEnumerable<T> _RecurLocateElementsOfType<T>(IElement elem) where T : IElement
        {
            var result = new List<T>();
            if (typeof(T).Equals(elem.GetType()))
                result.Add((T)elem);
            if (elem is IParentElement element)
            {
                foreach (IElement selem in element.Children)
                    result.AddRange(_RecurLocateElementsOfType<T>(selem));
            }
            return result;
        }

        public override bool IsValid(out string[] err)
        {
            if (!Children.Any())
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }

        internal BusinessProcess OwningProcess { get; set; }

        internal void Debug(IElement element,string message)
        {
            if (OwningProcess != null) 
                OwningProcess.WriteLogLine(element, LogLevels.Debug, new StackFrame(2, true), DateTime.Now, message);
        }

        internal void Debug(IElement element, string message, object[] pars)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogLine(element, LogLevels.Debug, new StackFrame(2, true), DateTime.Now, string.Format(message,pars));
        }
        internal void Info(IElement element,string message)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogLine(element, LogLevels.Info, new StackFrame(2, true), DateTime.Now, message);
        }
        internal void Info(IElement element, string message, object[] pars)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogLine(element, LogLevels.Info, new StackFrame(2, true), DateTime.Now, string.Format(message,pars));
        }
        internal void Error(IElement element, string message)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogLine(element, LogLevels.Error, new StackFrame(2, true), DateTime.Now, message);
        }
        internal void Error(IElement element, string message, object[] pars)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogLine(element, LogLevels.Error, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        }
        internal void Fatal(IElement element, string message)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogLine(element, LogLevels.Fatal, new StackFrame(2, true), DateTime.Now, message);
        }
        internal void Fatal(IElement element, string message, object[] pars)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogLine(element, LogLevels.Fatal, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        }
        internal Exception Exception(IElement element, Exception exception)
        {
            if (OwningProcess != null)
                OwningProcess.WriteLogException(element, new StackFrame(2, true), DateTime.Now, exception);
            return exception;
        }
    }
}
