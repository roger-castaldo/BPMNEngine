using Org.Reddragonit.BpmEngine.Attributes;
using System;
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

        protected int _Compare(ProcessVariablesContainer variables)
        {
            int ret = -1;
            object left = null;
            if (_GetAttributeValue("leftVariable") != null)
                left = variables[_GetAttributeValue("leftVariable")];
            else
            {
                if (SubNodes != null)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (_map.isMatch("exts","left",n.Name) || n.Name=="left")
                            {
                                left = n.InnerText;
                                break;
                            }
                        }
                    }
                }
            }
            object right = null;
            if (_GetAttributeValue("rightVariable") != null)
                right = variables[_GetAttributeValue("rightVariable")];
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
            if (left == null && right != null)
                ret = -1;
            else if (left != null && right == null)
                return 1;
            else
            {
                if (left is string && right is string)
                    ret = ((string)left).CompareTo(right);
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
            bool foundLeft = this._GetAttributeValue("leftVariable")!=null;
            bool foundRight = this._GetAttributeValue("rightVariable")!=null;
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
