using Org.Reddragonit.BpmEngine.Elements.Processes.Scripts;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    internal abstract class AConditionSet : ACondition
    {
        public AConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected ACondition[] _Conditions
        {
            get
            {
                if (Children == null)
                    return null;
                List<ACondition> ret = new List<ACondition>();
                foreach (IElement elem in Children)
                {
                    if (elem is ACondition)
                        ret.Add((ACondition)elem);
                    else if (elem is AScript)
                        ret.Add(new ScriptCondition((AScript)elem));
                }
                return ret.ToArray();
            }
        }

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
