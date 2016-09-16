using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "ConditionSet")]
    internal class ConditionSet : AConditionSet
    {
        public ConditionSet(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override bool Evaluate(ProcessVariablesContainer variables)
        {
            return _Conditions[0].Evaluate(variables);
        }

        public override bool IsValid(out string err)
        {
            if (Children.Length > 1)
            {
                err = "Too many children found.";
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
