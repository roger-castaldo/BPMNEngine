using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts","andCondition")]
    internal class AndCondition : ANegatableConditionSet
    {
        public AndCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool _Evaluate(IReadonlyVariables variables)
        {
            foreach (ACondition cond in _Conditions)
            {
                if (!cond.Evaluate(variables))
                    return false;
            }
            return true;
        }

        public override bool IsValid(out string[] err)
        {
            if (Children.Length < 2 )
            {
                err = new string[] { "Not enough child elements found" };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
