using BPMNEngine.Attributes;
using BPMNEngine.Elements;
using BPMNEngine.Interfaces.Elements;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

namespace BPMNEngine
{
    internal static class Utility
    {

        private static readonly Dictionary<Type, List<Type>> xmlChildren;
        private static readonly Dictionary<Type, XMLTagAttribute[]> tagAttributes;
        private static readonly Dictionary<Type, ConstructorInfo> xmlConstructors;
        private static readonly Dictionary<string, Dictionary<string, Type>> idealMap;
        public static Dictionary<string, Dictionary<string, Type>> IdealMap => idealMap;
        
        static Utility()
        {
            xmlConstructors = [];
            idealMap = [];
            tagAttributes = [];
            var tmp =
            Assembly.GetAssembly(typeof(Utility))
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IElement)))
                .Select(t => new { Type = t, Tags = t.GetCustomAttributes<XMLTagAttribute>(false).ToArray() })
                .Where(tt => tt.Tags.Length>0)
                .Select(tt =>
                {
                    tagAttributes.Add(tt.Type, tt.Tags);
                    var tmpTypes = new Dictionary<string, Type>();
                    if (idealMap.ContainsKey(tt.Tags[0].Prefix.ToLower()))
                    {
                        tmpTypes = idealMap[tt.Tags[0].Prefix.ToLower()];
                        idealMap.Remove(tt.Tags[0].Prefix.ToLower());
                    }
                    xmlConstructors.Add(tt.Type, tt.Type.GetConstructor([typeof(XmlElement), typeof(XmlPrefixMap), typeof(AElement)]));
                    tmpTypes.Add(tt.Tags[0].Name.ToLower(), tt.Type);
                    idealMap.Add(tt.Tags[0].Prefix.ToLower(), tmpTypes);
                    return tt.Type;
                })
                .ToArray();
            xmlChildren = [];
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

        internal static XMLTagAttribute[] GetTagAttributes(Type t)
            => tagAttributes.TryGetValue(t,out XMLTagAttribute[] value) ? value : null;

        internal static IElement ConstructElementType(XmlElement element, ref XmlPrefixMap map, ref ElementTypeCache cache, AElement parent)
            => cache.IsCached(element.Name) ? (IElement)xmlConstructors[cache[element.Name]].Invoke([element, map, parent]) : null;

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
            definition?.LogLine(LogLevel.Debug,null,$"Locating Element Index for element {element.Name}");
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
            => (type) switch
            {
                VariableTypes.Null => null,
                VariableTypes.Boolean => bool.Parse(text),
                VariableTypes.Byte => Convert.FromBase64String(text),
                VariableTypes.Char => text[0],
                VariableTypes.DateTime => (
                    !DateTime.TryParse(text, CultureInfo.InvariantCulture, out DateTime dt)
                    && !DateTime.TryParseExact(text, Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                    ? throw new FormatException($"Unable to parse date {text}")
                    : dt
                ),
                VariableTypes.Decimal=>decimal.Parse(text),
                VariableTypes.Double=>double.Parse(text),
                VariableTypes.Float=>float.Parse(text),
                VariableTypes.Integer=>int.Parse(text),
                VariableTypes.Long=>long.Parse(text),
                VariableTypes.Short=>short.Parse(text),
                VariableTypes.UnsignedInteger=>uint.Parse(text),
                VariableTypes.UnsignedLong=>ulong.Parse(text),
                VariableTypes.UnsignedShort=>ushort.Parse(text),
                VariableTypes.String=>text,
                VariableTypes.Guid=>new Guid(text),
                _ => null
            };
    }
}
