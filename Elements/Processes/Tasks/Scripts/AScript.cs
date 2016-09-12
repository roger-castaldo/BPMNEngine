using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks.Scripts
{
    internal abstract class AScript : AElement
    {
        public AScript(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        protected abstract void _Invoke(ref ProcessVariablesContainer variables);

        public void Invoke(ref ProcessVariablesContainer variables)
        {
            try
            {
                _Invoke(ref variables);
            }
            catch (Exception e) {
                throw e;
            }
        }
    }
}
