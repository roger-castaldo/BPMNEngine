using BPMNEngine.Elements.Processes;
using BPMNEngine.Interfaces.Elements;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    internal abstract record AElement
        : IElement
    {
        public AElement Parent { get; private init; }
        internal XmlElement Element { get; private init; }
        public ImmutableArray<XmlNode> SubNodes { get; private init; }
        public IParentElement ExtensionElement { get; private init; }

        protected AElement(XmlElement elem, XmlPrefixMap map, AElement parent)
        {
            Parent=parent;
            Element=elem;
            SubNodes=elem.ChildNodes.Cast<XmlNode>().ToImmutableArray();
            ExtensionElement = SubNodes.OfType<XmlElement>()
                .Where(n => Array.Exists(Utility.GetTagAttributes(typeof(ExtensionElements)),xt => xt.Matches(map, n.Name)))
                .Select(extElem => new ExtensionElements(extElem, map, this))
                .FirstOrDefault();
        }

        public virtual Definition OwningDefinition
            => Parent?.OwningDefinition;

        private string _cachedID=null;

        public string ID
        {
            get {
                var ret = _cachedID??this["id"];
                if (ret==null)
                    _cachedID = Utility.FindXPath(OwningDefinition, Element);
                return ret??_cachedID;
            }
        }

        

        public string this[string attributeName] => Element.Attributes.Cast<XmlAttribute>()
                    .Where(att => string.Equals(att.Name,attributeName,StringComparison.InvariantCultureIgnoreCase))
                    .Select(att => att.Value)
                    .FirstOrDefault()??null;

        public sealed override string ToString()
        {
            if (this.GetType().Name == "TextAnnotation")
                return (string)this.GetType().GetProperty("Content").GetValue(this, []);
            return this["name"]??String.Empty;
        }

        public virtual bool IsValid(out IEnumerable<string> err)
        {
            err = null;
            return true;
        }

        protected void Debug(string message, params object?[] pars)
            => OwningDefinition.LogLine(LogLevel.Debug, this, message, pars);

        protected void Info(string message, params object?[] pars)
            => OwningDefinition.LogLine(LogLevel.Information, this, message, pars);

        protected Exception Exception(Exception exception)
            => OwningDefinition.Exception(this, exception);

        internal void LoadExtensionElement(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            if (ExtensionElement!=null)
                ((ExtensionElements)ExtensionElement).LoadChildren(ref map, ref cache);
        }
    }
}
