using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.State
{
    internal class StateVariableContainer : AStateContainer
    {
        private const string _CONTAINER_NAME = "ProcessVariables";
        private const string _VARIABLE_ENTRY_ELEMENT = "sVariableEntry";
        private const string _PATH_STEP_INDEX = "pathStepIndex";
        private const string _NAME = "name";
        private const string _IS_ARRAY = "isArray";
        private const string _TYPE = "type";
        private const string _VALUE = "Value";

        protected override string _ContainerName
        {
            get
            {
                return _CONTAINER_NAME;
            }
        }

        public StateVariableContainer(ProcessState state)
            : base(state)
        {
        }

        public object this[string variableName,int stepIndex]
        {
            get {
                return ConvertValue((XmlElement?)ChildNodes.Cast<XmlNode>()
                    .LastOrDefault(elem => elem.Attributes[_NAME].Value==variableName
                    && int.Parse(elem.Attributes[_PATH_STEP_INDEX].Value) <= stepIndex));
            }
            set
            {
                XmlElement elem = _EncodeVariable(variableName, stepIndex, value);
                bool add = true;
                foreach (XmlElement e in ChildNodes)
                {
                    if (int.Parse(e.Attributes[_PATH_STEP_INDEX].Value) >= stepIndex)
                    {
                        add = false;
                        _InsertBefore(elem, e);
                    }
                }
                if (add)
                    _AppendElement(elem);
            }
        }

        public IEnumerable<string> this[int stepIndex]
        {
            get
            {
                return ChildNodes.Cast<XmlNode>()
                    .Where(elem => int.Parse(elem.Attributes[_PATH_STEP_INDEX].Value) <= stepIndex)
                    .GroupBy(elem => elem.Attributes[_NAME].Value)
                    .Where(grp => (VariableTypes)Enum.Parse(typeof(VariableTypes), grp.Last().Attributes[_TYPE].Value) != VariableTypes.Null)
                    .Select(grp => grp.Key);
            }
        }

        private static object ConvertValue(XmlElement? elem)
        {
            if (elem==null)
                return null;
            object ret = null;
            if ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes[_TYPE].Value) == VariableTypes.File)
                ret = new sFile((XmlElement)elem.ChildNodes[0]);
            else
            {
                if ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes[_TYPE].Value) != VariableTypes.Null)
                {
                    if ((elem.Attributes[_IS_ARRAY] !=null)&&bool.Parse(elem.Attributes[_IS_ARRAY].Value))
                    {
                        switch ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes[_TYPE].Value))
                        {
                            case VariableTypes.Boolean:
                                ret = Array.CreateInstance(typeof(bool), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Byte:
                                ret = Array.CreateInstance(typeof(byte[]), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Char:
                                ret = Array.CreateInstance(typeof(char), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.DateTime:
                                ret = Array.CreateInstance(typeof(DateTime), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Decimal:
                                ret = Array.CreateInstance(typeof(decimal), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Double:
                                ret = Array.CreateInstance(typeof(double), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.File:
                                ret = Array.CreateInstance(typeof(sFile), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Float:
                                ret = Array.CreateInstance(typeof(float), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Integer:
                                ret = Array.CreateInstance(typeof(int), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Long:
                                ret = Array.CreateInstance(typeof(long), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Short:
                                ret = Array.CreateInstance(typeof(short), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.String:
                                ret = Array.CreateInstance(typeof(string), elem.ChildNodes.Count);
                                break;
                            case VariableTypes.Guid:
                                ret = Array.CreateInstance(typeof(Guid), elem.ChildNodes.Count);
                                break;
                        }
                        for (int x = 0; x < elem.ChildNodes.Count; x++)
                        {
                            string text = ((XmlCDataSection)elem.ChildNodes[x].ChildNodes[0]).InnerText;
                            ((Array)ret).SetValue(Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes[_TYPE].Value), text), x);
                        }
                    }
                    else
                    {
                        string text = ((XmlCDataSection)elem.ChildNodes[0]).InnerText;
                        ret = Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes[_TYPE].Value), text);
                    }
                }
                else
                    ret = null;
            }
            return ret;
        }

        private XmlElement _EncodeVariable(string variableName, int stepIndex, object value)
        {
            XmlElement elem = _ProduceElement(_VARIABLE_ENTRY_ELEMENT);
            _SetAttribute(elem, _PATH_STEP_INDEX, stepIndex.ToString());
            _SetAttribute(elem, _NAME, variableName);
            if (value == null)
                _SetAttribute(elem, _TYPE, VariableTypes.Null.ToString());
            else
            {
                if (value.GetType().IsArray && value.GetType().FullName != "System.Byte[]" && value.GetType().FullName!="System.String")
                {
                    _SetAttribute(elem, _IS_ARRAY, true.ToString());
                    _SetTypeAttribute(elem, value);
                    if (value.GetType().GetElementType().FullName == typeof(sFile).FullName)
                    {
                        foreach (sFile sf in (IEnumerable)value)
                        {
                            elem.AppendChild(_ProduceElement(_VALUE));
                            elem.ChildNodes[elem.ChildNodes.Count-1].AppendChild(_EncodeFile(sf));
                        }
                    }
                    else
                    {
                        foreach (object val in (IEnumerable)value)
                        {
                            elem.AppendChild(_ProduceElement(_VALUE));
                            elem.ChildNodes[elem.ChildNodes.Count-1].AppendChild(_EncodeCData(_EncodeValue(val)));
                        }
                    }
                }
                else
                {
                    _SetAttribute(elem, _IS_ARRAY, false.ToString());
                    _SetTypeAttribute(elem, value);
                    if (value is sFile)
                        elem.AppendChild(_EncodeFile((sFile)value));
                    else
                        elem.AppendChild(_EncodeCData(_EncodeValue(value)));
                }
            }
            return elem;
        }

        private void _SetTypeAttribute(XmlElement elem,object value)
        {
            string fullName = (value.GetType().IsArray && value.GetType().FullName != "System.Byte[]" && value.GetType().FullName!="System.String" ? value.GetType().GetElementType().FullName : value.GetType().FullName);
            if (fullName==typeof(sFile).FullName)
                _SetAttribute(elem, _TYPE, VariableTypes.File.ToString());
            else
            {
                switch (fullName)
                {
                    case "System.Boolean":
                        _SetAttribute(elem, _TYPE, VariableTypes.Boolean.ToString());
                        break;
                    case "System.Byte[]":
                        _SetAttribute(elem, _TYPE, VariableTypes.Byte.ToString());
                        break;
                    case "System.Char":
                        _SetAttribute(elem, _TYPE, VariableTypes.Char.ToString());
                        break;
                    case "System.DateTime":
                        _SetAttribute(elem, _TYPE, VariableTypes.DateTime.ToString());
                        break;
                    case "System.Decimal":
                        _SetAttribute(elem, _TYPE, VariableTypes.Decimal.ToString());
                        break;
                    case "System.Double":
                        _SetAttribute(elem, _TYPE, VariableTypes.Double.ToString());
                        break;
                    case "System.Single":
                        _SetAttribute(elem, _TYPE, VariableTypes.Float.ToString());
                        break;
                    case "System.Int32":
                        _SetAttribute(elem, _TYPE, VariableTypes.Integer.ToString());
                        break;
                    case "System.Int64":
                        _SetAttribute(elem, _TYPE, VariableTypes.Long.ToString());
                        break;
                    case "System.Int16":
                        _SetAttribute(elem, _TYPE, VariableTypes.Short.ToString());
                        break;
                    case "System.UInt32":
                        _SetAttribute(elem, _TYPE, VariableTypes.UnsignedInteger.ToString());
                        break;
                    case "System.UInt64":
                        _SetAttribute(elem, _TYPE, VariableTypes.UnsignedLong.ToString());
                        break;
                    case "System.UInt16":
                        _SetAttribute(elem, _TYPE, VariableTypes.UnsignedShort.ToString());
                        break;
                    case "System.String":
                        _SetAttribute(elem, _TYPE, VariableTypes.String.ToString());
                        break;
                    case "System.Guid":
                        _SetAttribute(elem, _TYPE, VariableTypes.Guid.ToString());
                        break;
                }
            }
        }

        private string _EncodeValue(object value)
        {
            string text = "";
            switch (value.GetType().FullName)
            {
                case "System.Double":
                    text = ((double)value).ToString("R");
                    break;
                case "System.Single":
                    text = ((float)value).ToString("R");
                    break;
                case "System.Byte[]":
                    text = Convert.ToBase64String((byte[])value);
                    break;
                default:
                    text = value.ToString();
                    break;
            }
            return text;
        }

        public static Dictionary<string,object> ExtractVariables(XmlDocument doc)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            var grps = doc.GetElementsByTagName(_CONTAINER_NAME).Cast<XmlNode>()
                .SelectMany(node => node.ChildNodes.Cast<XmlNode>())
                .Where(cnode => cnode.NodeType==XmlNodeType.Element)
                .GroupBy(node => node.Attributes[_NAME].Value);
            foreach (var grp in grps)
                ret.Add(grp.Key, ConvertValue((XmlElement)grp.Last()));
            return ret;
        }
    }
}
