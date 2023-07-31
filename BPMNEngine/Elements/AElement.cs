using BPMNEngine.Elements.Processes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements
{
    internal abstract class AElement : IElement
    {
        public AElement Parent { get; private init; }

        public Definition Definition
            => (this is Definition definition) ? definition : Parent?.Definition;

        internal XmlElement Element { get; private init; }

        private string _cachedID=null;

        public string ID
        {
            get {
                var ret = _cachedID??this["id"];
                if (ret==null)
                    _cachedID = Utility.FindXPath(Definition, Element);
                return ret??_cachedID;
            }
        }

        public IEnumerable<XmlNode> SubNodes => Element.ChildNodes.Cast<XmlNode>();

        public string this[string attributeName] => Element.Attributes.Cast<XmlAttribute>()
                    .Where(att => att.Name.ToLower()==attributeName.ToLower())
                    .Select(att => att.Value)
                    .FirstOrDefault()??null;
        public IElement ExtensionElement { get; private init; }

        public AElement(XmlElement elem,XmlPrefixMap map,AElement parent)
        {
            Element = elem;
            ExtensionElement = null;
            Parent = parent;
            if (SubNodes != null)
            {
                var tags = Utility.GetTagAttributes(typeof(ExtensionElements));
                var extElem = SubNodes
                    .FirstOrDefault(n => n.NodeType==XmlNodeType.Element && tags.Any(xt => xt.Matches(map, n.Name)));
                if (extElem!=null)
                    ExtensionElement=new ExtensionElements((XmlElement)extElem, map,this);
            }
        }

        public sealed override string ToString()
        {
            if (this.GetType().Name == "TextAnnotation")
                return (string)this.GetType().GetProperty("Content").GetValue(this, Array.Empty<object>());
            return this["name"]??String.Empty;
        }

        public virtual bool IsValid(out string[] err)
        {
            err = null;
            return true;
        }

        protected void Debug(string message, object[] pars)
        {
            Definition.Debug(this, message, pars);
        }

        protected void Info(string message, object[] pars)
        {
            Definition.Info(this, message, pars);
        }

        protected void Error(string message)
        {
            Definition.Error(this, message);
        }

        protected Exception Exception(Exception exception)
        {
            return Definition.Exception(this, exception);
        }

        internal void LoadExtensionElement(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            if (ExtensionElement!=null)
                ((ExtensionElements)ExtensionElement).LoadChildren(ref map, ref cache);
        }
    }
}
