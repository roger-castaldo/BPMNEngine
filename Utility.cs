using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal static class Utility
    {

        private static Dictionary<Type, List<Type>> _xmlChildren;
        private static Type[] _globalXMLChildren;
        private static Dictionary<Type, ConstructorInfo> _xmlConstructors;
        private static Dictionary<string, Dictionary<string, Type>> _idealMap;
        public static Dictionary<string,Dictionary<string,Type>> IdealMap { get { return _idealMap; } }

        static Utility()
        {
            _xmlConstructors = new Dictionary<Type, ConstructorInfo>();
            _idealMap = new Dictionary<string, Dictionary<string, Type>>();
            List<Type> tmp = new List<Type>();
            foreach (Type t in Assembly.GetAssembly(typeof(Utility)).GetTypes())
            {
                if (new List<Type>(t.GetInterfaces()).Contains(typeof(IElement)))
                {
                    XMLTag[] tags = (XMLTag[])t.GetCustomAttributes(typeof(XMLTag), false);
                    if (tags.Length > 0)
                    {
                        tmp.Add(t);
                        Dictionary<string, Type> tmpTypes = new Dictionary<string, Type>();
                        if (_idealMap.ContainsKey(tags[0].Prefix.ToLower()))
                        {
                            tmpTypes = _idealMap[tags[0].Prefix.ToLower()];
                            _idealMap.Remove(tags[0].Prefix.ToLower());
                        }
                        _xmlConstructors.Add(t, t.GetConstructor(new Type[] { typeof(XmlElement), typeof(XmlPrefixMap), typeof(AElement) }));
                        tmpTypes.Add(tags[0].Name.ToLower(), t);
                        _idealMap.Add(tags[0].Prefix.ToLower(), tmpTypes);
                    }
                }
            }
            _xmlChildren = new Dictionary<Type, List<Type>>();
            List<Type> globalChildren = new List<Type>();
            for(int x = 0; x < tmp.Count; x++)
            {
                Type t = tmp[x];
                List<ValidParentAttribute> atts = new List<Attributes.ValidParentAttribute>();
                if (t.GetCustomAttributes(typeof(ValidParentAttribute), false).Length == 0)
                {
                    Type bt = t.BaseType;
                    while (bt != null)
                    {
                        if (bt.GetCustomAttributes(typeof(ValidParentAttribute), false).Length > 0)
                        {
                            atts.AddRange((ValidParentAttribute[])bt.GetCustomAttributes(typeof(ValidParentAttribute), false));
                            break;
                        }
                        else
                            bt = bt.BaseType;
                    }
                }
                else
                    atts.AddRange((ValidParentAttribute[])t.GetCustomAttributes(typeof(ValidParentAttribute), false));
                foreach (ValidParentAttribute vpa in atts)
                {
                    if (vpa.Parent == null)
                        globalChildren.Add(t);
                    else if (vpa.Parent.IsAbstract)
                    {
                        foreach (Type c in tmp)
                        {
                            if (c.IsSubclassOf(vpa.Parent))
                            {
                                if (!_xmlChildren.ContainsKey(c))
                                    _xmlChildren.Add(c, new List<Type>());
                                List<Type> types = _xmlChildren[c];
                                _xmlChildren.Remove(c);
                                types.Add(t);
                                _xmlChildren.Add(c, types);
                            }
                        }
                    }
                    else if (vpa.Parent.IsInterface)
                    {
                        foreach (Type c in tmp)
                        {
                            if (new List<Type>(c.GetInterfaces()).Contains(vpa.Parent))
                            {
                                if (!_xmlChildren.ContainsKey(c))
                                    _xmlChildren.Add(c, new List<Type>());
                                List<Type> types = _xmlChildren[c];
                                _xmlChildren.Remove(c);
                                types.Add(t);
                                _xmlChildren.Add(c, types);
                            }
                        }
                    }
                    else 
                    {
                        if (!_xmlChildren.ContainsKey(vpa.Parent))
                            _xmlChildren.Add(vpa.Parent, new List<Type>());
                        List<Type> types = _xmlChildren[vpa.Parent];
                        _xmlChildren.Remove(vpa.Parent);
                        types.Add(t);
                        _xmlChildren.Add(vpa.Parent, types);
                    }
                }
            }
            _globalXMLChildren = globalChildren.ToArray();
        }

        public static Type LocateElementType(Type parent,string tagName,XmlPrefixMap map)
        {
            DateTime start = DateTime.Now;
            Type ret = null;
            if (parent != null)
            {
                if (_xmlChildren.ContainsKey(parent))
                {
                    foreach (Type t in _xmlChildren[parent])
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
                }
            }
            foreach (Type t in _globalXMLChildren)
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
            Console.WriteLine("Time to translate [{0}] to [{1}] is {2}ms", new object[] { tagName, (ret == null ? null : ret.FullName), DateTime.Now.Subtract(start).TotalMilliseconds });
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
            }else
                t = Utility.LocateElementType((parent == null ? null : parent.GetType()), element.Name, map);
            if (t != null)
                return (IElement)_xmlConstructors[t].Invoke(new object[] { element, map, parent });
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

        internal static void EncodeVariableValue(object value,XmlElement variableContainer,XmlDocument doc)
        {
            variableContainer.Attributes.Append(doc.CreateAttribute("type"));
            if (value == null)
                variableContainer.Attributes["type"].Value = VariableTypes.Null.ToString();
            else
            {
                if (value.GetType().IsArray && value.GetType().FullName != "System.Byte[]" && value.GetType().FullName!="System.String")
                {
                    variableContainer.Attributes.Append(doc.CreateAttribute("isArray"));
                    variableContainer.Attributes["isArray"].Value = true.ToString();
                    if (value.GetType().GetElementType().FullName == typeof(sFile).FullName)
                    {
                        variableContainer.Attributes["type"].Value = VariableTypes.File.ToString();
                        foreach (sFile sf in (IEnumerable)value)
                        {
                            variableContainer.AppendChild(doc.CreateElement("Value"));
                            variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(sf.ToElement(doc));
                        }
                    }
                    else
                    {
                        switch (value.GetType().GetElementType().FullName)
                        {
                            case "System.Boolean":
                                variableContainer.Attributes["type"].Value = VariableTypes.Boolean.ToString();
                                foreach (bool b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Byte[]":
                                variableContainer.Attributes["type"].Value = VariableTypes.Byte.ToString();
                                foreach (byte[] b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(Convert.ToBase64String(b)));
                                }
                                break;
                            case "System.Char":
                                variableContainer.Attributes["type"].Value = VariableTypes.Char.ToString();
                                foreach (char b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.DateTime":
                                variableContainer.Attributes["type"].Value = VariableTypes.DateTime.ToString();
                                foreach (DateTime b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Decimal":
                                variableContainer.Attributes["type"].Value = VariableTypes.Decimal.ToString();
                                foreach (Decimal b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Double":
                                variableContainer.Attributes["type"].Value = VariableTypes.Double.ToString();
                                foreach (double b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Single":
                                variableContainer.Attributes["type"].Value = VariableTypes.Float.ToString();
                                foreach (Single b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Int32":
                                variableContainer.Attributes["type"].Value = VariableTypes.Integer.ToString();
                                foreach (int b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Int64":
                                variableContainer.Attributes["type"].Value = VariableTypes.Long.ToString();
                                foreach (long b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Int16":
                                variableContainer.Attributes["type"].Value = VariableTypes.Short.ToString();
                                foreach (short b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.String":
                                variableContainer.Attributes["type"].Value = VariableTypes.String.ToString();
                                foreach (string b in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(b.ToString()));
                                }
                                break;
                            case "System.Collections.Hashtable":
                                variableContainer.Attributes["type"].Value = VariableTypes.Hashtable.ToString();
                                foreach (Hashtable ht in (IEnumerable)value)
                                {
                                    variableContainer.AppendChild(doc.CreateElement("Value"));
                                    variableContainer.ChildNodes[variableContainer.ChildNodes.Count - 1].AppendChild(doc.CreateCDataSection(_EncodeHashtable(ht)));
                                }
                                break;
                        }
                    }
                }
                else
                {
                    variableContainer.Attributes.Append(doc.CreateAttribute("isArray"));
                    variableContainer.Attributes["isArray"].Value = false.ToString();
                    if (value is sFile)
                    {
                        variableContainer.Attributes["type"].Value = VariableTypes.File.ToString();
                        variableContainer.AppendChild(((sFile)value).ToElement(doc));
                    }
                    else
                    {
                        string text = value.ToString();
                        switch (value.GetType().FullName)
                        {
                            case "System.Boolean":
                                variableContainer.Attributes["type"].Value = VariableTypes.Boolean.ToString();
                                break;
                            case "System.Byte[]":
                                variableContainer.Attributes["type"].Value = VariableTypes.Byte.ToString();
                                text = Convert.ToBase64String((byte[])value);
                                break;
                            case "System.Char":
                                variableContainer.Attributes["type"].Value = VariableTypes.Char.ToString();
                                break;
                            case "System.DateTime":
                                variableContainer.Attributes["type"].Value = VariableTypes.DateTime.ToString();
                                break;
                            case "System.Decimal":
                                variableContainer.Attributes["type"].Value = VariableTypes.Decimal.ToString();
                                break;
                            case "System.Double":
                                variableContainer.Attributes["type"].Value = VariableTypes.Double.ToString();
                                break;
                            case "System.Single":
                                variableContainer.Attributes["type"].Value = VariableTypes.Float.ToString();
                                break;
                            case "System.Int32":
                                variableContainer.Attributes["type"].Value = VariableTypes.Integer.ToString();
                                break;
                            case "System.Int64":
                                variableContainer.Attributes["type"].Value = VariableTypes.Long.ToString();
                                break;
                            case "System.Int16":
                                variableContainer.Attributes["type"].Value = VariableTypes.Short.ToString();
                                break;
                            case "System.String":
                                variableContainer.Attributes["type"].Value = VariableTypes.String.ToString();
                                break;
                            case "System.Collections.Hashtable":
                                variableContainer.Attributes["type"].Value = VariableTypes.Hashtable.ToString();
                                text = _EncodeHashtable((Hashtable)value);
                                break;
                        }
                        variableContainer.AppendChild(doc.CreateCDataSection(text));
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

        private static readonly TimeSpan _maxSpan = new TimeSpan(int.MaxValue);

        internal static void Sleep(TimeSpan value)
        {
#if NET20
            while (value > _maxSpan)
            {
                Thread.Sleep(_maxSpan);
                value -= _maxSpan;
            }
            Thread.Sleep(value);
#else
            System.Threading.Tasks.Task tsk;
            while (value > _maxSpan)
            {
                tsk = System.Threading.Tasks.Task.Delay(_maxSpan);
                tsk.Wait();
                value -= _maxSpan;
            }
            tsk = System.Threading.Tasks.Task.Delay(value);
            tsk.Wait();
#endif

        }
    }
}
