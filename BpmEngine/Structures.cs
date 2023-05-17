using BpmEngine.Elements;
using BpmEngine.Elements.Processes.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace BpmEngine
{
    /// <summary>
    /// This structure is used to house a File associated within a process instance.  It is used to both store, encode, decode and retreive File variables inside the process state.
    /// </summary>
    public readonly struct sFile 
    {
        private readonly string _name;
        /// <summary>
        /// The name of the File.
        /// </summary>
        public string Name { get { return _name; } }

        private readonly string _extension;
        /// <summary>
        /// The extension of the File.
        /// </summary>
        public string Extension { get { return _extension; } }

        private readonly string _contentType;
        /// <summary>
        /// The content type tag for the File.  e.g. text/html
        /// </summary>
        public string ContentType { get { return _contentType; } }

        private readonly byte[] _content;
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
            _contentType = (contentType==String.Empty ? null : contentType);
            _content = content;
        }

        internal sFile(DefinitionFile file)
        {
            _name=file.Name;
            _extension=file.Extension;
            _contentType = file.ContentType;
            _content = file.Content;
        }

        public override bool Equals(object obj)
        {
            if (obj is sFile fle)
            {
                return fle.Name.Equals(Name) &&
                    fle.Extension.Equals(Extension) &&
                    _dataEquals(fle.Content);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}:{2}", new object[]
            {
                _name,
                _extension,
                Convert.ToBase64String(_content)
            }).GetHashCode();
        }

        private bool _dataEquals(byte[] content)
        {
            if (_content.Length==content.Length)
                return !_content.Select((b, i) => new { val = b, index = i }).Any(o => content[o.index]!=o.val);
            return false;
        }
    }

    /// <summary>
    /// This structure is used to specify a Process Runtime Constant.  These Constants are used as a Dynamic Constant, so a read only variable within the process that can be unique to the instance running, only a constant to that specific process instance.
    /// </summary>
    public readonly struct SProcessRuntimeConstant
    {
        private readonly string _name;
        /// <summary>
        /// The Name of the variable.
        /// </summary>
        public string Name => _name;

        private readonly object _value;
        /// <summary>
        /// The Value of the variable.
        /// </summary>
        public object Value => _value;

        /// <summary>
        /// Create a new Process Runtime Constant with the given name and value.
        /// </summary>
        /// <param name="name">Becomes the Name property of the variable.</param>
        /// <param name="value">Becomes the Value property of the variable.</param>
        public SProcessRuntimeConstant(string name,object value)
        {
            _name = name;
            _value = value;
        }
    }

    internal readonly struct SProcessSuspendEvent
    {
        private readonly ProcessInstance _instance;
        public ProcessInstance Instance => _instance;
        private readonly AEvent _event;
        public AEvent Event => _event;
        private readonly DateTime _endTime;
        public DateTime EndTime => _endTime;

        public SProcessSuspendEvent(ProcessInstance instance, AEvent evnt, TimeSpan time)
        {
            _instance = instance;
            _event = evnt;
            _endTime = DateTime.Now.Add(time);
        }
    }

    internal readonly struct SProcessDelayedEvent
    {
        private readonly ProcessInstance _instance;
        public ProcessInstance Instance { get { return _instance; } }
        private readonly BoundaryEvent _event;
        public BoundaryEvent Event { get { return _event; } }
        private readonly DateTime _startTime;
        public DateTime StartTime { get { return _startTime; } }
        private readonly string _sourceID;
        public string SourceID { get { return _sourceID; } }

        public SProcessDelayedEvent(ProcessInstance instance, BoundaryEvent evnt, TimeSpan time, string sourceID)
        {
            _instance = instance;
            _event = evnt;
            _startTime = DateTime.Now.Add(time);
            _sourceID=sourceID;
        }
    }
}
