using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "isEqualCondition")]
    internal class IsEqualCondition : ACompareCondition
    {
        public IsEqualCondition(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        protected override bool _Evaluate(ProcessVariablesContainer variables)
        {
            return _Compare(variables) == 0;
        }
    }
}
