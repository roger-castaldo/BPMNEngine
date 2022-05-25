using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited =true)]
    internal class ValidParentAttribute : Attribute
    {
        private Type _parent;
        public Type Parent { get { return _parent; } }

        public ValidParentAttribute(Type parent)
        {
            _parent = parent;
        }
    }
}
