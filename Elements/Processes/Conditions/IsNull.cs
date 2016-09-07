using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("isNull")]
    internal class IsNull : ACondition
    {
        public IsNull(XmlElement elem)
            : base(elem) { }

        public override bool Evaluate(ProcessVariablesContainer variables)
        {
            return variables[_GetAttributeValue("variable")] == null;
        }
    }
}
