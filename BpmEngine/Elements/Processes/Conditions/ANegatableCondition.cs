using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract class ANegatableCondition : ACondition
    {
        protected bool _negated => (this["negated"]==null ? false : bool.Parse(this["negated"]));

        protected abstract bool _Evaluate(IReadonlyVariables variables);

        public ANegatableCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }

        public sealed override bool Evaluate(IReadonlyVariables variables)
        {
            bool ret = _Evaluate(variables);
            return (_negated ? !ret : ret);
        }
    }
}
