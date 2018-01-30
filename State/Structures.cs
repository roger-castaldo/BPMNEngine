using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.State
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

    internal struct sStepSuspension
    {
        private string _id;
        public string id { get { return _id; } }

        private int _stepIndex;
        public int StepIndex { get { return _stepIndex; } }

        private DateTime _endTime;
        public DateTime EndTime { get { return _endTime; } }

        public sStepSuspension(string id, int stepIndex, TimeSpan span)
        {
            _id = id;
            _stepIndex = stepIndex;
            _endTime = DateTime.Now.Add(span);
        }

        public sStepSuspension(XmlElement elem)
        {
            _id = elem.Attributes["id"].Value;
            _stepIndex = int.Parse(elem.Attributes["stepIndex"].Value);
            _endTime = DateTime.ParseExact(elem.Attributes["endTime"].Value, Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
        }

    }

    internal struct sSuspendedStep
    {
        private string _incomingID;
        public string IncomingID { get { return _incomingID; } }

        private string _elementID;
        public string ElementID { get { return _elementID; } }

        public sSuspendedStep(string incomingID, string elementID)
        {
            _incomingID = incomingID;
            _elementID = elementID;
        }
    }
}
