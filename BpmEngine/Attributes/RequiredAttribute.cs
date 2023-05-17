using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
    internal class RequiredAttribute : Attribute
    {
        private string _name;
        public string Name { get { return _name; } }

        public RequiredAttribute(string name)
        {
            _name = name;
        }
    }
}
