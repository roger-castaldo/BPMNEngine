using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("conditionSet")]
    internal class ConditionSet : AndCondition
    {
        public ConditionSet(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

    }
}
