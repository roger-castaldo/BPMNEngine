using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("isEqualCondition")]
    internal class IsEqualCondition : ACompareCondition
    {
        public IsEqualCondition(XmlElement elem)
            : base(elem) { }

        public override bool Evaluate(ProcessVariablesContainer variables)
        {
            return _Compare(variables) == 0;
        }
    }
}
