using BPMNEngine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements
{
    internal abstract class AParentElement : AElement,IParentElement
    {

        private readonly List<IElement> _children=null;
        public IEnumerable<IElement> Children => _children;

        public void LoadChildren(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            if (SubNodes != null)
            {
                foreach (XmlElement elem in SubNodes.OfType<XmlElement>())
                {
                    IElement subElem = Utility.ConstructElementType(elem, ref map, ref cache, this);
                    if (subElem != null)
                    {
                        _children.Add(subElem);
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
            _children=new List<IElement>();
        }
    }
}
