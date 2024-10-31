using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    internal abstract record AParentElement : AElement, IParentElement
    {
        private readonly IElementFactory elementFactory;
        private ImmutableArray<IBaseElement>? children = null;
        public ImmutableArray<IBaseElement> Children
            => children ??= SubNodes.OfType<XmlElement>()
                    .Where(e => elementFactory.IsOfType<IElement>(e))
                    .Select(e => elementFactory.ProduceInstance(e, this))
                    .OfType<IBaseElement>()
                    .Concat(
                        SubNodes.OfType<XmlElement>()
                        .Where(e => elementFactory.IsOfType<IExtensionElement>(e))
                        .Select(e => elementFactory.ProduceExtensionElement(e, this))
                        .OfType<IBaseElement>()
                    )
                    .ToImmutableArray();
        protected AParentElement(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent,elementFactory)
            => this.elementFactory = elementFactory;
    }
}
