using Org.Reddragonit.BpmEngine.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.BpmEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
    internal class AttributeRegex : Attribute
    {
        private string _name;
        public string Name { get { return _name; } }
        private Regex _reg;
        public Regex Reg { get { return _reg; } }

        public AttributeRegex(string name, string regex)
        {
            _name = name;
            _reg=new Regex(regex,RegexOptions.Compiled|RegexOptions.ECMAScript);
        }

        public bool IsValid(AElement elem)
        {
            if (elem[_name]!=null)
                return _reg.IsMatch(elem[_name]);
            return true;
        }
    }
}
