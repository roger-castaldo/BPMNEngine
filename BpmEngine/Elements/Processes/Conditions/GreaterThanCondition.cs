using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "greaterThanCondition")]
    internal class GreaterThanCondition : ACompareCondition
    {
        public GreaterThanCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool _Evaluate(IReadonlyVariables variables)
        {
            return _Compare(variables) > 0;
        }
    }
}
