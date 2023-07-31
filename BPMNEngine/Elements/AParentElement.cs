using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements
{
    internal abstract class AParentElement : AElement,IParentElement
    {
        public IEnumerable<IElement> Children { get; private set; }

        public void LoadChildren(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            if (SubNodes != null)
            {
                foreach (XmlElement elem in SubNodes.OfType<XmlElement>())
                {
                    IElement subElem = Utility.ConstructElementType(elem, ref map, ref cache, this);
                    if (subElem != null)
                    {
                        Children=Children.Append(subElem);
                        if (subElem is AParentElement element)
                            element.LoadChildren(ref map, ref cache);
                        else
                            ((AElement)subElem).LoadExtensionElement(ref map, ref cache);
                    }
                }
            }
            LoadExtensionElement(ref map, ref cache);
        }

        public AParentElement(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
            Children=new List<IElement>();
        }
    }
}
