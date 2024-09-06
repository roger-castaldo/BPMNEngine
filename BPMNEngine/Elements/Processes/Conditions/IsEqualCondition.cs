﻿using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTagAttribute("exts", "isEqualCondition")]
    internal record IsEqualCondition : ACompareCondition
    {
        public IsEqualCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override ValueTask<bool> EvaluateConditionAsync(IReadonlyVariables variables, IElement owningElement)
            => ValueTask.FromResult<bool>(Compare(variables) == 0);
    }
}
