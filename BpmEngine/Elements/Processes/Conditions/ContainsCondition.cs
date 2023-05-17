using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Conditions
{
    [XMLTag("exts", "containsCondition")]
    internal class ContainsCondition : ACompareCondition
    {
        public ContainsCondition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public IEnumerable<object> IEnumarable { get; private set; }

        protected override bool _Evaluate(IReadonlyVariables variables)
        {
            object right = _GetRight(variables);
            object left = _GetLeft(variables);
            if (left == null && right != null)
                return false;
            else if (left != null && right == null)
                return false;
            else if (left==null && right==null)
                return false;
            else
            {
                if (left is Array array)
                    return array.OfType<object>().Any(ol => _Compare(ol, right, variables)==0);
                else if (left is Hashtable hashtable)
                    return hashtable.Keys.OfType<object>().Any(ol => _Compare(ol, right, variables)==0)
                        || hashtable.Values.OfType<object>().Any(ol => _Compare(ol, right, variables)==0);
                else if (left.ToString().Contains(right.ToString()))
                    return true;
            }
            return false;
        }
    }
}
