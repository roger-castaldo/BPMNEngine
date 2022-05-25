using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.State
{
    internal abstract class AStateContainer
    {

        protected abstract string _ContainerName { get; }

        private ProcessState _state;

        protected ProcessState State { get { return _state; } }
        protected XmlElement[] ChildNodes
        {
            get { return _state.GetChildNodes(_ContainerName); }
        }

        public AStateContainer(ProcessState state)
        {
            _state = state;
        }

        protected XmlElement _ProduceElement(string name)
        {
            return _state.CreateElement(name);
        }

        protected void AppendValue(string content)
        {
            _state.AppendValue(_ContainerName, content);
        }

        protected void _SetAttribute(XmlElement element,string name,string value)
        {
            _state.SetAttribute(element, name, value);
        }

        protected void _AppendElement(XmlElement element)
        {
            _state.AppendElement(_ContainerName, element);
        }
        protected void _InsertBefore(XmlElement element,XmlElement child)
        {
            _state.InsertBefore(_ContainerName, element, child);
        }
        protected void _EncodeVariableValue(object value,XmlElement elem)
        {
            _state.EncodeVariableValue(value, elem);
        }

        protected void _WriteLogLine(string elementID,LogLevels level,string message)
        {
            _state.Process.WriteLogLine(elementID, level, new System.Diagnostics.StackFrame(2, true), DateTime.Now, message);
        }

        protected void _WriteException(string elementID,Exception exception)
        {
            _state.Process.WriteLogException(elementID, new System.Diagnostics.StackFrame(2, true), DateTime.Now, exception);
        }
    }
}
