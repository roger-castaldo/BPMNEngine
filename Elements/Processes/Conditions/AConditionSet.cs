using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    internal abstract class AConditionSet : ACondition
    {
        public AConditionSet(XmlElement elem)
            : base(elem) { }

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
                }
                return ret.ToArray();
            }
        }
    }
}
