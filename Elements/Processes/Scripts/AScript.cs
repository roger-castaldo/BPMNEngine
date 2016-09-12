using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    internal abstract class AScript : AElement
    {
        public AScript(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        protected bool _IsCondition
        {
            get
            {
                XmlNode n = Element.ParentNode;
                while (n != null)
                {
                    if (n.Name == "conditionSet")
                        return true;
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
