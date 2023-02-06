using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal static class Utility
    {

        private const int GUID_ID_LENGTH = 16;

        private static Dictionary<Type, List<Type>> _xmlChildren;
        private static Dictionary<Type, XMLTag[]> _tagAttributes;
        private static Type[] _globalXMLChildren;
        private static Dictionary<Type, ConstructorInfo> _xmlConstructors;
        private static Dictionary<string, Dictionary<string, Type>> _idealMap;
        public static Dictionary<string, Dictionary<string, Type>> IdealMap { get { return _idealMap; } }
        private static Thread _backgroundThread;
        private static List<object> _events;
        private static ManualResetEvent _backgroundMREEvent;

        private static MT19937 _rand;

        static Utility()
        {
            _rand = new MT19937(DateTime.Now.Ticks);
            _backgroundMREEvent = new ManualResetEvent(true);
            _events = new List<object>();
            _backgroundThread = new Thread(new ThreadStart(_BackgroundStart));
            _backgroundThread.Name = "Background Suspend/Delay Thread";
            _backgroundThread.IsBackground = true;
            _backgroundThread.Start();
            _xmlConstructors = new Dictionary<Type, ConstructorInfo>();
            _idealMap = new Dictionary<string, Dictionary<string, Type>>();
            List<Type> tmp = new List<Type>();
            _tagAttributes = new Dictionary<Type, XMLTag[]>();
            foreach (Type t in Assembly.GetAssembly(typeof(Utility)).GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IElement))))
            {
                XMLTag[] tags = (XMLTag[])t.GetCustomAttributes(typeof(XMLTag), false);
                if (tags.Length > 0)
                {
                    tmp.Add(t);
                    _tagAttributes.Add(t, tags);
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
            _xmlChildren = new Dictionary<Type, List<Type>>();
            List<Type> globalChildren = new List<Type>();
            for (int x = 0; x < tmp.Count; x++)
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
                        foreach (Type c in tmp.Where(c => c.IsSubclassOf(vpa.Parent)))
                        {
                            if (!_xmlChildren.ContainsKey(c))
                                _xmlChildren.Add(c, new List<Type>());
                            List<Type> types = _xmlChildren[c];
                            _xmlChildren.Remove(c);
                            types.Add(t);
                            _xmlChildren.Add(c, types);
                        }
                    }
                    else if (vpa.Parent.IsInterface)
                    {
                        foreach (Type c in tmp.Where(c => c.GetInterfaces().Contains(vpa.Parent)))
                        {
                            if (!_xmlChildren.ContainsKey(c))
                                _xmlChildren.Add(c, new List<Type>());
                            List<Type> types = _xmlChildren[c];
                            _xmlChildren.Remove(c);
                            types.Add(t);
                            _xmlChildren.Add(c, types);
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

        internal static XMLTag[] GetTagAttributes(Type t)
        {
            return (_tagAttributes.ContainsKey(t) ? _tagAttributes[t] : null);
        }

        public static void NextRandomBytes(byte[] buffer)
        {
            lock (_rand)
            {
                _rand.NextBytes(buffer);
            }
        }

        public static byte[] NextRandomBytes(int length)
        {
            byte[] ret = new byte[length];
            NextRandomBytes(ret);
            return ret;
        }

        public static Guid NextRandomGuid()
        {
            return new Guid(NextRandomBytes(GUID_ID_LENGTH));
        }

        private static Dictionary<string, Assembly> _cache = new Dictionary<string, Assembly>();
        private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();


        public static bool AllTypesAvailable(string[] assemblies, string[] types)
        {
            return !types.Any(str => GetType(assemblies, str)==null);
        }

        public static Type GetType(string[] assemblies, string type)
        {
            Assembly ass = null;
            Type ret = null;
            bool search = true;
            lock (_typeCache)
            {
                if (_typeCache.ContainsKey(type))
                {
                    search=false;
                    ret=_typeCache[type];
                }
            }
            if (search)
            {
                foreach (string assembly in assemblies)
                {
                    lock (_cache)
                    {
                        if (_cache.ContainsKey(assembly))
                            ass=_cache[assembly];
                        else
                        {
                            try
                            {
                                ass = Assembly.Load(assembly);
                            }
                            catch (Exception ex)
                            {
                                ass=null;
                            }
                            _cache.Add(assembly, ass);
                        }
                    }
                    if (ass!=null)
                        ret = ass.GetType(type, false);
                    if (ret!=null)
                        break;
                }
                lock (_typeCache)
                {
                    if (!_typeCache.ContainsKey(type))
                        _typeCache.Add(type, ret);
                }
            }
            return ret;
        }

        public static Type LocateElementType(Type parent, string tagName, XmlPrefixMap map)
        {
            DateTime start = DateTime.Now;
            Type ret = null;
            if (parent != null && _xmlChildren.ContainsKey(parent))
            {
                ret = _xmlChildren[parent]
                    .FirstOrDefault(t => _tagAttributes[t].Any(xt => xt.Matches(map, tagName)));
                if (ret!=null)
                    return ret;
            }
            ret = _globalXMLChildren
                .FirstOrDefault(t => _tagAttributes[t].Any(xt => xt.Matches(map, tagName)));
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

        internal static IElement ConstructElementType(XmlElement element, ref XmlPrefixMap map, ref ElementTypeCache cache, AElement parent)
        {
            Type t = null;
            if (cache != null)
            {
                if (cache.IsCached(element.Name))
                    t = cache[element.Name];
            }
            else
                t = Utility.LocateElementType((parent == null ? null : parent.GetType()), element.Name, map);
            if (t != null)
                return (IElement)_xmlConstructors[t].Invoke(new object[] { element, map, parent });
            return null;
        }

        public static string FindXPath(Definition definition, XmlNode node)
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
                            int index = FindElementIndex(definition, (XmlElement)node);
                            builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        }
                        else
                            builder.Insert(0, string.Format("/{0}[@id='{1}']", node.Name, node.Attributes["id"].Value));
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw (definition == null ? new ArgumentException("Only elements and attributes are supported") : definition.Exception(new ArgumentException("Only elements and attributes are supported")));
                }
            }
            throw (definition==null ? new ArgumentException("Node was not in a document") : definition.Exception(new ArgumentException("Node was not in a document")));
        }

        public static int FindElementIndex(Definition definition, XmlElement element)
        {
            if (definition!=null)
                definition.Debug("Locating Element Index for element {0}", new object[] { element.Name });
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
            throw (definition==null ? new ArgumentException("Couldn't find element within parent") : definition.Exception(new ArgumentException("Couldn't find element within parent")));
        }

        internal static object[] GetCustomAttributesForClass(Type clazz, Type attributeType)
        {
            List<object> ret = new List<object>(clazz.GetCustomAttributes(attributeType, false));
            Type parent = clazz.BaseType;
            if (parent != typeof(object))
            {
                foreach (object obj in GetCustomAttributesForClass(parent, attributeType))
                {
                    if (!ret.Contains(obj))
                        ret.Add(obj);
                }
            }
            return ret.ToArray();
        }

        internal static object ExtractVariableValue(VariableTypes type, string text)
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
                case VariableTypes.UnsignedInteger:
                    ret = uint.Parse(text);
                    break;
                case VariableTypes.UnsignedLong:
                    ret = ulong.Parse(text);
                    break;
                case VariableTypes.UnsignedShort:
                    ret = ushort.Parse(text);
                    break;
                case VariableTypes.String:
                    ret = text;
                    break;
                case VariableTypes.Guid:
                    ret = new Guid(text);
                    break;
            }
            return ret;
        }

        private static readonly TimeSpan _maxSpan = new TimeSpan(int.MaxValue);

        internal static void Sleep(TimeSpan value, ProcessInstance process, AEvent evnt)
        {
            lock (_events)
            {
                _events.Add(new sProcessSuspendEvent(process, evnt, value));
            }
            _backgroundMREEvent.Set();
        }

        internal static void DelayStart(TimeSpan value, ProcessInstance process, BoundaryEvent evnt, string sourceID)
        {
            lock (_events)
            {
                _events.Add(new sProcessDelayedEvent(process, evnt, value, sourceID));
            }
            _backgroundMREEvent.Set();
        }

        internal static void AbortDelayedEvent(ProcessInstance process,BoundaryEvent evnt,string sourceID)
        {
            lock (_events)
            {
                _events.RemoveAll(e =>
                    e is sProcessDelayedEvent &&
                    ((sProcessDelayedEvent)e).Instance.Equals(process) &&
                    ((sProcessDelayedEvent)e).Event.id==evnt.id &&
                    ((sProcessDelayedEvent)e).SourceID==sourceID);
            }
            _backgroundMREEvent.Set();
        }

        internal static void AbortSuspendedElement(ProcessInstance process, string id)
        {
            lock (_events)
            {
                _events.RemoveAll(e =>
                    e is sProcessSuspendEvent &&
                    ((sProcessSuspendEvent)e).Instance.Equals(process) &&
                    ((sProcessSuspendEvent)e).Event.id==id
                );
            }
            _backgroundMREEvent.Set();
        }

        internal static void UnloadProcess(BusinessProcess process)
        {
            bool changed = false;
            lock (_events)
            {
                changed = _events.RemoveAll(e =>
                    (e is sProcessDelayedEvent
                    && ((sProcessDelayedEvent)e).Instance.Process.Equals(process))
                    ||(e is sProcessSuspendEvent &&
                    ((sProcessSuspendEvent)e).Instance.Process.Equals(process))
                )>0;
            }
            if (changed)
                _backgroundMREEvent.Set();
        }

        internal static void UnloadProcess(ProcessInstance process)
        {
            bool changed = false;
            lock (_events) {
                changed = _events.RemoveAll(e =>
                    (e is sProcessDelayedEvent
                    && ((sProcessDelayedEvent)e).Instance.Equals(process))
                    ||(e is sProcessSuspendEvent &&
                    ((sProcessSuspendEvent)e).Instance.Equals(process))
                )>0;
            }
            if (changed)
                _backgroundMREEvent.Set();
        }

        internal static void _BackgroundStart()
        {
            while (true)
            {
                int sleep = -1;
                lock (_events)
                {
                    foreach (object obj in _events)
                    {
                        DateTime compare = DateTime.MaxValue;
                        if (obj is sProcessSuspendEvent)
                            compare = ((sProcessSuspendEvent)obj).EndTime;
                        else if (obj is sProcessDelayedEvent)
                            compare = ((sProcessDelayedEvent)obj).StartTime;
                        if (compare.Ticks < DateTime.Now.Ticks)
                        {
                            sleep = 0;
                            break;
                        }
                        else if (sleep == -1)
                            sleep = (int)Math.Min(compare.Subtract(DateTime.Now).TotalMilliseconds, (double)int.MaxValue);
                        else
                            sleep = Math.Min(sleep, (int)Math.Min(compare.Subtract(DateTime.Now).TotalMilliseconds, (double)int.MaxValue));
                    }
                }
                if (sleep != 0)
                {
                    if (sleep != -1)
                        _backgroundMREEvent.WaitOne(sleep);
                    else
                        _backgroundMREEvent.WaitOne();
                }
                _backgroundMREEvent.Reset();
                lock (_events)
                {
                    for (int x = 0; x < _events.Count; x++)
                    {
                        if (_events[x] is sProcessSuspendEvent)
                        {
                            sProcessSuspendEvent spe = (sProcessSuspendEvent)_events[x];
                            if (spe.EndTime.Ticks < DateTime.Now.Ticks)
                            {
                                try
                                {
                                    spe.Instance.CompleteTimedEvent(spe.Event);
                                    _events.RemoveAt(x);
                                    x--;
                                }
                                catch (Exception e) { spe.Instance.WriteLogException(spe.Event, new StackFrame(1, true), DateTime.Now, e); }
                            }
                        }else if (_events[x] is sProcessDelayedEvent)
                        {
                            sProcessDelayedEvent sde = (sProcessDelayedEvent)_events[x];
                            if (sde.StartTime.Ticks < DateTime.Now.Ticks)
                            {
                                try
                                {
                                    sde.Instance.StartTimedEvent(sde.Event, sde.SourceID);
                                    _events.RemoveAt(x);
                                    x--;
                                }
                                catch (Exception e) { sde.Instance.WriteLogException(sde.Event, new StackFrame(1, true), DateTime.Now, e); }
                            }
                        }
                    }
                }
            }
        }
    }
}
