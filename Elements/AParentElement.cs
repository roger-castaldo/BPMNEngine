using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    internal abstract class AParentElement : AElement,IParentElement
    {
        private XmlPrefixMap _map;

        private List<IElement> _children=null;
        public IElement[] Children
        {
            get {   
                if (_children == null)
                {
                    _children = new List<IElement>();
                    if (SubNodes != null)
                    {
                        foreach (XmlNode n in SubNodes)
                        {
                            if (n.NodeType == XmlNodeType.Element)
                            {
                                IElement subElem = Utility.ConstructElementType((XmlElement)n,_map , this);
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
                return _children.ToArray(); 
            }
        }

        public AParentElement(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem,map,parent)
        {
            _map = map;
        }
    }
}
