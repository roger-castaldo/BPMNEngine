using BPMNEngine.Elements;
using System.Globalization;
using System.Text;

namespace BPMNEngine
{
    internal static class Utility
    {
        public static string FindXPath(Definition? definition, XmlNode node)
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
                            builder.Insert(0, $"/{node.Name}[{index}]");
                        }
                        else
                            builder.Insert(0, $"/{node.Name}[@id='{node.Attributes["id"]?.Value}']");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in document");
        }

        public static int FindElementIndex(Definition definition, XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
                return 1;
            XmlElement parent = (XmlElement)parentNode;
            var result = parent.ChildNodes.Cast<XmlNode>().OfType<XmlElement>().IndexOf(e => e.Name == element.Name);
            if (result!=-1)
                return result;
            throw new ArgumentException("Couldn't find element within parent");
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
                VariableTypes.Decimal => decimal.Parse(text),
                VariableTypes.Double => double.Parse(text),
                VariableTypes.Float => float.Parse(text),
                VariableTypes.Integer => int.Parse(text),
                VariableTypes.Long => long.Parse(text),
                VariableTypes.Short => short.Parse(text),
                VariableTypes.UnsignedInteger => uint.Parse(text),
                VariableTypes.UnsignedLong => ulong.Parse(text),
                VariableTypes.UnsignedShort => ushort.Parse(text),
                VariableTypes.String => text,
                VariableTypes.Guid => new Guid(text),
                _ => null
            };
    }
}
