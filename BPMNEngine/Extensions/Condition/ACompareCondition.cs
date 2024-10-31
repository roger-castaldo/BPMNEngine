using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System.Collections;

namespace BPMNEngine.Extensions.Conditions
{
    internal abstract record ACompareCondition : ANegatableCondition
    {
        protected ACompareCondition(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory) : 
            base(xmlElement, parent, elementFactory)
        {}

        protected object? GetLeft(IReadonlyVariables variables)
            => this["leftVariable"] != null ?
                ACompareCondition.ExtractVariable(variables, this["leftVariable"]!)
                : Children
                    .OfType<LeftVariable>()
                    .FirstOrDefault()?.Value;
        protected object? GetRight(IReadonlyVariables variables)
            => this["rightVariable"] != null
                ? ACompareCondition.ExtractVariable(variables, this["rightVariable"]!)
                : Children
                    .OfType<RightVariable>()
                    .FirstOrDefault()?.Value;

        protected int Compare(IReadonlyVariables variables)
            => ACompareCondition.Compare(GetLeft(variables), GetRight(variables), variables);

        protected static int Compare(object? left, object? right, IReadonlyVariables variables)
            => (left, right) switch
            {
                (null, not null) => -1,
                (not null, null) => 1,
                (null, null) => 0,
                (string ls, string rs) => ls.CompareTo(rs),
                (string ls, not string) => ACompareCondition.ConvertToType(ls!, right!.GetType(), variables).ToString()!.CompareTo(right!.ToString()),
                (not string, string rs) => left!.ToString()!.CompareTo(ACompareCondition.ConvertToType(rs, left.GetType(), variables).ToString()),
                (IComparable lic, _) when left.GetType()==right.GetType() => lic.CompareTo(right),
                _ => left!.ToString()!.CompareTo(right.ToString())
            };

        private static object? ExtractVariable(object source, string name)
        {
            object? ret = null;
            if (source is Array arr)
            {
                var al = new ArrayList();
                arr.Cast<object>().ForEach(o => al.Add(ExtractVariable(o, name)));
                if (al.Count > 0)
                {
                    ret = Array.CreateInstance(al[0].GetType(), al.Count);
                    al.Cast<object>().Select((v, i) => new { value = v, index = i }).ForEach(o => ((Array)ret).SetValue(o.value, o.index));
                }
                ret=al.ToArray();
            }
            else if (source is IVariablesContainer variablesContainer)
            {
                if (!name.Contains('.'))
                    ret = variablesContainer[name];
                else if (variablesContainer[name[..name.IndexOf('.')]] != null)
                    ret = ExtractVariable(variablesContainer[name[..name.IndexOf('.')]], name[(name.IndexOf('.') + 1)..]);
            }
            else if (source is IDictionary dictionary)
            {
                if (!name.Contains('.'))
                {
                    if (dictionary.Keys.OfType<object>().Any(o => o.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                        ret = dictionary[name];
                }
                else
                {
                    if (dictionary.Keys.OfType<object>().Any(o => o.ToString().Equals(name[..name.IndexOf('.')], StringComparison.InvariantCultureIgnoreCase)))
                        ret = ExtractVariable(dictionary[name[..name.IndexOf('.')]], name[(name.IndexOf('.') + 1)..]);
                }
            }
            return ret;
        }

        private static object ConvertToType(string value, Type type, IReadonlyVariables variables)
        => type.FullName switch
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

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            bool foundLeft = this["leftVariable"]!=null;
            bool foundRight = this["rightVariable"]!=null;
            var errs = new List<string>();
            if ((foundRight && Children.OfType<RightVariable>().Any())
                || Children.OfType<RightVariable>().Count()>1)
                errs.Add("Right value specified more than once.");
            else
                foundRight|=Children.OfType<RightVariable>().Any();
            if ((foundLeft && Children.OfType<LeftVariable>().Any())
                || Children.OfType<LeftVariable>().Count()>1)
                errs.Add("Left value specified more than once.");
            else
                foundLeft|=Children.OfType<LeftVariable>().Any();
            if (!foundRight && !foundLeft)
                errs.Add("Right and Left value missing.");
            else if (!foundRight)
                errs.Add("Right value missing.");
            else if (!foundLeft)
                errs.Add("Left value missing.");
            return (errs.Count==0, errs);
        }
    }
}
