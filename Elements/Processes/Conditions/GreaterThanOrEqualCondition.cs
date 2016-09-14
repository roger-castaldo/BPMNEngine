using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "greaterThanOrEqualCondition")]
    internal class GreaterThanOrEqualCondition : ACompareCondition
    {
        public GreaterThanOrEqualCondition(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override bool Evaluate(ProcessVariablesContainer variables)
        {
            return _Compare(variables) >= 0;
        }
    }
}
