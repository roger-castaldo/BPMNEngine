using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "orCondition")]
    internal class OrCondition : ANegatableConditionSet
    {
        public OrCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool _Evaluate(IReadonlyVariables variables)
        {
            return _Conditions.Any(cond=>cond.Evaluate(variables));
        }

        public override bool IsValid(out string[] err)
        {
            if (Children.Count() < 2)
            {
                err = new string[] { "Not enough child elements found" };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
