using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "isEqualCondition")]
    internal class IsEqualCondition : ACompareCondition
    {
        public IsEqualCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool _Evaluate(IReadonlyVariables variables)
        {
            return _Compare(variables) == 0;
        }
    }
}
