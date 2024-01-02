using BPMNEngine.Interfaces.Variables;
using System.Collections;
using System.Linq;

namespace BPMNEngine.Elements.Processes.Conditions
{
    internal abstract class ACompareCondition : ANegatableCondition
    {
        private readonly XmlPrefixMap _map;

        public ACompareCondition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
                _map = map;
        }

        protected object GetLeft(IReadonlyVariables variables)
        {
            return this["leftVariable"] != null?
                ACompareCondition.ExtractVariable(variables, this["leftVariable"])
                : SubNodes
                    .Where(n => n.NodeType==XmlNodeType.Element && (_map.IsMatch("exts", "left", n.Name) || n.Name == "left"))
                    .Select(n => n.InnerText)
                    .FirstOrDefault();
        }

        protected object GetRight(IReadonlyVariables variables)
        {
            return this["rightVariable"] != null
                ? ACompareCondition.ExtractVariable(variables, this["rightVariable"])
                : SubNodes
                    .Where(n => n.NodeType==XmlNodeType.Element && (_map.IsMatch("exts", "right", n.Name) || n.Name == "right"))
                    .Select(n => n.InnerText)
                    .FirstOrDefault();
        }

        protected int Compare(IReadonlyVariables variables)
            => ACompareCondition.Compare(GetLeft(variables), GetRight(variables), variables);

        protected static int Compare(object left, object right, IReadonlyVariables variables)
        {
            if (left == null && right != null)
                return -1;
            else if (left != null && right == null)
                return 1;
            else if (left==null && right==null)
                return 0;
            else
            {
                if (left is string ls && right is string rs)
                    return ls.CompareTo(rs);
                else
                {
                    if (left is string ls1 && right is not string)
                        left = ACompareCondition.ConvertToType(ls1, right.GetType(), variables);
                    else if (left is not string && right is string rs1)
                        right = ACompareCondition.ConvertToType(rs1, left.GetType(), variables);
                    else if (left.GetType() == right.GetType() && left is IComparable lic)
                        return lic.CompareTo(right);
                    return left.ToString().CompareTo(right.ToString());
                }
            }
        }

        private static object ExtractVariable(object source, string name)
        {
            object ret = null;
            if (source is IReadonlyVariables readonlyVariables)
            {
                if (!name.Contains('.'))
                    ret = readonlyVariables[name];
                else if (readonlyVariables[name[..name.IndexOf('.')]] != null)
                    ret = ExtractVariable(readonlyVariables[name[..name.IndexOf('.')]], name[(name.IndexOf('.') + 1)..]);
            } else if (source is IVariables variables)
            {
                if (!name.Contains('.'))
                    ret = variables[name];
                else if (variables[name[..name.IndexOf('.')]] != null)
                    ret = ExtractVariable(variables[name[..name.IndexOf('.')]], name[(name.IndexOf('.') + 1)..]);
            }else if (source is Hashtable hashtable)
            {
                if (!name.Contains('.'))
                {
                    if (hashtable.ContainsKey(name))
                        ret = hashtable[name];
                } else
                {
                    if (hashtable.ContainsKey(name[..name.IndexOf('.')]))
                        ret = ExtractVariable(hashtable[name[..name.IndexOf('.')]], name[(name.IndexOf('.') + 1)..]);
                }
            }else if (source is Array arr)
            {
                var al = new ArrayList();
                arr.Cast<object>().ForEach(o => al.Add(ExtractVariable(o, name)));
                if (al.Count > 0)
                {
                    ret = Array.CreateInstance(al[0].GetType(), al.Count);
                    al.Cast<object>().Select((v, i) => new { value = v, index = i }).ForEach(o => ((Array)ret).SetValue(o.value, o.index));
                }
            }
            return ret;
        }

        private static object ConvertToType(string value, Type type, IReadonlyVariables variables)
        =>type.FullName switch
        {
            "System.Boolean" => bool.Parse(value),
            "System.Byte[]" => Convert.FromBase64String(value),
            "System.Char" => value[0],
            "System.DateTime" => new DateString(value).GetTime(variables),
            "System.Decimal" => decimal.Parse(value),
            "System.Double" => double.Parse(value),
            "System.Single" => float.Parse(value),
            "System.Int32" => int.Parse(value),
            "System.Int64" => long.Parse(value),
            "System.Int16" => short.Parse(value),
            _ => value
        }; 

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            bool foundLeft = this["leftVariable"]!=null;
            bool foundRight = this["rightVariable"]!=null;
            var errs = new List<string>();
            SubNodes.OfType<XmlElement>().Select(n => n.Name).ForEach(name =>
            {
                if (_map.IsMatch("exts", "right", name) || name == "right")
                {
                    if (foundRight)
                        errs.Add("Right value specified more than once.");
                    foundRight = true;
                }
                else if (_map.IsMatch("exts", "left", name) || name == "left")
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
            err=(err??Array.Empty<string>()).Concat(errs);
            return res&&!errs.Any();
        }
    }
}
