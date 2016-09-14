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

        public AScript(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) {
                _map = map;
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
    }
}
