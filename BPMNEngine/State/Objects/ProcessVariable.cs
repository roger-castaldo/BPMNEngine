using BPMNEngine.Interfaces.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BPMNEngine.State.ProcessVariables;

namespace BPMNEngine.State.Objects
{
    internal record ProcessVariable : IStateComponent
    {
        private static readonly Version ORIGINAL_VERSION = new("1.0");
        private const string _ORIGINAL_PATH_ENTRY_ELEMENT = "sVariableEntry";
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

        public string Name { get; private set; }
        public int StepIndex { get; private set; }  
        public object Value { get;private set; }

        public ProcessVariable() { }

        public ProcessVariable(string name, int stepIndex, object value)
        {
            Name=name;
            StepIndex=stepIndex;
            Value=value;
        }   

        public bool CanRead(XmlReader reader, Version version) => reader.NodeType==XmlNodeType.Element && (
                (reader.Name==_ORIGINAL_PATH_ENTRY_ELEMENT && version.Equals(ORIGINAL_VERSION))
                ||(reader.Name==GetType().Name && version>ORIGINAL_VERSION)
            );
        public bool CanRead(Utf8JsonReader reader, Version version)
            => reader.TokenType==JsonTokenType.StartObject;
        public XmlReader Read(XmlReader reader, Version version){
            StepIndex = int.Parse(reader.GetAttribute(_PATH_STEP_INDEX));
            Name = reader.GetAttribute(_NAME);
            var type = (VariableTypes)Enum.Parse(typeof(VariableTypes), reader.GetAttribute(_TYPE));
            var isArray = (reader.GetAttribute(_IS_ARRAY)!=null && bool.Parse(reader.GetAttribute(_IS_ARRAY)));
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
                            al.Add(DecodeFile(ref reader));
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
                        value = DecodeFile(ref reader);
                }
            }
            reader.Read();
            Value=value;
            if (reader.NodeType==XmlNodeType.EndElement && (
                (reader.Name==_ORIGINAL_PATH_ENTRY_ELEMENT && version.Equals(ORIGINAL_VERSION))
                ||(reader.Name==GetType().Name && version>ORIGINAL_VERSION)
            ))
                reader.Read();
            return reader;
        }
        public Utf8JsonReader Read(Utf8JsonReader reader, Version version) {
            var type = VariableTypes.Null;
            if (reader.TokenType==JsonTokenType.StartObject)
                reader.Read();
            while (reader.TokenType!=JsonTokenType.EndObject)
            {
                var prop = reader.GetString();
                reader.Read();
                switch (prop)
                {
                    case _PATH_STEP_INDEX:
                        StepIndex=reader.GetInt32(); break;
                    case _NAME:
                        Name=reader.GetString(); break;
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
                                    al.Add(DecodeFile(ref reader));
                                else
                                {
                                    al.Add(Utility.ExtractVariableValue(type, reader.GetString()));
                                    reader.Read();
                                }
                            }
                            Value = ConvertArray(al, type);
                        }
                        else if (reader.TokenType==JsonTokenType.StartObject && type == VariableTypes.File)
                            Value = DecodeFile(ref reader);
                        else
                            Value = Utility.ExtractVariableValue(type, reader.GetString());
                        break;
                }
                if (reader.TokenType!=JsonTokenType.EndObject)
                    reader.Read();
            }
            reader.Read();
            return reader;
        }
        public void Write(XmlWriter writer) {
            writer.WriteStartElement(GetType().Name);
            writer.WriteAttributeString(_PATH_STEP_INDEX, StepIndex.ToString());
            writer.WriteAttributeString(_NAME, Name);
            if (Value==null)
                writer.WriteAttributeString(_TYPE, VariableTypes.Null.ToString());
            else
            {
                writer.WriteAttributeString(_TYPE, GetVariableType(Value).ToString());
                if (Value.GetType().IsArray && Value.GetType().FullName != "System.Byte[]" && Value.GetType().FullName!="System.String")
                {
                    writer.WriteAttributeString(_IS_ARRAY, true.ToString());
                    if (Value.GetType().GetElementType().FullName == typeof(SFile).FullName)
                    {
                        ((IEnumerable)Value).Cast<SFile>().ForEach(sf =>
                        {
                            writer.WriteStartElement(_VALUE);
                            AppendFile(sf, writer);
                            writer.WriteEndElement();
                        });
                    }
                    else
                    {
                        ((IEnumerable)Value).Cast<object>().ForEach(val =>
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
                    if (Value is SFile file)
                        AppendFile(file, writer);
                    else
                        writer.WriteCData(EncodeValue(Value));
                }
            }
            writer.WriteEndElement();
        }
        public void Write(Utf8JsonWriter writer) {
            writer.WriteStartObject();
            writer.WritePropertyName(_PATH_STEP_INDEX);
            writer.WriteNumberValue(StepIndex);
            writer.WritePropertyName(_NAME);
            writer.WriteStringValue(Name);
            writer.WritePropertyName(_TYPE);
            if (Value==null)
                writer.WriteStringValue(VariableTypes.Null.ToString());
            else
            {
                writer.WriteStringValue(GetVariableType(Value).ToString());
                writer.WritePropertyName(_IS_ARRAY);
                if (Value.GetType().IsArray && Value.GetType().FullName != "System.Byte[]" && Value.GetType().FullName!="System.String")
                {
                    writer.WriteBooleanValue(true);
                    writer.WritePropertyName(_VALUE);
                    writer.WriteStartArray();
                    if (Value.GetType().GetElementType().FullName == typeof(SFile).FullName)
                    {
                        ((IEnumerable)Value).Cast<SFile>().ForEach(sf =>
                        {
                            AppendFile(sf, writer);
                        });
                    }
                    else
                    {
                        ((IEnumerable)Value).Cast<object>().ForEach(val =>
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
                    if (Value is SFile file)
                        AppendFile(file, writer);
                    else
                        writer.WriteStringValue(EncodeValue(Value));
                }
            }
            writer.WriteEndObject();
        }

        #region encoders
        private static void AppendFile(SFile file, XmlWriter writer)
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
        private static object ConvertArray(ArrayList al, VariableTypes type)
        {
            object result = null;
            switch (type)
            {
                case VariableTypes.Boolean:
                    result = Array.CreateInstance(typeof(bool), al.Count);
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
            for (var x = 0; x<al.Count; x++)
                ((Array)result).SetValue(al[x], x);
            return result;
        }
        private static SFile DecodeFile(ref XmlReader reader)
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
        private static SFile DecodeFile(ref Utf8JsonReader reader)
        {
            var name = string.Empty;
            var extension = string.Empty;
            string contentType = null;
            byte[] content = null;
            reader.Read();
            while (reader.TokenType!=JsonTokenType.EndObject)
            {
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
                reader.Read();
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

        private static VariableTypes GetVariableType(object value)
        {
            string fullName = (value.GetType().IsArray && value.GetType().FullName != "System.Byte[]" && value.GetType().FullName!="System.String" ? value.GetType().GetElementType().FullName : value.GetType().FullName);
            if (string.Equals(fullName,typeof(SFile).FullName))
                return VariableTypes.File;
            else
                return fullName switch
                {
                    "System.Boolean" => VariableTypes.Boolean,
                    "System.Byte[]" => VariableTypes.Byte,
                    "System.Char" => VariableTypes.Char,
                    "System.DateTime" => VariableTypes.DateTime,
                    "System.Decimal" => VariableTypes.Decimal,
                    "System.Double" => VariableTypes.Double,
                    "System.Single" => VariableTypes.Float,
                    "System.Int32" => VariableTypes.Integer,
                    "System.Int64" => VariableTypes.Long,
                    "System.Int16" => VariableTypes.Short,
                    "System.UInt32" => VariableTypes.UnsignedInteger,
                    "System.UInt64" => VariableTypes.UnsignedLong,
                    "System.UInt16" => VariableTypes.UnsignedShort,
                    "System.String" => VariableTypes.String,
                    "System.Guid" => VariableTypes.Guid,
                    _=>VariableTypes.Null
                };
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

        #endregion
    }
}
