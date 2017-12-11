using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal struct sPathEntry
    {
        private string _incomingID;
        public string IncomingID { get { return _incomingID; } }
        private string[] _outgoingID;
        public string[] OutgoingID { get { return _outgoingID; } }
        private string _elementID;
        public string ElementID { get { return _elementID; } }
        private StepStatuses _status;
        public StepStatuses Status { get { return _status; } }
        private DateTime _startTime;
        public DateTime StartTime { get { return _startTime; } }
        private DateTime? _endTime;
        public DateTime? EndTime { get { return _endTime; } }
        private string _completedByID;
        public string CompletedByID { get { return _completedByID; } }

        public sPathEntry(XmlElement elem)
        {
            _incomingID = (elem.Attributes["incomingID"] == null ? null : elem.Attributes["incomingID"].Value);
            _elementID = elem.Attributes["elementID"].Value;
            _status = (StepStatuses)Enum.Parse(typeof(StepStatuses), elem.Attributes["status"].Value);
            _startTime = DateTime.ParseExact(elem.Attributes["startTime"].Value,Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
            _endTime = (elem.Attributes["endTime"] == null ? (DateTime?)null : DateTime.ParseExact(elem.Attributes["endTime"].Value, Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture));
            _completedByID = (elem.Attributes["CompletedByID"] != null ? elem.Attributes["CompletedByID"].Value : null);
            _outgoingID = null;
            if (elem.Attributes["outgoingID"] != null)
                _outgoingID = new string[] { elem.Attributes["outgoingID"].Value };
            else if (elem.ChildNodes.Count > 0)
            {
                List<string> tmp = new List<string>();
                foreach (XmlNode n in elem.ChildNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        switch (n.Name)
                        {
                            case "outgoing":
                                tmp.Add(n.InnerText);
                                break;
                        }
                    }
                }
                _outgoingID = tmp.ToArray();
            }
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = null;
            _endTime = null;
            _completedByID = null;
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID,DateTime endTime,string completedByID)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = null;
            _endTime = endTime;
            _completedByID = completedByID;
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID, DateTime endTime)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = null;
            _endTime = endTime;
            _completedByID = null;
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID, string[] outgoingID, DateTime? endTime)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = outgoingID;
            _endTime = endTime;
            _completedByID = null;
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID, string outgoingID, DateTime endTime, string completedByID)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = (outgoingID == null ? null : new string[] { outgoingID });
            _endTime = endTime;
            _completedByID = completedByID;
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID,string outgoingID, DateTime? endTime)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = (outgoingID == null ? null : new string[]{outgoingID});
            _endTime = endTime;
            _completedByID = null;
        }

        public void Append(XmlWriter writer)
        {
            writer.WriteStartElement("sPathEntry");
            if (_incomingID != null)
                writer.WriteAttributeString("incomingID", _incomingID);
            writer.WriteAttributeString("elementID", _elementID);
            writer.WriteAttributeString("status", _status.ToString());
            writer.WriteAttributeString("startTime", _startTime.ToString(Constants.DATETIME_FORMAT));
            if (_endTime.HasValue)
                writer.WriteAttributeString("endTime", _endTime.Value.ToString(Constants.DATETIME_FORMAT));
            if (_completedByID != null)
                writer.WriteAttributeString("CompletedByID", _completedByID);
            if (_outgoingID != null)
            {
                if (_outgoingID.Length == 1)
                    writer.WriteAttributeString("outgoingID", _outgoingID[0]);
                else
                {
                    foreach (string str in _outgoingID)
                    {
                        writer.WriteStartElement("outgoing");
                        writer.WriteValue(str);
                        writer.WriteEndElement();
                    }
                }
            }
            writer.WriteEndElement();
        }
    }

    internal struct sVariableEntry : IComparable
    {
        private string _name;
        public string Name { get { return _name; } }

        private object _value;
        public object Value { get { return _value; } }

        private int _pathStepIndex;
        public int PathStepIndex { get { return _pathStepIndex; } }

        public sVariableEntry(XmlElement elem)
        {
            _name = elem.Attributes["name"].Value;
            _pathStepIndex = int.Parse(elem.Attributes["pathStepIndex"].Value);
            if ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value) == VariableTypes.File)
                _value = new sFile((XmlElement)elem.ChildNodes[0]);
            else
            {
                if ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value) != VariableTypes.Null)
                {
                    if ((elem.Attributes["isArray"] == null ? false : bool.Parse(elem.Attributes["isArray"].Value)))
                    {
                        _value = null;
                        switch((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value))
                        {
                            case VariableTypes.Boolean:
                                _value = Array.CreateInstance(typeof(bool), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Byte:
                                _value = Array.CreateInstance(typeof(byte[]), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Char:
                                _value = Array.CreateInstance(typeof(char), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.DateTime:
                                _value = Array.CreateInstance(typeof(DateTime), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Decimal:
                                _value = Array.CreateInstance(typeof(decimal), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Double:
                                _value = Array.CreateInstance(typeof(double), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.File:
                                _value = Array.CreateInstance(typeof(sFile), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Float:
                                _value = Array.CreateInstance(typeof(float), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Integer:
                                _value = Array.CreateInstance(typeof(int), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Long:
                                _value = Array.CreateInstance(typeof(long), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Short:
                                _value = Array.CreateInstance(typeof(short), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.String:
                                _value = Array.CreateInstance(typeof(string), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Hashtable:
                                _value = Array.CreateInstance(typeof(Hashtable), elem.ChildNodes.Count);
                                break;
                        }
                        for(int x = 0; x < elem.ChildNodes.Count; x++)
                        {
                            string text = elem.ChildNodes[x].InnerText;
                            if (elem.ChildNodes[x].ChildNodes[0].NodeType == XmlNodeType.CDATA)
                                text = ((XmlCDataSection)elem.ChildNodes[x].ChildNodes[0]).InnerText;
                            ((Array)_value).SetValue(Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value), text), x);
                        }
                    }
                    else
                    {
                        string text = elem.InnerText;
                        if (elem.ChildNodes[0].NodeType == XmlNodeType.CDATA)
                            text = ((XmlCDataSection)elem.ChildNodes[0]).InnerText;
                        _value = Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value), text);
                    }
                }
                else
                    _value = null;
            }
        }

        public sVariableEntry(string name, int pathStepIndex, object value)
        {
            _name = name;
            _value = value;
            _pathStepIndex = pathStepIndex;
        }

        public void Append(XmlWriter writer)
        {
            writer.WriteStartElement("sVariableEntry");
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("pathStepIndex", _pathStepIndex.ToString());
            Utility.EncodeVariableValue(_value, writer);
            writer.WriteEndElement();
        }

        public int CompareTo(object obj)
        {
            return _pathStepIndex.CompareTo(((sVariableEntry)obj).PathStepIndex);
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

        public void Append(XmlWriter writer)
        {
            writer.WriteStartElement("sStepSuspension");
            writer.WriteAttributeString("id", _id);
            writer.WriteAttributeString("stepIndex", _stepIndex.ToString());
            writer.WriteAttributeString("endTime", _endTime.ToString(Constants.DATETIME_FORMAT));
            writer.WriteEndElement();
        }
    }

    internal struct sSuspendedStep
    {
        private string _incomingID;
        public string IncomingID { get { return _incomingID; } }

        private string _elementID;
        public string ElementID { get { return _elementID; } }

        public sSuspendedStep(string incomingID,string elementID)
        {
            _incomingID = incomingID;
            _elementID = elementID;
        }
    }

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
            :this(name,extension,null,new byte[0])
        {}

        public sFile(string name, string extension, byte[] content)
            : this(name, extension, null, content)
        {}

        public sFile(string name,string extension,string contentType,byte[] content)
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

        internal void Append(XmlWriter writer)
        {
            writer.WriteStartElement("sFile");
            writer.WriteAttributeString("Name", _name);
            writer.WriteAttributeString("Extension", _extension);
            if (_contentType != null)
                writer.WriteAttributeString("ContentType", _contentType);
            if (_content.Length > 0)
                writer.WriteCData(Convert.ToBase64String(_content));
            writer.WriteEndElement();
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
}
