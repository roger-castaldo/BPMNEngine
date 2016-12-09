using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [RequiredAttribute("negated")]
    [RequiredAttribute("id")]
    internal abstract class ANegatableConditionSet : AConditionSet
    {
        protected bool _negated { get { return bool.Parse(this._GetAttributeValue("negated")); } }

        protected abstract bool _Evaluate(ProcessVariablesContainer variables);

        public ANegatableConditionSet(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public sealed override bool Evaluate(ProcessVariablesContainer variables)
        {
            bool ret = _Evaluate(variables);
            return (_negated ? !ret : ret);
        }
    }
}
