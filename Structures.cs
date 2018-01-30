using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    public struct sProcessRuntimeConstant
    {
        private string _name;
        public string Name { get { return _name; } }

        private object _value;
        public object Value { get { return _value; } }

        public sProcessRuntimeConstant(string name,object value)
        {
            _name = name;
            _value = value;
        }
    }
}
