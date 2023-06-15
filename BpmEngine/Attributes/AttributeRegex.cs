using BPMNEngine.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BPMNEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
    internal class AttributeRegex : Attribute
    {
        public string Name { get; private set; }
        public Regex Reg { get; private set; }

        public AttributeRegex(string name, string regex)
        {
            Name = name;
            Reg=new Regex(regex,RegexOptions.Compiled|RegexOptions.ECMAScript);
        }

        public bool IsValid(AElement elem)
        {
            if (elem[Name]!=null)
                return Reg.IsMatch(elem[Name]);
            return true;
        }
    }
}
