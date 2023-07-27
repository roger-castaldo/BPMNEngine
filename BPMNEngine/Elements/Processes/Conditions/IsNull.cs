﻿using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "isNull")]
    internal class IsNull : ANegatableCondition
    {
        public IsNull(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override bool _Evaluate(IReadonlyVariables variables)
        {
            return variables[this["variable"]] == null;
        }
    }
}