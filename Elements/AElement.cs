using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    internal abstract class AElement : IElement
    {
        private XmlElement _element;
        internal XmlElement Element { get { return _element; } }

        public string id
        {
            get { return (_GetAttributeValue("id")==null ? Utility.FindXPath(_element) : _GetAttributeValue("id")); }
        }

        public XmlNode[] SubNodes
        {
            get {
                List<XmlNode> ret = new List<XmlNode>();
                foreach (XmlNode n in _element.ChildNodes)
                    ret.Add(n);
                return ret.ToArray();
            }
        }

        public XmlAttribute[] Attributes
        {
            get
            {
                List<XmlAttribute> ret = new List<XmlAttribute>();
                foreach (XmlAttribute att in _element.Attributes)
                    ret.Add(att);
                return ret.ToArray();
            }
        }

        private IElement _extensionElement;
        public IElement ExtensionElement { get { return _extensionElement; } }

        public AElement(XmlElement elem,XmlPrefixMap map)
        {
            _element = elem;
            _extensionElement = null;
            if (SubNodes != null)
            {
                Type t = typeof(ExtensionElements);
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        foreach (XMLTag xt in t.GetCustomAttributes(typeof(XMLTag), false))
                        {
                            if (xt.Matches(map, n.Name))
                            {
                                _extensionElement = new ExtensionElements((XmlElement)n,map);
                                break;
                            }
                        }
                        if (_extensionElement != null)
                            break;
                    }
                }
            }
        }

        protected string _GetAttributeValue(string name)
        {
            return (_element.Attributes[name] == null ? null : _element.Attributes[name].Value);
        }

        public sealed override string ToString()
        {
            if (this.GetType().Name == "TextAnnotation")
                return (string)this.GetType().GetProperty("Content").GetValue(this, new object[] { });
            return _GetAttributeValue("name");
        }

        public virtual bool IsValid(out string err)
        {
            err = null;
            return true;
        }
    }
}
