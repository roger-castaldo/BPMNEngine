using BPMNEngine.Attributes;
using BPMNEngine.Elements;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace BPMNEngine
{
    internal static class Utility
    {

        private const int GUID_ID_LENGTH = 16;

        private static Dictionary<Type, List<Type>> _xmlChildren;
        private static Dictionary<Type, XMLTag[]> _tagAttributes;
        private static readonly Type[] _globalXMLChildren;
        private static Dictionary<Type, ConstructorInfo> _xmlConstructors;
        private static Dictionary<string, Dictionary<string, Type>> _idealMap;
        public static Dictionary<string, Dictionary<string, Type>> IdealMap { get { return _idealMap; } }
        private static readonly Thread _backgroundThread;
        private static List<object> _events;
        private static ManualResetEvent _backgroundMREEvent;


        static Utility()
        {
            _backgroundMREEvent = new ManualResetEvent(true);
            _events = new List<object>();
            _backgroundThread = new Thread(new ThreadStart(BackgroundStart));
            _backgroundThread.Name = "Background Suspend/Delay Thread";
            _backgroundThread.IsBackground = true;
            _backgroundThread.Start();
            _xmlConstructors = new Dictionary<Type, ConstructorInfo>();
            _idealMap = new Dictionary<string, Dictionary<string, Type>>();
            List<Type> tmp = new List<Type>();
            _tagAttributes = new Dictionary<Type, XMLTag[]>();
            Assembly.GetAssembly(typeof(Utility)).GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IElement))).ForEach(t =>
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
            });
            _xmlChildren = new Dictionary<Type, List<Type>>();
            List<Type> globalChildren = new List<Type>();
            tmp.ForEach(t =>
            {
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
                atts.Select(vpa => vpa.Parent).ForEach(parent =>
                {
                    if (parent == null)
                        globalChildren.Add(t);
                    else if (parent.IsAbstract)
                    {
                        tmp.Where(c => c.IsSubclassOf(parent)).ForEach(c =>
                        {
                            if (!_xmlChildren.ContainsKey(c))
                                _xmlChildren.Add(c, new List<Type>());
                            List<Type> types = _xmlChildren[c];
                            _xmlChildren.Remove(c);
                            types.Add(t);
                            _xmlChildren.Add(c, types);
                        });
                    }else
                    {
                        if (!_xmlChildren.ContainsKey(parent))
                            _xmlChildren.Add(parent, new List<Type>());
                        List<Type> types = _xmlChildren[parent];
                        _xmlChildren.Remove(parent);
                        types.Add(t);
                        _xmlChildren.Add(parent, types);
                    }
                });
            });
            _globalXMLChildren = globalChildren.ToArray();
        }

        internal static XMLTag[] GetTagAttributes(Type t)
        {
            return (_tagAttributes.ContainsKey(t) ? _tagAttributes[t] : null);
        }

        public static byte[] NextRandomBytes(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }

        public static Guid NextRandomGuid()
        {
            return new Guid(NextRandomBytes(GUID_ID_LENGTH));
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
                        throw (definition == null ? new ArgumentException("Only elements and attributes are supported") : definition.Exception(null,new ArgumentException("Only elements and attributes are supported")));
                }
            }
            throw (definition==null ? new ArgumentException("Node was not in a document") : definition.Exception(null,new ArgumentException("Node was not in a document")));
        }

        public static int FindElementIndex(Definition definition, XmlElement element)
        {
            if (definition!=null)
                definition.Debug(null,"Locating Element Index for element {0}", new object[] { element.Name });
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
                return 1;
            XmlElement parent = (XmlElement)parentNode;
            var result = parent.ChildNodes.Cast<XmlNode>().OfType<XmlElement>().IndexOf(e=>e.Name == element.Name);
            if (result!=-1)
                return result;
            throw (definition==null ? new ArgumentException("Couldn't find element within parent") : definition.Exception(null,new ArgumentException("Couldn't find element within parent")));
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

        internal static void Sleep(TimeSpan value, ProcessInstance process, AEvent evnt)
        {
            lock (_events)
            {
                _events.Add(new SProcessSuspendEvent(process, evnt, value));
            }
            _backgroundMREEvent.Set();
        }

        internal static void DelayStart(TimeSpan value, ProcessInstance process, BoundaryEvent evnt, string sourceID)
        {
            lock (_events)
            {
                _events.Add(new SProcessDelayedEvent(process, evnt, value, sourceID));
            }
            _backgroundMREEvent.Set();
        }

        internal static void AbortDelayedEvent(ProcessInstance process,BoundaryEvent evnt,string sourceID)
        {
            lock (_events)
            {
                _events.RemoveAll(e =>
                    e is SProcessDelayedEvent &&
                    ((SProcessDelayedEvent)e).Instance.Equals(process) &&
                    ((SProcessDelayedEvent)e).Event.id==evnt.id &&
                    ((SProcessDelayedEvent)e).SourceID==sourceID);
            }
            _backgroundMREEvent.Set();
        }

        internal static void AbortSuspendedElement(ProcessInstance process, string id)
        {
            lock (_events)
            {
                _events.RemoveAll(e =>
                    e is SProcessSuspendEvent &&
                    ((SProcessSuspendEvent)e).Instance.Equals(process) &&
                    ((SProcessSuspendEvent)e).Event.id==id
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
                    (e is SProcessDelayedEvent
                    && ((SProcessDelayedEvent)e).Instance.Process.Equals(process))
                    ||(e is SProcessSuspendEvent &&
                    ((SProcessSuspendEvent)e).Instance.Process.Equals(process))
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
                    (e is SProcessDelayedEvent
                    && ((SProcessDelayedEvent)e).Instance.Equals(process))
                    ||(e is SProcessSuspendEvent &&
                    ((SProcessSuspendEvent)e).Instance.Equals(process))
                )>0;
            }
            if (changed)
                _backgroundMREEvent.Set();
        }

        internal static void BackgroundStart()
        {
            while (true)
            {
                int sleep = -1;
                lock (_events)
                {
                    if (_events.Any())
                    {
                        sleep = _events.Select(obj =>
                        {
                            DateTime compare = DateTime.MaxValue;
                            if (obj is SProcessSuspendEvent)
                                compare = ((SProcessSuspendEvent)obj).EndTime;
                            else if (obj is SProcessDelayedEvent)
                                compare = ((SProcessDelayedEvent)obj).StartTime;
                            if (compare.Ticks < DateTime.Now.Ticks)
                                return 0;
                            else
                                return (int)Math.Min(compare.Subtract(DateTime.Now).TotalMilliseconds, (double)int.MaxValue);
                        }).Min();
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
                    var now = DateTime.Now;
                    IEnumerable<object> toRemove = Array.Empty<object>();

                    _events.OfType<SProcessSuspendEvent>().ForEach(spse =>
                    {
                        if (spse.EndTime.Ticks<now.Ticks)
                        {
                            try
                            {
                                spse.Instance.CompleteTimedEvent(spse.Event);
                                toRemove = toRemove.Append(spse);
                            }
                            catch (Exception e) { spse.Instance.WriteLogException(spse.Event, new StackFrame(1, true), DateTime.Now, e); }
                        }
                    });

                    _events.OfType<SProcessDelayedEvent>().ForEach(spde =>
                    {
                        if (spde.StartTime.Ticks<now.Ticks)
                        {
                            try
                            {
                                spde.Instance.StartTimedEvent(spde.Event, spde.SourceID);
                                toRemove = toRemove.Append(spde);
                            }
                            catch (Exception e) { spde.Instance.WriteLogException(spde.Event, new StackFrame(1, true), DateTime.Now, e); }
                        }
                    });

                    _events.RemoveAll(o=>toRemove.Contains(o));
                }
            }
        }
    }
}
