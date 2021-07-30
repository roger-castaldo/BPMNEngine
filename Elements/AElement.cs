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
        private AElement _parent;
        public AElement Parent { get { return _parent; } }

        public Definition Definition
        {
            get
            {
                if (this is Definition)
                    return (Definition)this;
                if (Parent != null)
                    return Parent.Definition;
                return null;
            }
        }

        private XmlElement _element;
        internal XmlElement Element { get { return _element; } }

        private string _cachedID=null;

        public string id
        {
            get {
                string ret = null; ;
                if (_cachedID == null)
                {
                    ret = this["id"];
                    if (ret == null)
                    {
                        if (_cachedID == null)
                            _cachedID = Utility.FindXPath(_element);
                        ret = _cachedID;
                    }
                    else
                        _cachedID = ret;
                }
                else
                    ret = _cachedID;
                return ret;
            }
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

        public string this[string attributeName]
        {
            get
            {
                string value = null;
                foreach (XmlAttribute att in _element.Attributes)
                {
                    if (att.Name.ToLower()==attributeName.ToLower())
                    {
                        value = att.Value;
                        break;
                    }
                }
                return value;
            }
        }

        private IElement _extensionElement;
        public IElement ExtensionElement { get { return _extensionElement; } }

        public AElement(XmlElement elem,XmlPrefixMap map,AElement parent)
        {
            _element = elem;
            _extensionElement = null;
            _parent = parent;
            if (SubNodes != null)
            {
                Type t = typeof(ExtensionElements);
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        foreach (XMLTag xt in Utility.GetTagAttributes(t))
                        {
                            if (xt.Matches(map, n.Name))
                            {
                                _extensionElement = new ExtensionElements((XmlElement)n,map,this);
                                break;
                            }
                        }
                        if (_extensionElement != null)
                            break;
                    }
                }
            }
        }

        public sealed override string ToString()
        {
            if (this.GetType().Name == "TextAnnotation")
                return (string)this.GetType().GetProperty("Content").GetValue(this, new object[] { });
            return this["name"];
        }

        public virtual bool IsValid(out string[] err)
        {
            err = null;
            return true;
        }
    }
}
