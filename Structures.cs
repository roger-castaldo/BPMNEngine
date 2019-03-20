using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    public struct sFile
    {
        private string _name;
        public string Name { get { return _name; } }

        private string _extension;
        public string Extension { get { return _extension; } }

        private string _contentType;
        public string ContentType { get { return _contentType; } }

        private byte[] _content;
        public byte[] Content { get { return _content; } }

        public sFile(string name, string extension)
            : this(name, extension, null, new byte[0])
        { }

        public sFile(string name, string extension, byte[] content)
            : this(name, extension, null, content)
        { }

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

    public struct sProcessRuntimeConstant
    {
        private string _name;
        public string Name { get { return _name; } }

        private object _value;
        public object Value { get { return _value; } }

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
            return _endTime.CompareTo(((sProcessSuspendEvent)obj)._endTime);
        }
    }
}
