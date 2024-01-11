using BPMNEngine.Attributes;
using BPMNEngine.Elements;
using BPMNEngine.Interfaces.Elements;
using System.Reflection;
using System.Text;

namespace BPMNEngine
{
    internal static class Utility
    {

        private static readonly Dictionary<Type, List<Type>> xmlChildren;
        private static readonly Dictionary<Type, XMLTag[]> tagAttributes;
        private static readonly Dictionary<Type, ConstructorInfo> xmlConstructors;
        private static readonly Dictionary<string, Dictionary<string, Type>> idealMap;
        public static Dictionary<string, Dictionary<string, Type>> IdealMap => idealMap;
        
        static Utility()
        {
            xmlConstructors = new();
            idealMap = new();
            tagAttributes = new();
            var tmp = new List<Type>();
            Assembly.GetAssembly(typeof(Utility)).GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IElement))).ForEach(t =>
            {
                XMLTag[] tags = (XMLTag[])t.GetCustomAttributes(typeof(XMLTag), false);
                if (tags.Length > 0)
                {
                    tmp.Add(t);
                    tagAttributes.Add(t, tags);
                    var tmpTypes = new Dictionary<string, Type>();
                    if (idealMap.ContainsKey(tags[0].Prefix.ToLower()))
                    {
                        tmpTypes = idealMap[tags[0].Prefix.ToLower()];
                        idealMap.Remove(tags[0].Prefix.ToLower());
                    }
                    xmlConstructors.Add(t, t.GetConstructor(new Type[] { typeof(XmlElement), typeof(XmlPrefixMap), typeof(AElement) }));
                    tmpTypes.Add(tags[0].Name.ToLower(), t);
                    idealMap.Add(tags[0].Prefix.ToLower(), tmpTypes);
                }
            });
            xmlChildren = new Dictionary<Type, List<Type>>();
            tmp.ForEach(t =>
            {
                var atts = new List<Attributes.ValidParentAttribute>();
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
                    if (parent != null)
                    {
                        if (parent.IsAbstract)
                        {
                            tmp.Where(c => c.IsSubclassOf(parent)).ForEach(c =>
                            {
                                if (!xmlChildren.ContainsKey(c))
                                    xmlChildren.Add(c, new List<Type>());
                                var types = xmlChildren[c];
                                xmlChildren.Remove(c);
                                types.Add(t);
                                xmlChildren.Add(c, types);
                            });
                        }
                        else
                        {
                            if (!xmlChildren.ContainsKey(parent))
                                xmlChildren.Add(parent, new List<Type>());
                            var types = xmlChildren[parent];
                            xmlChildren.Remove(parent);
                            types.Add(t);
                            xmlChildren.Add(parent, types);
                        }
                    }
                });
            });
        }

        internal static XMLTag[] GetTagAttributes(Type t)
            => tagAttributes.TryGetValue(t,out XMLTag[] value) ? value : null;

        internal static IElement ConstructElementType(XmlElement element, ref XmlPrefixMap map, ref ElementTypeCache cache, AElement parent)
            => cache.IsCached(element.Name) ? (IElement)xmlConstructors[cache[element.Name]].Invoke(new object[] { element, map, parent }) : null;

        public static string FindXPath(Definition definition, XmlNode node)
        {
            var builder = new StringBuilder();
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
            definition?.LogLine(LogLevel.Debug,null,"Locating Element Index for element {0}", new object[] { element.Name });
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
    }
}
