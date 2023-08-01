using System.Collections;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading;
using BPMNEngine.Interfaces.State;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.State
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
            private readonly int stepIndex;
            private readonly ProcessVariables processVariables;

            public ReadOnlyProcessVariables(ProcessVariables processVariables,int stepIndex)
            {
                this.processVariables = processVariables;
                this.stepIndex=stepIndex;
            }

            public object this[string name] 
                => processVariables[name,stepIndex];

            public IEnumerable<string> Keys 
                => processVariables[stepIndex];

            public Dictionary<string, object> AsExtract { 
                get
                {
                    Dictionary<string, object> ret = new();
                    Keys.ForEach(key =>
                    {
                        ret.Add(key, this[key]);
                    });
                    return ret;
                }
            }

            public void Append(XmlWriter writer)
            {
                processVariables.stateLock.EnterReadLock();
                var variables = processVariables.variables.Where(variable => variable.StepIndex==stepIndex);
                processVariables.stateLock.ExitReadLock();
                _=variables.OrderBy(variable => variable.StepIndex).ForEach(variable =>
                {
                    writer.WriteStartElement(_VARIABLE_ENTRY_ELEMENT);
                    writer.WriteAttributeString(_PATH_STEP_INDEX, variable.StepIndex.ToString());
                    writer.WriteAttributeString(_NAME, variable.Name);
                    if (variable.Value==null)
                        writer.WriteAttributeString(_TYPE, VariableTypes.Null.ToString());
                    else
                    {
                        writer.WriteAttributeString(_TYPE, GetVariableType(variable.Value).ToString());
                        if (variable.Value.GetType().IsArray && variable.Value.GetType().FullName != "System.Byte[]" && variable.Value.GetType().FullName!="System.String")
                        {
                            writer.WriteAttributeString(_IS_ARRAY, true.ToString());
                            if (variable.Value.GetType().GetElementType().FullName == typeof(SFile).FullName)
                            {
                                ((IEnumerable)variable.Value).Cast<SFile>().ForEach(sf =>
                                {
                                    writer.WriteStartElement(_VALUE);
                                    ReadOnlyProcessVariables.AppendFile(sf, writer);
                                    writer.WriteEndElement();
                                });
                            }
                            else
                            {
                                ((IEnumerable)variable.Value).Cast<object>().ForEach(val =>
                                {
                                    writer.WriteStartElement(_VALUE);
                                    writer.WriteCData(EncodeValue(val));
                                    writer.WriteEndElement();
                                });
                            }
                        }
                        else
                        {
                            writer.WriteAttributeString(_IS_ARRAY, false.ToString());
                            if (variable.Value is SFile file)
                                ReadOnlyProcessVariables.AppendFile(file, writer);
                            else
                                writer.WriteCData(EncodeValue(variable.Value));
                        }
                    }
                    writer.WriteEndElement();
                });
            }

            public void Append(Utf8JsonWriter writer)
            {
                processVariables.stateLock.EnterReadLock();
                var variables = processVariables.variables.Where(variable => variable.StepIndex==stepIndex);
                processVariables.stateLock.ExitReadLock();
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
                        writer.WriteStringValue(GetVariableType(variable.Value).ToString());
                        writer.WritePropertyName(_IS_ARRAY);
                        if (variable.Value.GetType().IsArray && variable.Value.GetType().FullName != "System.Byte[]" && variable.Value.GetType().FullName!="System.String")
                        {
                            writer.WriteBooleanValue(true);
                            writer.WritePropertyName(_VALUE);
                            writer.WriteStartArray();
                            if (variable.Value.GetType().GetElementType().FullName == typeof(SFile).FullName)
                            {
                                ((IEnumerable)variable.Value).Cast<SFile>().ForEach(sf =>
                                {
                                    ReadOnlyProcessVariables.AppendFile(sf, writer);
                                });
                            }
                            else
                            {
                                ((IEnumerable)variable.Value).Cast<object>().ForEach(val =>
                                {
                                    writer.WriteStringValue(EncodeValue(val));
                                });
                            }
                            writer.WriteEndArray();
                        }
                        else
                        {
                            writer.WriteBooleanValue(false);
                            writer.WritePropertyName(_VALUE);
                            if (variable.Value is SFile file)
                                ReadOnlyProcessVariables.AppendFile(file, writer);
                            else
                                writer.WriteStringValue(EncodeValue(variable.Value));
                        }
                    }
                    writer.WriteEndObject();
                });
                writer.WriteEndArray();
            }

            private static void AppendFile(SFile file,XmlWriter writer)
            {
                writer.WriteStartElement(_FILE_ELEMENT_TYPE);
                writer.WriteAttributeString(_FILE_NAME, file.Name);
                writer.WriteAttributeString(_FILE_EXTENSION, file.Extension);
                if (!string.IsNullOrEmpty(file.ContentType))
                    writer.WriteAttributeString(_FILE_CONTENT_TYPE, file.ContentType);
                writer.WriteCData(Convert.ToBase64String(file.Content));
                writer.WriteEndElement();
            }

            private static void AppendFile(SFile file, Utf8JsonWriter writer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(_FILE_NAME);
                writer.WriteStringValue(file.Name);
                writer.WritePropertyName(_FILE_EXTENSION);
                writer.WriteStringValue(file.Extension);
                if (!string.IsNullOrEmpty(file.ContentType))
                {
                    writer.WritePropertyName(_FILE_CONTENT_TYPE);
                    writer.WriteStringValue(file.ContentType);
                }
                writer.WritePropertyName(_FILE_DATA);
                writer.WriteStringValue(Convert.ToBase64String(file.Content));
                writer.WriteEndObject();
            }
        }

        private readonly ReaderWriterLockSlim stateLock;

        private readonly List<SProcessVariable> variables;
        private readonly int? stepIndex;

        public ProcessVariables(ReaderWriterLockSlim stateLock,int? stepIndex=null)
        {
            this.stateLock=stateLock;
            variables=new List<SProcessVariable>();
            this.stepIndex=stepIndex;
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
            variables.Clear();
            reader.Read();
            while(reader.NodeType==XmlNodeType.Element && reader.Name==_VARIABLE_ENTRY_ELEMENT)
            {
                var stepIndex=int.Parse(reader.GetAttribute(_PATH_STEP_INDEX));
                var name=reader.GetAttribute(_NAME);
                var type = (VariableTypes)Enum.Parse(typeof(VariableTypes), reader.GetAttribute(_TYPE));
                var isArray = (reader.GetAttribute(_IS_ARRAY)!=null &&Boolean.Parse(reader.GetAttribute(_IS_ARRAY)));
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
                                al.Add(DecodeFile(reader));
                            else
                                al.Add(Utility.ExtractVariableValue(type, reader.Value));
                            reader.Read();
                            if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_VALUE)
                                reader.Read();
                        }
                        value = ConvertArray(al, type);
                    }
                    else
                    {
                        reader.Read();
                        if (reader.NodeType==XmlNodeType.CDATA)
                            value = Utility.ExtractVariableValue(type, reader.Value);
                        else
                            value = DecodeFile(reader);
                    }
                }
                reader.Read();
                variables.Add(new SProcessVariable()
                {
                    Name=name,
                    Value=value,
                    StepIndex=stepIndex
                });
                if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_VARIABLE_ENTRY_ELEMENT)
                    reader.Read();
            }
        }

        private static object ConvertArray(ArrayList al, VariableTypes type)
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
                    result = Array.CreateInstance(typeof(SFile), al.Count);
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

        private static SFile DecodeFile(XmlReader reader)
        {
            var name = reader.GetAttribute(_FILE_NAME);
            var extension = reader.GetAttribute(_FILE_EXTENSION);
            var contentType = reader.GetAttribute(_FILE_CONTENT_TYPE);
            reader.Read();
            var result = new SFile()
            {
                Name=name,
                Extension=extension,
                ContentType=contentType,
                Content=Convert.FromBase64String(reader.Value)
            };
            reader.Read();
            return result;
        }

        private static SFile DecodeFile(Utf8JsonReader reader)
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
            return new()
            {
                Name=name,
                Extension=extension,
                ContentType=contentType,
                Content=content
            };
        }

        public void Load(Utf8JsonReader reader)
        {
            variables.Clear();
            reader.Read();
            while (reader.TokenType !=JsonTokenType.EndArray)
            {
                var stepIndex = 0;
                var name = string.Empty;
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
                                        al.Add(DecodeFile(reader));
                                    else
                                        al.Add(Utility.ExtractVariableValue(type, reader.GetString()));
                                    reader.Read();
                                }
                                value = ConvertArray(al, type);
                            }
                            else
                                value = Utility.ExtractVariableValue(type, reader.GetString());
                            break;
                    }
                    reader.Read();
                }
                variables.Add(new SProcessVariable()
                {
                    Name=name,
                    Value=value,
                    StepIndex=stepIndex
                });
            }
            reader.Read();
        }

        public IReadOnlyStateContainer Clone()
            => new ReadOnlyProcessVariables(this, stepIndex??(variables.Any() ? variables.Max(var => var.StepIndex) : -1));

        public void Dispose()
            => variables.Clear();
        #endregion

        private IEnumerable<T> RunQuery<T>(Func<IEnumerable<SProcessVariable>, IEnumerable<T>> filter)
        {
            stateLock.EnterReadLock();
            var results = filter(variables).ToImmutableList();
            stateLock.ExitReadLock();
            return results;
        }

        public object this[string variableName,int stepIndex]
        {
            get {
                var lst = RunQuery((IEnumerable<SProcessVariable> variables) =>
                {
                    return variables
                    .OrderBy(var => var.StepIndex)
                    .Where(var => var.Name==variableName && var.StepIndex<=stepIndex);
                });
                return (lst.Any() ? lst.Last().Value : null);
            }
            set
            {
                stateLock.EnterWriteLock();
                variables.Add(new SProcessVariable()
                {
                    Name=variableName,
                    StepIndex=stepIndex,
                    Value=value
                });
                stateLock.ExitWriteLock();
            }
        }

        public IEnumerable<string> this[int stepIndex]
            => RunQuery((IEnumerable<SProcessVariable> variables) => { 
                return variables
                    .OrderBy(var => var.StepIndex)
                    .Where(variable => variable.StepIndex <= stepIndex)
                    .GroupBy(variable => variable.Name)
                    .Where(grp => grp.Last().Value != null)
                    .Select(grp => grp.Key);
            });

        internal void MergeVariables(int stepIndex, IVariables vars)
        {
            stateLock.EnterWriteLock();
            var changes = new Queue<SProcessVariable>();
            vars.Keys.ForEach(key =>
            {
                object left = vars[key];
                var right = (variables.Any(var => var.Name==key && var.StepIndex <= stepIndex) ?
                    (SProcessVariable?)variables
                        .OrderBy(var => var.StepIndex)
                        .LastOrDefault(var => var.Name==key
                            && var.StepIndex <= stepIndex)
                    : null);
                if (!IsVariablesEqual(left, (right?.Value)))
                    changes.Enqueue(new SProcessVariable()
                    {
                        Name=key,
                        StepIndex=stepIndex,
                        Value=left
                    });
            });
            while(changes.Any())
                variables.Add(changes.Dequeue());
            stateLock.ExitWriteLock();
        }

        private static VariableTypes GetVariableType(object value)
        {
            string fullName = (value.GetType().IsArray && value.GetType().FullName != "System.Byte[]" && value.GetType().FullName!="System.String" ? value.GetType().GetElementType().FullName : value.GetType().FullName);
            var result = VariableTypes.Null;
            if (fullName==typeof(SFile).FullName)
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

        private static string EncodeValue(object value)
        {
            string text = value.GetType().FullName switch
            {
                "System.Double" => ((double)value).ToString("R"),
                "System.Single" => ((float)value).ToString("R"),
                "System.Byte[]" => Convert.ToBase64String((byte[])value),
                _ => value.ToString(),
            };
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
                if (left is Array array)
                {
                    if (right is not Array array1)
                        return false;
                    else
                    {
                        Array aleft = array;
                        Array aright = array1;
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
    }
}
