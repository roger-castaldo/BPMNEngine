using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    internal abstract class ANegatableConditionSet : AConditionSet
    {
        protected bool _negated { get { return (this["negated"] == null ? false : bool.Parse(this["negated"])); } }

        protected abstract bool _Evaluate(IReadonlyVariables variables);

        public ANegatableConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public sealed override bool Evaluate(IReadonlyVariables variables)
        {
            bool ret = _Evaluate(variables);
            return (_negated ? !ret : ret);
        }
    }
}
