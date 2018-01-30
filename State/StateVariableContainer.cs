using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.State
{
    internal class StateVariableContainer : AStateContainer
    {
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
                return "ProcessVariables";
            }
        }

        public StateVariableContainer(ProcessState state)
            : base(state)
        {
        }

        public object this[string variableName,int stepIndex]
        {
            get {
                object ret = null;
                foreach (XmlElement elem in ChildNodes)
                {
                    if (int.Parse(elem.Attributes[_PATH_STEP_INDEX].Value) <= stepIndex)
                    {
                        if (elem.Attributes[_NAME].Value == variableName)
                            ret = ConvertValue(elem);
                    }
                }
                return ret;
            }
            set
            {
                XmlElement elem = _ProduceElement(_VARIABLE_ENTRY_ELEMENT);
                _SetAttribute(elem, _PATH_STEP_INDEX, stepIndex.ToString());
                _SetAttribute(elem, _NAME, variableName);
                _EncodeVariableValue(value, elem);
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

        public string[] this[int stepIndex]
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (XmlElement elem in ChildNodes)
                {
                    if (int.Parse(elem.Attributes[_PATH_STEP_INDEX].Value) <= stepIndex)
                    {
                        if (((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes[_TYPE].Value) == VariableTypes.Null))
                            ret.Remove(elem.Attributes[_NAME].Value);
                        else if (!ret.Contains(elem.Attributes[_NAME].Value))
                            ret.Add(elem.Attributes[_NAME].Value);

                    }
                }
                return ret.ToArray();
            }
        }

        private object ConvertValue(XmlElement elem)
        {
            object ret = null;
            if ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value) == VariableTypes.File)
                ret = new sFile((XmlElement)elem.ChildNodes[0]);
            else
            {
                if ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value) != VariableTypes.Null)
                {
                    if ((elem.Attributes["isArray"] == null ? false : bool.Parse(elem.Attributes["isArray"].Value)))
                    {
                        switch ((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value))
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
                            case VariableTypes.Hashtable:
                                ret = Array.CreateInstance(typeof(Hashtable), elem.ChildNodes.Count);
                                break;
                        }
                        for (int x = 0; x < elem.ChildNodes.Count; x++)
                        {
                            string text = elem.ChildNodes[x].InnerText;
                            if (elem.ChildNodes[x].ChildNodes[0].NodeType == XmlNodeType.CDATA)
                                text = ((XmlCDataSection)elem.ChildNodes[x].ChildNodes[0]).InnerText;
                            ((Array)ret).SetValue(Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value), text), x);
                        }
                    }
                    else
                    {
                        string text = elem.InnerText;
                        if (elem.ChildNodes[0].NodeType == XmlNodeType.CDATA)
                            text = ((XmlCDataSection)elem.ChildNodes[0]).InnerText;
                        ret = Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes["type"].Value), text);
                    }
                }
                else
                    ret = null;
            }
            return ret;
        }
    }
}
