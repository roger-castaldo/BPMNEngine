using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("andCondition")]
    internal class AndCondition : AConditionSet
    {
        public AndCondition(XmlElement elem) :
            base(elem) { }

        public override bool Evaluate(ProcessVariablesContainer variables)
        {
            bool ret = true;
            if (_Conditions != null)
            {
                foreach (ACondition cond in _Conditions)
                    ret = ret && cond.Evaluate(variables);
            }
            return ret;
        }
    }
}
