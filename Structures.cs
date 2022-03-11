using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    /// <summary>
    /// This structure is used to house a File associated within a process instance.  It is used to both store, encode, decode and retreive File variables inside the process state.
    /// </summary>
    public struct sFile
    {
        private string _name;
        /// <summary>
        /// The name of the File.
        /// </summary>
        public string Name { get { return _name; } }

        private string _extension;
        /// <summary>
        /// The extension of the File.
        /// </summary>
        public string Extension { get { return _extension; } }

        private string _contentType;
        /// <summary>
        /// The content type tag for the File.  e.g. text/html
        /// </summary>
        public string ContentType { get { return _contentType; } }

        private byte[] _content;
        /// <summary>
        /// The binary content of the File.
        /// </summary>
        public byte[] Content { get { return _content; } }

        /// <summary>
        /// Create a new Empty File, only specifying the name and extension.
        /// </summary>
        /// <param name="name">Becomes the Name property of the File.</param>
        /// <param name="extension">Becomes the Extension property of the File.</param>
        public sFile(string name, string extension)
            : this(name, extension, null, new byte[0])
        { }

        /// <summary>
        /// Create with new File with a given content.
        /// </summary>
        /// <param name="name">Becomes the Name property of the File.</param>
        /// <param name="extension">Becomes the Extension property of the File.</param>
        /// <param name="content">Becomes the Content property of the File.</param>
        public sFile(string name, string extension, byte[] content)
            : this(name, extension, null, content)
        { }

        /// <summary>
        /// Create with new File with a given content and Content Type.
        /// </summary>
        /// <param name="name">Becomes the Name property of the File.</param>
        /// <param name="extension">Becomes the Extension property of the File.</param>
        /// <param name="contentType">Becomes the ContentType property of the File.</param>
        /// <param name="content">Becomes the Content property of the File.</param>
        public sFile(string name, string extension, string contentType, byte[] content)
        {
            _name = name;
            _extension = extension;
            _contentType = contentType;
            _content = content;
        }

        internal sFile(XmlElement elem)
        {
            _name = elem.Attributes["Name"].Value;
            _extension = elem.Attributes["Extension"].Value;
            _contentType = (elem.Attributes["ContentType"] == null ? null : elem.Attributes["ContentType"].Value);
            _content = new byte[0];
            if (elem.ChildNodes.Count > 0)
                _content = Convert.FromBase64String(((XmlCDataSection)elem.ChildNodes[0]).InnerText);
        }

        internal XmlElement ToElement(XmlDocument doc)
        {
            XmlElement ret = doc.CreateElement("sFile");
            ret.Attributes.Append(doc.CreateAttribute("Name"));
            ret.Attributes["Name"].Value = _name;
            ret.Attributes.Append(doc.CreateAttribute("Extension"));
            ret.Attributes["Extension"].Value = _extension;
            if (_contentType != null)
            {
                ret.Attributes.Append(doc.CreateAttribute("ContentType"));
                ret.Attributes["ContentType"].Value = _contentType;
            }
            if (_content.Length > 0)
                ret.AppendChild(doc.CreateCDataSection(Convert.ToBase64String(_content)));
            return ret;
        }
    }

    /// <summary>
    /// This structure is used to specify a Process Runtime Constant.  These Constants are used as a Dynamic Constant, so a read only variable within the process that can be unique to the instance running, only a constant to that specific process instance.
    /// </summary>
    public struct sProcessRuntimeConstant
    {
        private string _name;
        /// <summary>
        /// The Name of the variable.
        /// </summary>
        public string Name { get { return _name; } }

        private object _value;
        /// <summary>
        /// The Value of the variable.
        /// </summary>
        public object Value { get { return _value; } }

        /// <summary>
        /// Create a new Process Runtime Constant with the given name and value.
        /// </summary>
        /// <param name="name">Becomes the Name property of the File.</param>
        /// <param name="value">Becomes the Value property of the File.</param>
        public sProcessRuntimeConstant(string name,object value)
        {
            _name = name;
            _value = value;
        }
    }

    internal struct sProcessSuspendEvent : IComparable
    {
        private BusinessProcess _process;
        public BusinessProcess Process { get { return _process; } }
        private AEvent _event;
        public AEvent Event { get { return _event; } }
        private DateTime _endTime;
        public DateTime EndTime { get { return _endTime; } }

        public sProcessSuspendEvent(BusinessProcess process,AEvent evnt,TimeSpan time){
            _process = process;
            _event = evnt;
            _endTime = DateTime.Now.Add(time);
        }

        public int CompareTo(object obj)
        {
            DateTime compare = DateTime.MaxValue;
            if (obj is sProcessSuspendEvent)
                compare = ((sProcessSuspendEvent)obj).EndTime;
            else if (obj is sProcessDelayedEvent)
                compare = ((sProcessDelayedEvent)obj).StartTime;
            return _endTime.CompareTo(compare);
        }
    }

    internal struct sProcessDelayedEvent : IComparable
    {
        private BusinessProcess _process;
        public BusinessProcess Process { get { return _process; } }
        private BoundaryEvent _event;
        public BoundaryEvent Event { get { return _event; } }
        private DateTime _startTime;
        public DateTime StartTime { get { return _startTime; } }
        private string _sourceID;
        public string SourceID { get { return _sourceID; } }

        public sProcessDelayedEvent(BusinessProcess process, BoundaryEvent evnt, TimeSpan time,string sourceID)
        {
            _process = process;
            _event = evnt;
            _startTime = DateTime.Now.Add(time);
            _sourceID=sourceID;
        }

        public int CompareTo(object obj)
        {
            DateTime compare = DateTime.MaxValue;
            if (obj is sProcessSuspendEvent)
                compare = ((sProcessSuspendEvent)obj).EndTime;
            else if (obj is sProcessDelayedEvent)
                compare = ((sProcessDelayedEvent)obj).StartTime;
            return _startTime.CompareTo(compare);
        }
    }
}
