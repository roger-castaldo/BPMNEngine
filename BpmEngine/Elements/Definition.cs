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
            return (this.id==id
                ? this
                : Children.Traverse(ielem => (ielem is AParentElement ? ((AParentElement)ielem).Children : Array.Empty<IElement>())).FirstOrDefault(elem => elem.id==id));
        }

        public IEnumerable<T> LocateElementsOfType<T>() where T: IElement
        {
            return Children.Traverse(ielem => (ielem is AParentElement ? ((AParentElement)ielem).Children : Array.Empty<IElement>())).OfType<T>();
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
