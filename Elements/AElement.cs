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

        public string id
        {
            get { return _GetAttributeValue("id"); }
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

        public AElement(XmlElement elem)
        {
            _element = elem;
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
    }
}
