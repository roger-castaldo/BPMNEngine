using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "isNull")]
    [RequiredAttribute("id")]
    internal class IsNull : ANegatableCondition
    {
        public IsNull(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        protected override bool _Evaluate(ProcessVariablesContainer variables)
        {
            return variables[_GetAttributeValue("variable")] == null;
        }
    }
}
