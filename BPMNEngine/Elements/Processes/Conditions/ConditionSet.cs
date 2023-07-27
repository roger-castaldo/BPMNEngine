using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "ConditionSet")]
    [ValidParent(typeof(ExtensionElements))]
    internal class ConditionSet : AConditionSet
    {
        public ConditionSet(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool Evaluate(IReadonlyVariables variables)
        {
            try
            {
                return _Conditions.First().Evaluate(variables);
            }catch(Exception ex)
            {
                Error(ex.Message);
                return false;
            }
        }

        public override bool IsValid(out string[] err)
        {
            if (Children.Count() > 1)
            {
                err = new string[] { "Too many children found." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
