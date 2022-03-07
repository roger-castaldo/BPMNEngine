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
            bool ret = true;
            if (_Conditions != null)
            {
                foreach (ACondition cond in _Conditions)
                    ret = ret && cond.Evaluate(variables);
            }
            return ret;
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
