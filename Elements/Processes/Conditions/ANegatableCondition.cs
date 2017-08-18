using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    internal abstract class ANegatableCondition : ACondition
    {
        protected bool _negated { get { return (this["negated"]==null ? false : bool.Parse(this["negated"])); } }

        protected abstract bool _Evaluate(ProcessVariablesContainer variables);

        public ANegatableCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }

        public sealed override bool Evaluate(ProcessVariablesContainer variables)
        {
            bool ret = _Evaluate(variables);
            return (_negated ? !ret : ret);
        }
    }
}
