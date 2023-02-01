using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    internal abstract class AParentElement : AElement,IParentElement
    {

        private readonly List<IElement> _children=null;
        public IElement[] Children
        {
            get {   
                return _children.ToArray();
            }
        }

        public void LoadChildren(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            if (SubNodes != null)
            {
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        IElement subElem = Utility.ConstructElementType((XmlElement)n, ref map, ref cache, this);
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
