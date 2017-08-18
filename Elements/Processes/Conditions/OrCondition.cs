using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "orCondition")]
    internal class OrCondition : ANegatableConditionSet
    {
        public OrCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool _Evaluate(ProcessVariablesContainer variables)
        {
            bool ret = false;
            if (_Conditions != null)
            {
                foreach (ACondition cond in _Conditions)
                    ret = ret || cond.Evaluate(variables);
            }
            return ret;
        }

        public override bool IsValid(out string[] err)
        {
            if (Children.Length < 2)
            {
                err = new string[] { "Not enough child elements found" };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
