using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    [RequiredAttribute("id")]
    internal abstract class AScript : AElement
    {
        private XmlPrefixMap _map;

        public AScript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
                _map = map;
        }

        protected string _Code
        {
            get
            {
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Text)
                        return n.Value;
                }
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.CDATA)
                        return ((XmlCDataSection)n).InnerText;
                }
                return "";
            }
        }

        protected bool _IsCondition
        {
            get
            {
                Type t = typeof(ConditionSet);
                XmlNode n = Element.ParentNode;
                while (n != null)
                {
                    foreach (XMLTag xt in t.GetCustomAttributes(typeof(XMLTag), true))
                    {
                        if (xt.Matches(_map, n.Name))
                        {
                            return true;
                        }
                    }
                    n = n.ParentNode;
                }
                return false;
            }
        }

        protected abstract object _Invoke(ProcessVariablesContainer variables);
        protected abstract bool _IsValid(out string[] err);

        public object Invoke(ProcessVariablesContainer variables)
        {
            try
            {
                return _Invoke(variables);
            }
            catch (Exception e) {
                throw e;
            }
        }

        public sealed override bool IsValid(out string[] err)
        {
            if (!_IsValid(out err))
                return false;
            return base.IsValid(out err);
        }
    }
}
