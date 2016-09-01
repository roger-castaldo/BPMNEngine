using System;
using System.Collections.Generic;
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

        public sPathEntry(XmlElement elem)
        {
            _incomingID = (elem.Attributes["incomingID"] == null ? null : elem.Attributes["incomingID"].Value);
            _elementID = elem.Attributes["elementID"].Value;
            _status = (StepStatuses)Enum.Parse(typeof(StepStatuses), elem.Attributes["status"].Value);
            _startTime = DateTime.Parse(elem.Attributes["startTime"].Value);
            _endTime = (elem.Attributes["endTime"] == null ? (DateTime?)null : DateTime.Parse(elem.Attributes["endTime"].Value));
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
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID,DateTime endTime)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = null;
            _endTime = endTime;
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID, string[] outgoingID, DateTime? endTime)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = outgoingID;
            _endTime = endTime;
        }

        public sPathEntry(string elementID, StepStatuses status, DateTime startTime, string incomingID,string outgoingID, DateTime? endTime)
        {
            _elementID = elementID;
            _status = status;
            _startTime = startTime;
            _incomingID = incomingID;
            _outgoingID = (outgoingID == null ? null : new string[]{outgoingID});
            _endTime = endTime;
        }

        public void Append(XmlWriter writer)
        {
            writer.WriteStartElement("sPathEntry");
            if (_incomingID != null)
                writer.WriteAttributeString("incomingID", _incomingID);
            writer.WriteAttributeString("elementID", _elementID);
            writer.WriteAttributeString("status", _status.ToString());
            writer.WriteAttributeString("startTime", _startTime.ToString());
            if (_endTime.HasValue)
                writer.WriteAttributeString("endtime", _endTime.Value.ToString());
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
            _value = null;
            switch ((VariableTypes)Enum.Parse(typeof(VariableTypes),elem.Attributes["type"].Value))
            {
                case VariableTypes.Boolean:
                    _value = bool.Parse(elem.InnerText);
                    break;
                case VariableTypes.Byte:
                    _value = Convert.FromBase64String(elem.InnerText);
                    break;
                case VariableTypes.Char:
                    _value = elem.InnerText[0];
                    break;
                case VariableTypes.DateTime:
                    _value = DateTime.Parse(elem.InnerText);
                    break;
                case VariableTypes.Decimal:
                    _value = decimal.Parse(elem.InnerText);
                    break;
                case VariableTypes.Double:
                    _value = double.Parse(elem.InnerText);
                    break;
                case VariableTypes.Float:
                    _value = float.Parse(elem.InnerText);
                    break;
                case VariableTypes.Integer:
                    _value = int.Parse(elem.InnerText);
                    break;
                case VariableTypes.Long:
                    _value = long.Parse(elem.InnerText);
                    break;
                case VariableTypes.Short:
                    _value = short.Parse(elem.InnerText);
                    break;
                case VariableTypes.String:
                    _value = elem.InnerText;
                    break;
            }
        }

        public sVariableEntry(string name, int pathStepIndex, object value)
        {
            _name = name;
            _pathStepIndex = pathStepIndex;
            _value = value;
        }

        public void Append(XmlWriter writer)
        {
            writer.WriteStartElement("sVariableEntry");
            writer.WriteAttributeString("name", _name);
            writer.WriteAttributeString("pathStepIndex", _pathStepIndex.ToString());
            if (_value == null)
                writer.WriteAttributeString("type",VariableTypes.Null.ToString());
            else
            {
                switch (_value.GetType().FullName)
                {
                    case "System.Boolean":
                        writer.WriteAttributeString("type", VariableTypes.Boolean.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.Byte[]":
                        writer.WriteAttributeString("type", VariableTypes.Byte.ToString());
                        writer.WriteValue(Convert.ToBase64String((byte[])_value));
                        break;
                    case "System.Char":
                        writer.WriteAttributeString("type", VariableTypes.Char.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.DateTime":
                        writer.WriteAttributeString("type", VariableTypes.DateTime.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.Decimal":
                        writer.WriteAttributeString("type", VariableTypes.Decimal.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.Double":
                        writer.WriteAttributeString("type", VariableTypes.Double.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.Single":
                        writer.WriteAttributeString("type", VariableTypes.Float.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.Int32":
                        writer.WriteAttributeString("type", VariableTypes.Integer.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.Int64":
                        writer.WriteAttributeString("type", VariableTypes.Long.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.Int16":
                        writer.WriteAttributeString("type", VariableTypes.Short.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                    case "System.String":
                        writer.WriteAttributeString("type", VariableTypes.String.ToString());
                        writer.WriteValue(_value.ToString());
                        break;
                }
            }
            writer.WriteEndElement();
        }

        public int CompareTo(object obj)
        {
            return _pathStepIndex.CompareTo(((sVariableEntry)obj).PathStepIndex);
        }
    }
}
