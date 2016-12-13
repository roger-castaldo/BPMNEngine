using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    internal abstract class AParentElement : AElement,IParentElement
    {
        private List<IElement> _children;
        public IElement[] Children
        {
            get { return (_children == null ? new IElement[] { } : _children.ToArray()); }
        }

        public AParentElement(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem,map,parent)
        {
            if (SubNodes != null)
            {
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        IElement subElem = Utility.ConstructElementType((XmlElement)n, map,this);
                        if (subElem != null)
                        {
                            if (_children == null)
                                _children = new List<IElement>();
                            _children.Add(subElem);
                        }
                    }
                }
            }
        }
    }
}
