using BPMNEngine.Elements;
using System.Text.RegularExpressions;

namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
    internal class AttributeRegexAttribute(string name, string regex) : Attribute
    {
        public string Name { get; private init; } = name;
        public Regex Reg { get; private init; } = new Regex(regex, RegexOptions.Compiled|RegexOptions.ECMAScript);

        public bool IsValid(AElement elem)
            => elem[Name]!=null && Reg.IsMatch(elem[Name]);
    }
}
