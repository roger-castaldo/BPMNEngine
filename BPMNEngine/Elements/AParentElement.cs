using BPMNEngine.Interfaces.Elements;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    internal abstract class AParentElement : AElement,IParentElement
    {
        public ImmutableArray<IElement> Children { get; private set; }

        public void LoadChildren(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            if (SubNodes != null)
            {
                var children = new List<IElement>();
                foreach (XmlElement elem in SubNodes.OfType<XmlElement>())
                {
                    IElement subElem = Utility.ConstructElementType(elem, ref map, ref cache, this);
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
            }
            LoadExtensionElement(ref map, ref cache);
        }

        public AParentElement(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
            Children=ImmutableArray.Create<IElement>();
        }
    }
}
