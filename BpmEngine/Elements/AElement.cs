using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    internal abstract class AElement : IElement
    {
        private readonly AElement _parent;
        public AElement Parent { get { return _parent; } }

        public Definition Definition
        {
            get
            {
                if (this is Definition definition)
                    return definition;
                if (Parent != null)
                    return Parent.Definition;
                return null;
            }
        }

        private readonly XmlElement _element;
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
                            _cachedID = Utility.FindXPath(Definition,_element);
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

        private readonly IElement _extensionElement;
        public IElement ExtensionElement { get { return _extensionElement; } }

        public AElement(XmlElement elem,XmlPrefixMap map,AElement parent)
        {
            _element = elem;
            _extensionElement = null;
            _parent = parent;
            if (SubNodes != null)
            {
                var tags = Utility.GetTagAttributes(typeof(ExtensionElements));
                var extElem = SubNodes
                    .FirstOrDefault(n => n.NodeType==XmlNodeType.Element && tags.Any(xt => xt.Matches(map, n.Name)));
                if (extElem!=null)
                    _extensionElement=new ExtensionElements((XmlElement)extElem, map,this);
            }
        }

        public sealed override string ToString()
        {
            if (this.GetType().Name == "TextAnnotation")
                return (string)this.GetType().GetProperty("Content").GetValue(this, new object[] { });
            return (this["name"]==null ? "" : this["name"]);
        }

        public virtual bool IsValid(out string[] err)
        {
            err = null;
            return true;
        }

        public void Debug(string message)
        {
            Definition.Debug(this,message);
        }

        public void Debug(string message, object[] pars)
        {
            Definition.Debug(this, message, pars);
        }

        public void Info(string message)
        {
            Definition.Info(this, message);
        }

        public void Info(string message, object[] pars)
        {
            Definition.Info(this, message, pars);
        }

        public void Error(string message)
        {
            Definition.Error(this, message);
        }

        public void Error(string message, object[] pars)
        {
            Definition.Error(this, message, pars);
        }

        public void Fatal(string message)
        {
            Definition.Fatal(this, message);
        }

        public void Fatal(string message, object[] pars)
        {
            Definition.Fatal(this, message, pars);
        }

        public Exception Exception(Exception exception)
        {
            return Definition.Exception(this, exception);
        }

        internal void LoadExtensionElement(ref XmlPrefixMap map, ref ElementTypeCache cache)
        {
            if (_extensionElement!=null)
                ((Org.Reddragonit.BpmEngine.Elements.Processes.ExtensionElements)_extensionElement).LoadChildren(ref map, ref cache);
        }
    }
}
