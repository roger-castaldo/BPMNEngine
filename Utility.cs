using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal static class Utility
    {

        private static Type[] _xmlElements;
        private static Dictionary<Type, ConstructorInfo> _xmlConstructors;

        static Utility()
        {
            _xmlConstructors = new Dictionary<Type, ConstructorInfo>();
            List<Type> tmp = new List<Type>();
            foreach (Type t in Assembly.GetAssembly(typeof(Utility)).GetTypes())
            {
                if (new List<Type>(t.GetInterfaces()).Contains(typeof(IElement)))
                {
                    if (t.GetCustomAttributes(typeof(XMLTag), false).Length > 0)
                    {
                        tmp.Add(t);
                        _xmlConstructors.Add(t, t.GetConstructor(new Type[] { typeof(XmlElement), typeof(XmlPrefixMap), typeof(AElement) }));
                    }
                }
            }
            _xmlElements = tmp.ToArray();
        }

        public static Type LocateElementType(string tagName,XmlPrefixMap map)
        {
            Type ret = null;
            foreach (Type t in _xmlElements)
            {
                foreach (XMLTag xt in t.GetCustomAttributes(typeof(XMLTag), false))
                {
                    if (xt.Matches(map, tagName))
                    {
                        ret = t;
                        break;
                    }
                }
                if (ret != null)
                    break;
            }
            return ret;
        }

        //called to open a stream of a given embedded resource, again searches through all assemblies
        public static Stream LocateEmbededResource(string name)
        {
            Stream ret = typeof(Utility).Assembly.GetManifestResourceStream(name);
            if (ret == null)
            {
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (ass.GetName().Name != "mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name != "System" && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            ret = ass.GetManifestResourceStream(name);
                            if (ret != null)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                        {
                            throw e;
                        }
                    }
                }
            }
            return ret;
        }

        internal static IElement ConstructElementType(XmlElement element, XmlPrefixMap map,AElement parent)
        {
            Type t = null;
            if (BusinessProcess.ElementMapCache != null)
            {
                if (BusinessProcess.ElementMapCache.IsCached(element.Name))
                    t = BusinessProcess.ElementMapCache[element.Name];
                else
                {
                    t = Utility.LocateElementType(element.Name, map);
                    BusinessProcess.ElementMapCache[element.Name] = t;
                }
            }else
                t = Utility.LocateElementType(element.Name, map);
            if (t != null)
            {
                return (IElement)_xmlConstructors[t].Invoke(new object[] { element, map, parent });
            }
            return null;
        }

        public static string FindXPath(XmlNode node)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        if (node.Attributes["id"] == null)
                        {
                            int index = FindElementIndex((XmlElement)node);
                            builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        }
                        else
                            builder.Insert(0, string.Format("/{0}[@id='{1}']", node.Name, node.Attributes["id"].Value));
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw Log._Exception(new ArgumentException("Only elements and attributes are supported"));
                        break;
                }
            }
            throw Log._Exception(new ArgumentException("Node was not in a document"));
        }

        public static int FindElementIndex(XmlElement element)
        {
            Log.Debug("Locating Element Index for element {0}", new object[] { element.Name });
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                        return index;
                    index++;
                }
            }
            throw Log._Exception(new ArgumentException("Couldn't find element within parent"));
        }

        internal static object[] GetCustomAttributesForClass(Type clazz,Type attributeType)
        {
            List<object> ret = new List<object>(clazz.GetCustomAttributes(attributeType,false));
            Type parent = clazz.BaseType;
            if (parent != typeof(object))
            {
                foreach (object obj in GetCustomAttributesForClass(parent,attributeType))
                {
                    if (!ret.Contains(obj))
                        ret.Add(obj);
                }
            }
            return ret.ToArray();
        }

        internal static object ExtractVariableValue(VariableTypes type,string text)
        {
            if (type == VariableTypes.Null)
                return null;
            object ret = null;
            switch (type)
            {
                case VariableTypes.Boolean:
                    ret = bool.Parse(text);
                    break;
                case VariableTypes.Byte:
                    ret = Convert.FromBase64String(text);
                    break;
                case VariableTypes.Char:
                    ret = text[0];
                    break;
                case VariableTypes.DateTime:
                    ret = DateTime.Parse(text);
                    break;
                case VariableTypes.Decimal:
                    ret = decimal.Parse(text);
                    break;
                case VariableTypes.Double:
                    ret = double.Parse(text);
                    break;
                case VariableTypes.Float:
                    ret = float.Parse(text);
                    break;
                case VariableTypes.Integer:
                    ret = int.Parse(text);
                    break;
                case VariableTypes.Long:
                    ret = long.Parse(text);
                    break;
                case VariableTypes.Short:
                    ret = short.Parse(text);
                    break;
                case VariableTypes.String:
                    ret = text;
                    break;
                case VariableTypes.Hashtable:
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    ret = bf.Deserialize(new MemoryStream(Convert.FromBase64String(text)));
                    break;
            }
            return ret;
        }

        internal static void EncodeVariableValue(object value,XmlWriter writer)
        {
            if (value == null)
                writer.WriteAttributeString("type", VariableTypes.Null.ToString());
            else
            {
                if (value.GetType().IsArray && value.GetType().FullName!="System.Byte[]")
                {
                    writer.WriteAttributeString("isArray", "true");
                    if (value.GetType().GetElementType().FullName == typeof(sFile).FullName)
                    {
                        foreach (sFile sf in (IEnumerable)value)
                        {
                            writer.WriteStartElement("Value");
                            sf.Append(writer);
                            writer.WriteEndElement();
                        }
                    }else
                    {
                        switch (value.GetType().GetElementType().FullName)
                        {
                            case "System.Boolean":
                                writer.WriteAttributeString("type", VariableTypes.Boolean.ToString());
                                foreach (bool b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Byte[]":
                                writer.WriteAttributeString("type", VariableTypes.Byte.ToString());
                                foreach (byte[] b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    Convert.ToBase64String(b);
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Char":
                                writer.WriteAttributeString("type", VariableTypes.Char.ToString());
                                foreach (char b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.DateTime":
                                writer.WriteAttributeString("type", VariableTypes.DateTime.ToString());
                                foreach (DateTime b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Decimal":
                                writer.WriteAttributeString("type", VariableTypes.Decimal.ToString());
                                foreach (Decimal b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Double":
                                writer.WriteAttributeString("type", VariableTypes.Double.ToString());
                                foreach (double b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Single":
                                writer.WriteAttributeString("type", VariableTypes.Float.ToString());
                                foreach (Single b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Int32":
                                writer.WriteAttributeString("type", VariableTypes.Integer.ToString());
                                foreach (int b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Int64":
                                writer.WriteAttributeString("type", VariableTypes.Long.ToString());
                                foreach (long b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Int16":
                                writer.WriteAttributeString("type", VariableTypes.Short.ToString());
                                foreach (short b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.String":
                                writer.WriteAttributeString("type", VariableTypes.String.ToString());
                                foreach (string b in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(b.ToString());
                                    writer.WriteEndElement();
                                }
                                break;
                            case "System.Collections.Hashtable":
                                writer.WriteAttributeString("type", VariableTypes.Hashtable.ToString());
                                foreach (Hashtable ht in (IEnumerable)value)
                                {
                                    writer.WriteStartElement("Value");
                                    writer.WriteCData(_EncodeHashtable(ht));
                                    writer.WriteEndElement();
                                }
                                break;
                        }
                    }
                }
                else
                {
                    writer.WriteAttributeString("isArray", "false");
                    if (value is sFile)
                        ((sFile)value).Append(writer);
                    else
                    {
                        string text = value.ToString();
                        switch (value.GetType().FullName)
                        {
                            case "System.Boolean":
                                writer.WriteAttributeString("type", VariableTypes.Boolean.ToString());
                                break;
                            case "System.Byte[]":
                                writer.WriteAttributeString("type", VariableTypes.Byte.ToString());
                                text = Convert.ToBase64String((byte[])value);
                                break;
                            case "System.Char":
                                writer.WriteAttributeString("type", VariableTypes.Char.ToString());
                                break;
                            case "System.DateTime":
                                writer.WriteAttributeString("type", VariableTypes.DateTime.ToString());
                                break;
                            case "System.Decimal":
                                writer.WriteAttributeString("type", VariableTypes.Decimal.ToString());
                                break;
                            case "System.Double":
                                writer.WriteAttributeString("type", VariableTypes.Double.ToString());
                                break;
                            case "System.Single":
                                writer.WriteAttributeString("type", VariableTypes.Float.ToString());
                                break;
                            case "System.Int32":
                                writer.WriteAttributeString("type", VariableTypes.Integer.ToString());
                                break;
                            case "System.Int64":
                                writer.WriteAttributeString("type", VariableTypes.Long.ToString());
                                break;
                            case "System.Int16":
                                writer.WriteAttributeString("type", VariableTypes.Short.ToString());
                                break;
                            case "System.String":
                                writer.WriteAttributeString("type", VariableTypes.String.ToString());
                                break;
                            case "System.Collections.Hashtable":
                                writer.WriteAttributeString("type", VariableTypes.Hashtable.ToString());
                                text = _EncodeHashtable((Hashtable)value);
                                break;
                        }
                        writer.WriteCData(text);
                    }
                }
            }
        }

        private static string _EncodeHashtable(Hashtable hash)
        {
            MemoryStream ms = new MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            bf.Serialize(ms, hash);
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
