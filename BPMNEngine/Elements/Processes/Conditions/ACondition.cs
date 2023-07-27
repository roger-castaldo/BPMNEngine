using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(AConditionSet))]
    internal abstract class ACondition : AParentElement
    {
        public abstract bool Evaluate(IReadonlyVariables variables);

        public ACondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
