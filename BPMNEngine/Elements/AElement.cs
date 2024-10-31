using BPMNEngine.Elements.Processes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    internal abstract record AElement(XmlElement Element,IBaseElement? Parent)
        : IValidatableElement,IElement
    {
        public ImmutableArray<XmlNode> SubNodes { get; private init; } = Element.ChildNodes.Cast<XmlNode>().ToImmutableArray();
        public IExtensionsElement? ExtensionElement { get; private init; }

        protected AElement(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : this(elem,parent)
        => ExtensionElement = SubNodes.OfType<XmlElement>()
                .Where(n => elementFactory.IsOfType<ExtensionElements>(n))
                .Select(n => elementFactory.ProduceInstance(n, this))
                .OfType<IExtensionsElement>()
                .FirstOrDefault();

        protected T? GetParent<T>()
            where T : IBaseElement
        {
            var parent = Parent;
            while(parent != null)
            {
                if (parent is T result)
                    return result;
                parent = parent.Parent;
            }
            return default;
        }

        public virtual Definition? OwningDefinition
            => GetParent<Definition>();

        private string? cachedID = null;

        public string ID
        {
            get
            {
                var ret = cachedID??this["id"];
                if (ret==null)
                    cachedID = Utility.FindXPath(OwningDefinition, Element);
                return ret??cachedID??string.Empty;
            }
        }



        public string? this[string attributeName] => Element.Attributes.Cast<XmlAttribute>()
                    .Where(att => string.Equals(att.Name, attributeName, StringComparison.InvariantCultureIgnoreCase))
                    .Select(att => att.Value)
                    .FirstOrDefault()??null;

        public sealed override string ToString()
        {
            if (this.GetType().Name == "TextAnnotation")
                return (string?)this.GetType().GetProperty("Content")?.GetValue(this, [])??string.Empty;
            return this["name"]??String.Empty;
        }

        public virtual (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
            => (ExtensionElement==null ? (true, []) : ExtensionElement.IsValid(logger));
    }
}
