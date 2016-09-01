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

        public AParentElement(XmlElement elem)
            : base(elem)
        {
            if (SubNodes != null)
            {
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        Type t = Utility.LocateElementType(n.Name);
                        if (t != null)
                        {
                            if (_children == null)
                                _children = new List<IElement>();
                            _children.Add((IElement)t.GetConstructor(new Type[] { typeof(XmlElement) }).Invoke(new object[] { (XmlElement)n }));
                        }
                    }
                }
            }
        }
    }
}
