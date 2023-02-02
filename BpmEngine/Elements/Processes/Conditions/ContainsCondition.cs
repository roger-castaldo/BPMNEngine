using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
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
                {
                    foreach (object ol in array)
                    {
                        if (_Compare(ol, right, variables) == 0)
                            return true;
                    }
                }
                else if (left is Hashtable hashtable)
                {
                    foreach (object ol in hashtable.Keys)
                    {
                        if (_Compare(ol, right, variables) == 0)
                            return true;
                    }
                    foreach (object ol in hashtable.Values)
                    {
                        if (_Compare(ol, right, variables) == 0)
                            return true;
                    }
                }
                else if (left.ToString().Contains(right.ToString()))
                    return true;
            }
            return false;
        }
    }
}
