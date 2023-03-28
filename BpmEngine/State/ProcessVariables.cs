using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.State
{
    internal class ProcessVariables : IStateContainer
    {
        private readonly struct SProcessVariable
        {
            public string Name { get; init; }
            public int StepIndex { get; init; }
            public object Value { get; init; }
        }

        internal class ReadOnlyProcessVariables : IReadOnlyStateVariablesContainer
        {
            private readonly int _stepIndex;
            private readonly ProcessVariables _processVariables;

            public ReadOnlyProcessVariables(ProcessVariables processVariables,int stepIndex)
            {
                _processVariables = processVariables;
                _stepIndex=stepIndex;
            }

            public object this[string name] => _processVariables[name,_stepIndex];

            public IEnumerable<string> Keys => _processVariables[_stepIndex];

            public void Append(XmlWriter writer)
            {
                _processVariables._stateLock.EnterReadLock();
                var variables = _processVariables._variables.Where(variable => variable.StepIndex==_stepIndex);
                _processVariables._stateLock.ExitReadLock();
                variables.OrderBy(variable => variable.StepIndex).ForEach(variable =>
                {
                    writer.WriteStartElement(_VARIABLE_ENTRY_ELEMENT);
                    writer.WriteAttributeString(_PATH_STEP_INDEX, variable.StepIndex.ToString());
                    writer.WriteAttributeString(_NAME, variable.Name);
                    if (variable.Value==null)
                        writer.WriteAttributeString(_TYPE, VariableTypes.Null.ToString());
                    else
                    {
                        writer.WriteAttributeString(_TYPE, _GetVariableType(variable.Value).ToString());
                        if (variable.Value.GetType().IsArray && variable.Value.GetType().FullName != "System.Byte[]" && variable.Value.GetType().FullName!="System.String")
                        {
                            writer.WriteAttributeString(_IS_ARRAY, true.ToString());
                            if (variable.Value.GetType().GetElementType().FullName == typeof(sFile).FullName)
                            {
                                ((IEnumerable)variable.Value).Cast<sFile>().ForEach(sf =>
                                {
                                    writer.WriteStartElement(_VALUE);
                                    _AppendFile(sf, writer);
                                    writer.WriteEndElement();
                                });
                            }
                            else
                            {
                                ((IEnumerable)variable.Value).Cast<object>().ForEach(val =>
                                {
                                    writer.WriteStartElement(_VALUE);
                                    writer.WriteCData(_EncodeValue(val));
                                    writer.WriteEndElement();
                                });
                            }
                        }
                        else
                        {
                            writer.WriteAttributeString(_IS_ARRAY, false.ToString());
                            if (variable.Value is sFile)
                                _AppendFile((sFile)variable.Value, writer);
                            else
                                writer.WriteCData(_EncodeValue(variable.Value));
                        }
                    }
                    writer.WriteEndElement();
                });
            }

            public void Append(Utf8JsonWriter writer)
            {
                _processVariables._stateLock.EnterReadLock();
                var variables = _processVariables._variables.Where(variable => variable.StepIndex==_stepIndex);
                _processVariables._stateLock.ExitReadLock();
                writer.WriteStartArray();
                variables.OrderBy(variable => variable.StepIndex).ForEach(variable =>
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(_PATH_STEP_INDEX);
                    writer.WriteNumberValue(variable.StepIndex);
                    writer.WritePropertyName(_NAME);
                    writer.WriteStringValue(variable.Name);
                    writer.WritePropertyName(_TYPE);
                    if (variable.Value==null)
                        writer.WriteStringValue(VariableTypes.Null.ToString());
                    else
                    {
                        writer.WriteStringValue(_GetVariableType(variable.Value).ToString());
                        writer.WritePropertyName(_IS_ARRAY);
                        if (variable.Value.GetType().IsArray && variable.Value.GetType().FullName != "System.Byte[]" && variable.Value.GetType().FullName!="System.String")
                        {
                            writer.WriteBooleanValue(true);
                            writer.WritePropertyName(_VALUE);
                            writer.WriteStartArray();
                            if (variable.Value.GetType().GetElementType().FullName == typeof(sFile).FullName)
                            {
                                ((IEnumerable)variable.Value).Cast<sFile>().ForEach(sf =>
                                {
                                    _AppendFile(sf, writer);
                                });
                            }
                            else
                            {
                                ((IEnumerable)variable.Value).Cast<object>().ForEach(val =>
                                {
                                    writer.WriteStringValue(_EncodeValue(val));
                                });
                            }
                            writer.WriteEndArray();
                        }
                        else
                        {
                            writer.WriteBooleanValue(false);
                            writer.WritePropertyName(_VALUE);
                            if (variable.Value is sFile)
                                _AppendFile((sFile)variable.Value, writer);
                            else
                                writer.WriteStringValue(_EncodeValue(variable.Value));
                        }
                    }
                    writer.WriteEndObject();
                });
                writer.WriteEndArray();
            }

            private void _AppendFile(sFile file,XmlWriter writer)
            {
                writer.WriteStartElement(_FILE_ELEMENT_TYPE);
                writer.WriteAttributeString(_FILE_NAME, file.Name);
                writer.WriteAttributeString(_FILE_EXTENSION, file.Extension);
                if (file.ContentType!=null)
                    writer.WriteAttributeString(_FILE_CONTENT_TYPE, file.ContentType);
                writer.WriteCData(Convert.ToBase64String(file.Content));
                writer.WriteEndElement();
            }

            private void _AppendFile(sFile file, Utf8JsonWriter writer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(_FILE_NAME);
                writer.WriteStringValue(file.Name);
                writer.WritePropertyName(_FILE_EXTENSION);
                writer.WriteStringValue(file.Extension);
                if (file.ContentType!=null)
                {
                    writer.WritePropertyName(_FILE_CONTENT_TYPE);
                    writer.WriteStringValue(file.ContentType);
                }
                writer.WritePropertyName(_FILE_DATA);
                writer.WriteStringValue(Convert.ToBase64String(file.Content));
                writer.WriteEndObject();
            }
        }

        private readonly ReaderWriterLockSlim _stateLock;

        private readonly List<SProcessVariable> _variables;

        public ProcessVariables(ReaderWriterLockSlim stateLock)
        {
            _stateLock=stateLock;
            _variables=new List<SProcessVariable>();
        }

        #region IStateContainer
        private const string _VARIABLE_ENTRY_ELEMENT = "sVariableEntry";
        private const string _PATH_STEP_INDEX = "pathStepIndex";
        private const string _NAME = "name";
        private const string _IS_ARRAY = "isArray";
        private const string _TYPE = "type";
        private const string _VALUE = "Value";
        private const string _FILE_ELEMENT_TYPE = "sFile";
        private const string _FILE_NAME = "Name";
        private const string _FILE_EXTENSION = "Extension";
        private const string _FILE_CONTENT_TYPE = "ContentType";
        private const string _FILE_DATA = "Data";
        
        public void Load(XmlReader reader)
        {
            _variables.Clear();
            reader.Read();
            while(reader.NodeType==XmlNodeType.Element && reader.Name==_VARIABLE_ENTRY_ELEMENT)
            {
                var stepIndex=int.Parse(reader.GetAttribute(_PATH_STEP_INDEX));
                var name=reader.GetAttribute(_NAME);
                var type = (VariableTypes)Enum.Parse(typeof(VariableTypes), reader.GetAttribute(_TYPE));
                var isArray = (reader.GetAttribute(_IS_ARRAY)==null ? false : Boolean.Parse(reader.GetAttribute(_IS_ARRAY)));
                object value = null;
                if (type!=VariableTypes.Null)
                {
                    if (isArray)
                    {
                        var al = new ArrayList();
                        reader.Read();
                        while (reader.NodeType==XmlNodeType.Element && reader.Name==_VALUE)
                        {
                            reader.Read();
                            if (type==VariableTypes.File)
                                al.Add(_DecodeFile(reader));
                            else
                                al.Add(Utility.ExtractVariableValue(type, reader.Value));
                            reader.Read();
                            if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_VALUE)
                                reader.Read();
                        }
                        value = _ConvertArray(al, type);
                    }
                    else
                    {
                        reader.Read();
                        if (reader.NodeType==XmlNodeType.CDATA)
                            value = Utility.ExtractVariableValue(type, reader.Value);
                        else
                            value = _DecodeFile(reader);
                    }
                }
                reader.Read();
                _variables.Add(new SProcessVariable()
                {
                    Name=name,
                    Value=value,
                    StepIndex=stepIndex
                });
                if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_VARIABLE_ENTRY_ELEMENT)
                    reader.Read();
            }
        }

        private static object _ConvertArray(ArrayList al, VariableTypes type)
        {
            object result = null;
            switch (type)
            {
                case VariableTypes.Boolean:
                    result = Array.CreateInstance(typeof(bool),al.Count);
                    break;
                case VariableTypes.Byte:
                    result = Array.CreateInstance(typeof(byte[]), al.Count);
                    break;
                case VariableTypes.Char:
                    result = Array.CreateInstance(typeof(char), al.Count);
                    break;
                case VariableTypes.DateTime:
                    result = Array.CreateInstance(typeof(DateTime), al.Count);
                    break;
                case VariableTypes.Decimal:
                    result = Array.CreateInstance(typeof(decimal), al.Count);
                    break;
                case VariableTypes.Double:
                    result = Array.CreateInstance(typeof(double), al.Count);
                    break;
                case VariableTypes.File:
                    result = Array.CreateInstance(typeof(sFile), al.Count);
                    break;
                case VariableTypes.Float:
                    result = Array.CreateInstance(typeof(float), al.Count);
                    break;
                case VariableTypes.Integer:
                    result = Array.CreateInstance(typeof(int), al.Count);
                    break;
                case VariableTypes.Long:
                    result = Array.CreateInstance(typeof(long), al.Count);
                    break;
                case VariableTypes.Short:
                    result = Array.CreateInstance(typeof(short), al.Count);
                    break;
                case VariableTypes.String:
                    result = Array.CreateInstance(typeof(string), al.Count);
                    break;
                case VariableTypes.Guid:
                    result = Array.CreateInstance(typeof(Guid), al.Count);
                    break;
                case VariableTypes.UnsignedShort:
                    result = Array.CreateInstance(typeof(ushort), al.Count);
                    break;
                case VariableTypes.UnsignedInteger:
                    result = Array.CreateInstance(typeof(uint), al.Count);
                    break;
                case VariableTypes.UnsignedLong:
                    result = Array.CreateInstance(typeof(ulong), al.Count);
                    break;
            }
            for(var x = 0; x<al.Count; x++)
                ((Array)result).SetValue(al[x], x);
            return result;
        }

        private static sFile _DecodeFile(XmlReader reader)
        {
            var name = reader.GetAttribute(_FILE_NAME);
            var extension = reader.GetAttribute(_FILE_EXTENSION);
            var contentType = reader.GetAttribute(_FILE_CONTENT_TYPE);
            reader.Read();
            var result = new sFile(name, extension, contentType, Convert.FromBase64String(reader.Value));
            reader.Read();
            return result;
        }

        private static sFile _DecodeFile(Utf8JsonReader reader)
        {
            var name = string.Empty;
            var extension = string.Empty;
            string contentType = null;
            byte[] content = null;
            while (reader.TokenType!=JsonTokenType.EndObject)
            {
                reader.Read();
                string prop = reader.GetString();
                reader.Read();
                switch (prop)
                {
                    case _FILE_NAME:
                        name=reader.GetString();
                        break;
                    case _FILE_EXTENSION:
                        extension=reader.GetString();
                        break;
                    case _FILE_CONTENT_TYPE:
                        contentType=reader.GetString();
                        break;
                    case _FILE_DATA:
                        content = Convert.FromBase64String(reader.GetString());
                        break;
                }
            }
            reader.Read();
            return new sFile(name,extension,contentType, content);
        }

        private static sFile _DecodeFile(XmlElement element)
        {
            return new sFile(
                element.GetAttribute(_FILE_NAME),
                element.GetAttribute(_FILE_EXTENSION),
                element.GetAttribute(_FILE_CONTENT_TYPE),
                Convert.FromBase64String(((XmlCDataSection)element.ChildNodes[0]).Value)
            );
        }

        public void Load(Utf8JsonReader reader)
        {
            _variables.Clear();
            reader.Read();
            while (reader.TokenType !=JsonTokenType.EndArray)
            {
                var stepIndex = 0;
                var name = string.Empty;
                var isArray = false;
                var type = VariableTypes.Null;
                object value = null;
                if (reader.TokenType==JsonTokenType.StartObject)
                    reader.Read();
                while (reader.TokenType!=JsonTokenType.EndObject)
                {
                    var prop = reader.GetString();
                    reader.Read();
                    switch (prop)
                    {
                        case _PATH_STEP_INDEX:
                            stepIndex=reader.GetInt32(); break;
                        case _NAME:
                            name=reader.GetString(); break;
                        case _IS_ARRAY:
                            isArray = reader.GetBoolean();
                            break;
                        case _TYPE:
                            type = (VariableTypes)Enum.Parse(typeof(VariableTypes), reader.GetString());
                            break;
                        case _VALUE:
                            if (reader.TokenType==JsonTokenType.StartArray)
                            {
                                reader.Read();
                                var al = new ArrayList();
                                while (reader.TokenType!=JsonTokenType.EndArray)
                                {
                                    if (type==VariableTypes.File)
                                        al.Add(_DecodeFile(reader));
                                    else
                                        al.Add(Utility.ExtractVariableValue(type, reader.GetString()));
                                    reader.Read();
                                }
                                value = _ConvertArray(al, type);
                            }
                            else
                                value = Utility.ExtractVariableValue(type, reader.GetString());
                            break;
                    }
                    reader.Read();
                }
                _variables.Add(new SProcessVariable()
                {
                    Name=name,
                    Value=value,
                    StepIndex=stepIndex
                });
            }
            reader.Read();
        }

        public IReadOnlyStateContainer Clone()
        {
            return new ReadOnlyProcessVariables(this, (_variables.Any() ? _variables.Max(var => var.StepIndex) : -1));
        }

        public void Dispose()
        {
            _variables.Clear();
        }
        #endregion

        private IEnumerable<T> RunQuery<T>(Func<IEnumerable<SProcessVariable>, IEnumerable<T>> filter)
        {
            _stateLock.EnterReadLock();
            var results = filter(_variables).ToImmutableList();
            _stateLock.ExitReadLock();
            return results;
        }

        public object this[string variableName,int stepIndex]
        {
            get {
                var lst = RunQuery<SProcessVariable>((IEnumerable<SProcessVariable> variables) =>
                {
                    return variables
                    .OrderBy(var => var.StepIndex)
                    .Where(var => var.Name==variableName && var.StepIndex<=stepIndex);
                });
                return (lst.Any() ? lst.Last().Value : null);
            }
            set
            {
                _stateLock.EnterWriteLock();
                _variables.Add(new SProcessVariable()
                {
                    Name=variableName,
                    StepIndex=stepIndex,
                    Value=value
                });
                _stateLock.ExitWriteLock();
            }
        }

        public IEnumerable<string> this[int stepIndex]
            => RunQuery<string>((IEnumerable<SProcessVariable> variables) => { 
                return variables
                    .OrderBy(var => var.StepIndex)
                    .Where(variable => variable.StepIndex <= stepIndex)
                    .GroupBy(variable => variable.Name)
                    .Where(grp => grp.Last().Value != null)
                    .Select(grp => grp.Key);
            });

        internal void MergeVariables(int stepIndex, IVariables vars)
        {
            _stateLock.EnterWriteLock();
            var changes = new Queue<SProcessVariable>();
            vars.Keys.ForEach(key =>
            {
                object left = vars[key];
                var right = (_variables.Any(var => var.Name==key && var.StepIndex <= stepIndex) ?
                    (SProcessVariable?)_variables
                        .OrderBy(var => var.StepIndex)
                        .LastOrDefault(var => var.Name==key
                            && var.StepIndex <= stepIndex)
                    : null);
                if (!IsVariablesEqual(left, (right==null ? null : right.Value.Value)))
                    changes.Enqueue(new SProcessVariable()
                    {
                        Name=key,
                        StepIndex=stepIndex,
                        Value=left
                    });
            });
            while(changes.Count()>0)
                _variables.Add(changes.Dequeue());
            _stateLock.ExitWriteLock();
        }

        private static VariableTypes _GetVariableType(object value)
        {
            string fullName = (value.GetType().IsArray && value.GetType().FullName != "System.Byte[]" && value.GetType().FullName!="System.String" ? value.GetType().GetElementType().FullName : value.GetType().FullName);
            var result = VariableTypes.Null;
            if (fullName==typeof(sFile).FullName)
                result= VariableTypes.File;
            else
            {
                switch (fullName)
                {
                    case "System.Boolean":
                        result = VariableTypes.Boolean;
                        break;
                    case "System.Byte[]":
                        result = VariableTypes.Byte;
                        break;
                    case "System.Char":
                        result = VariableTypes.Char;
                        break;
                    case "System.DateTime":
                        result = VariableTypes.DateTime;
                        break;
                    case "System.Decimal":
                        result = VariableTypes.Decimal;
                        break;
                    case "System.Double":
                        result = VariableTypes.Double;
                        break;
                    case "System.Single":
                        result = VariableTypes.Float;
                        break;
                    case "System.Int32":
                        result = VariableTypes.Integer;
                        break;
                    case "System.Int64":
                        result = VariableTypes.Long;
                        break;
                    case "System.Int16":
                        result = VariableTypes.Short;
                        break;
                    case "System.UInt32":
                        result = VariableTypes.UnsignedInteger;
                        break;
                    case "System.UInt64":
                        result = VariableTypes.UnsignedLong;
                        break;
                    case "System.UInt16":
                        result = VariableTypes.UnsignedShort;
                        break;
                    case "System.String":
                        result = VariableTypes.String;
                        break;
                    case "System.Guid":
                        result = VariableTypes.Guid;
                        break;
                }
            }
            return result;
        }

        private static string _EncodeValue(object value)
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

        internal static bool IsVariablesEqual(object left, object right)
        {
            if (left == null && right != null)
                return false;
            else if (left != null && right == null)
                return false;
            else if (left == null && right == null)
                return true;
            else
            {
                if (left is Array)
                {
                    if (!(right is Array))
                        return false;
                    else
                    {
                        Array aleft = (Array)left;
                        Array aright = (Array)right;
                        if (aleft.Length != aright.Length)
                            return false;
                        return aleft.Cast<object>().Select((v, i) => new { val = v, index = i }).All(ival => IsVariablesEqual(ival.val, aright.GetValue(ival.index)));
                    }
                }
                else
                {
                    try { return left.Equals(right); }
                    catch (Exception) { return false; }
                }
            }
        }
        private static object ConvertValue(XmlElement? elem)
        {
            if (elem==null)
                return null;
            object ret = null;
            var type = (VariableTypes)Enum.Parse(typeof(VariableTypes), elem.Attributes[_TYPE].Value);
            if (type != VariableTypes.Null)
            {
                if ((elem.Attributes[_IS_ARRAY] !=null)&&bool.Parse(elem.Attributes[_IS_ARRAY].Value))
                {
                    var al = new ArrayList();
                    elem.ChildNodes.Cast<XmlNode>().ForEach(node =>
                    {
                        if (type==VariableTypes.File)
                            al.Add(_DecodeFile((XmlElement)node));
                        else
                        {
                            string text = ((XmlCDataSection)node.ChildNodes[0]).InnerText;
                            al.Add(Utility.ExtractVariableValue(type, text));
                        }
                    });
                    ret=_ConvertArray(al, type);
                }
                else if (type == VariableTypes.File)
                    ret = _DecodeFile((XmlElement)elem.ChildNodes[0]);
                else
                {
                    string text = ((XmlCDataSection)elem.ChildNodes[0]).InnerText;
                    ret = Utility.ExtractVariableValue(type, text);
                }
            }
            else
                ret = null;
            return ret;
        }

        public static Dictionary<string, object> ExtractVariables(IState currentState)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            currentState.Keys.ForEach(key =>
            {
                ret.Add(key, currentState[key]);
            });
            return ret;
        }

        public static Dictionary<string, object> ExtractVariables(XmlDocument doc)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            var grps = doc.GetElementsByTagName(typeof(ProcessVariables).Name).Cast<XmlNode>()
                .SelectMany(node => node.ChildNodes.Cast<XmlNode>())
                .Where(cnode => cnode.NodeType==XmlNodeType.Element)
                .GroupBy(node => node.Attributes[_NAME].Value)
                .Where(grp=>grp.Last().Attributes[_TYPE].Value!=VariableTypes.Null.ToString());
            grps.ForEach(grp => ret.Add(grp.Key, ConvertValue((XmlElement)grp.Last())));
            return ret;
        }
    }
}
