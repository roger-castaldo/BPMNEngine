using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    internal abstract class ACompareCondition : ANegatableCondition
    {
        private XmlPrefixMap _map;

        public ACompareCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
                _map = map;
        }

        protected object _GetLeft(IReadonlyVariables variables)
        {
            if (this["leftVariable"] != null)
                return _extractVariable(variables, this["leftVariable"]);
            else if (SubNodes!=null)
            {
                return SubNodes
                    .Where(n => n.NodeType==XmlNodeType.Element && (_map.isMatch("exts", "left", n.Name) || n.Name == "left"))
                    .Select(n => n.InnerText)
                    .FirstOrDefault();
            }
            return null;
        }

        protected object _GetRight(IReadonlyVariables variables)
        {
            if (this["rightVariable"] != null)
                return _extractVariable(variables, this["rightVariable"]);
            else if (SubNodes!=null)
            {
                return SubNodes
                    .Where(n => n.NodeType==XmlNodeType.Element && (_map.isMatch("exts", "right", n.Name) || n.Name == "right"))
                    .Select(n => n.InnerText)
                    .FirstOrDefault();
            }
            return null;
        }

        protected int _Compare(IReadonlyVariables variables)
        {
            return _Compare(_GetLeft(variables), _GetRight(variables), variables);
        }

        protected int _Compare(object left, object right, IReadonlyVariables variables)
        {
            if (left == null && right != null)
                return -1;
            else if (left != null && right == null)
                return 1;
            else if (left==null && right==null)
                return 0;
            else
            {
                if (left is string && right is string)
                    return ((string)left).CompareTo(right);
                else
                {
                    if (left is string && !(right is string))
                        left = _ConvertToType((string)left, right.GetType(), variables);
                    else if (!(left is string) && right is string)
                        right = _ConvertToType((string)right, left.GetType(), variables);
                    else if (left.GetType() == right.GetType() && left is IComparable)
                        return ((IComparable)left).CompareTo(right);
                    return left.ToString().CompareTo(right.ToString());
                }
            }
        }

        private object _extractVariable(object source, string name)
        {
            object ret = null;
            if (source is IReadonlyVariables readonlyVariables)
            {
                if (!name.Contains("."))
                    ret = readonlyVariables[name];
                else if (readonlyVariables[name.Substring(0, name.IndexOf("."))] != null)
                    ret = _extractVariable(readonlyVariables[name.Substring(0, name.IndexOf("."))], name.Substring(name.IndexOf(".") + 1));
            } else if (source is IVariables variables)
            {
                if (!name.Contains("."))
                    ret = variables[name];
                else if (variables[name.Substring(0, name.IndexOf("."))] != null)
                    ret = _extractVariable(variables[name.Substring(0, name.IndexOf("."))], name.Substring(name.IndexOf(".") + 1));
            }else if (source is Hashtable hashtable)
            {
                if (!name.Contains("."))
                {
                    if (hashtable.ContainsKey(name))
                        ret = hashtable[name];
                } else
                {
                    if (hashtable.ContainsKey(name.Substring(0, name.IndexOf("."))))
                        ret = _extractVariable(hashtable[name.Substring(0, name.IndexOf("."))], name.Substring(name.IndexOf(".") + 1));
                }
            }else if (source is Array)
            {
                ArrayList al = new ArrayList();
                ((IEnumerable)source).Cast<object>().ForEach(o => al.Add(_extractVariable(o, name)));
                if (al.Count > 0)
                {
                    ret = Array.CreateInstance(al[0].GetType(), al.Count);
                    for (int x = 0; x < al.Count; x++)
                        ((Array)ret).SetValue(al[x], x);
                }
            }
            return ret;
        }

        private object _ConvertToType(string value, Type type, IReadonlyVariables variables)
        {
            object ret = value;
            switch (type.FullName)
            {
                case "System.Boolean":
                    ret = bool.Parse(value);
                    break;
                case "System.Byte[]":
                    ret = Convert.FromBase64String(value);
                    break;
                case "System.Char":
                    ret = value[0];
                    break;
                case "System.DateTime":
                    ret = new DateString(value).GetTime(variables);
                    break;
                case "System.Decimal":
                    ret = decimal.Parse(value);
                    break;
                case "System.Double":
                    ret = double.Parse(value);
                    break;
                case "System.Single":
                    ret = float.Parse(value);
                    break;
                case "System.Int32":
                    ret = int.Parse(value);
                    break;
                case "System.Int64":
                    ret = long.Parse(value);
                    break;
                case "System.Int16":
                    ret = short.Parse(value);
                    break;
            }
            return ret;
        }

        public override bool IsValid(out string[] err)
        {
            bool foundLeft = this["leftVariable"]!=null;
            bool foundRight = this["rightVariable"]!=null;
            List<string> errs = new List<string>();
            SubNodes.Where(n => n.NodeType == XmlNodeType.Element).Select(n => n.Name).ForEach(name =>
            {
                if (_map.isMatch("exts", "right", name) || name == "right")
                {
                    if (foundRight)
                        errs.Add("Right value specified more than once.");
                    foundRight = true;
                }
                else if (_map.isMatch("exts", "left", name) || name == "left")
                {
                    if (foundLeft)
                        errs.Add("Left value specified more than once.");
                    foundLeft = true;
                }
            });
            if (!foundRight && !foundLeft)
                errs.Add("Right and Left value missing.");
            else if (!foundRight)
                errs.Add("Right value missing.");
            else if (!foundLeft)
                errs.Add("Left value missing.");
            if (errs.Count>0)
            {
                err = errs.ToArray();
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
