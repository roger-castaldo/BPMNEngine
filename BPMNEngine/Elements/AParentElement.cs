using BPMNEngine.Interfaces.Elements;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    internal abstract record AParentElement : AElement, IParentElement
    {
        protected AParentElement(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public ImmutableArray<IElement> Children { get; private set; } = [];

        public void LoadChildren(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            var children = new List<IElement>();
            foreach (XmlElement xelem in SubNodes.OfType<XmlElement>())
            {
                IElement subElem = Utility.ConstructElementType(xelem, ref map, ref cache, this);
                if (subElem != null)
                {
                    if (subElem is AParentElement element)
                        element.LoadChildren(ref map, ref cache);
                    else
                        ((AElement)subElem).LoadExtensionElement(ref map, ref cache);
                    children.Add(subElem);
                }
            }
            Children = children.ToImmutableArray();
            LoadExtensionElement(ref map, ref cache);
        }
    }
}
