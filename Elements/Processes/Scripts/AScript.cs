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
