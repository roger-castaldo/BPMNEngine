using Org.Reddragonit.BpmEngine.Elements.Processes.Scripts;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    internal abstract class AConditionSet : ACondition
    {
        protected AConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected IEnumerable<ACondition> _Conditions => Children
            .Where(child=>child is ACondition || child is AScript)
            .Select(child=>(ACondition)(child is AScript ? new ScriptCondition((AScript)child) : child));

        public override bool IsValid(out string[] err)
        {
            if (Children.Length == 0)
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
