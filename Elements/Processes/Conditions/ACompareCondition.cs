using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
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

        protected object _GetLeft(ProcessVariablesContainer variables)
        {
            object left = null;
            if (this["leftVariable"] != null)
                left = _extractVariable(variables, this["leftVariable"]);
            else
            {
                if (SubNodes != null)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (_map.isMatch("exts", "left", n.Name) || n.Name == "left")
                            {
                                left = n.InnerText;
                                break;
                            }
                        }
                    }
                }
            }
            return left;
        }

        protected object _GetRight(ProcessVariablesContainer variables)
        {
            object right = null;
            if (this["rightVariable"] != null)
                right = _extractVariable(variables, this["rightVariable"]);
            else
            {
                if (SubNodes != null)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (_map.isMatch("exts", "right", n.Name) || n.Name == "right")
                            {
                                right = n.InnerText;
                                break;
                            }
                        }
                    }
                }
            }
            return right;
        }

        protected int _Compare(ProcessVariablesContainer variables)
        {
            object left = _GetLeft(variables);
            object right = _GetRight(variables);
            return _Compare(left, right);
        }

        protected int _Compare(object left, object right)
        {
            if (left == null && right != null)
                return -1;
            else if (left != null && right == null)
                return 1;
            else
            {
                if (left is string && right is string)
                    return ((string)left).CompareTo(right);
                else
                {
                    if (left is string && !(right is string))
                        left = _ConvertToType((string)left, right.GetType());
                    else if (!(left is string) && right is string)
                        right = _ConvertToType((string)right, left.GetType());
                    else
                        return left.ToString().CompareTo(right.ToString());
                    return ((IComparable)left).CompareTo(right);
                }
            }
        }

        private object _extractVariable(object source, string name)
        {
            object ret = null;
            if (source is ProcessVariablesContainer)
            {
                if (!name.Contains("."))
                    ret = ((ProcessVariablesContainer)source)[name];
                else if (((ProcessVariablesContainer)source)[name.Substring(0, name.IndexOf("."))] != null)
                    ret = _extractVariable(((ProcessVariablesContainer)source)[name.Substring(0, name.IndexOf("."))], name.Substring(name.IndexOf(".") + 1));
            } else if (source is Hashtable)
            {
                if (!name.Contains("."))
                {
                    if (((Hashtable)source).ContainsKey(name))
                        ret = ((Hashtable)source)[name];
                } else
                {
                    if (((Hashtable)source).ContainsKey(name.Substring(0, name.IndexOf("."))))
                        ret = _extractVariable(((Hashtable)source)[name.Substring(0, name.IndexOf("."))], name.Substring(name.IndexOf(".") + 1));
                }
            }else if (source is Array)
            {
                ArrayList al = new ArrayList();
                foreach (object o in (IEnumerable)source)
                    al.Add(_extractVariable(o, name));
                if (al.Count > 0)
                {
                    ret = Array.CreateInstance(al[0].GetType(), al.Count);
                    for (int x = 0; x < al.Count; x++)
                        ((Array)ret).SetValue(al[x], x);
                }
            }
            return ret;
        }

        private object _ConvertToType(string value, Type type)
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
                    ret = DateTime.Parse(value);
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
            foreach (XmlNode n in SubNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (_map.isMatch("exts", "right", n.Name) || n.Name == "right")
                    {
                        if (foundRight)
                            errs.Add("Right value specified more than once.");
                        foundRight = true;
                    }
                    else if (_map.isMatch("exts", "left", n.Name) || n.Name == "left")
                    {
                        if (foundLeft)
                            errs.Add("Left value specified more than once.");
                        foundLeft = true;
                    }
                }
            }
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
