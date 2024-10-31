using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using System.Collections.Immutable;

namespace BPMNEngine.Extensions
{
    internal abstract record AExtension(XmlElement Element, IBaseElement? Parent) : IExtensionElement
    {
        public ImmutableArray<XmlNode> SubNodes { get; private init; } = Element.ChildNodes.Cast<XmlNode>().ToImmutableArray();
        protected string? this[string attributeName] => Element.Attributes.Cast<XmlAttribute>()
                    .Where(att => string.Equals(att.Name, attributeName, StringComparison.InvariantCultureIgnoreCase))
                    .Select(att => att.Value)
                    .FirstOrDefault() ?? null;

        public virtual (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
            => (true, []);
    }
}
