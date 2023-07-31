using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;
using System.Collections;

namespace BPMNEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "containsCondition")]
    internal class ContainsCondition : ACompareCondition
    {
        public ContainsCondition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        protected override bool EvaluateCondition(IReadonlyVariables variables)
        {
            object right = GetRight(variables);
            object left = GetLeft(variables);
            if (left == null && right != null)
                return false;
            else if (left != null && right == null)
                return false;
            else if (left==null && right==null)
                return false;
            else
            {
                if (left is Array array)
                    return array.OfType<object>().Any(ol => ACompareCondition.Compare(ol, right, variables)==0);
                else if (left is Hashtable hashtable)
                    return hashtable.Keys.OfType<object>().Any(ol => ACompareCondition.Compare(ol, right, variables)==0)
                        || hashtable.Values.OfType<object>().Any(ol => ACompareCondition.Compare(ol, right, variables)==0);
                else if (left.ToString().Contains(right.ToString()))
                    return true;
            }
            return false;
        }
    }
}
