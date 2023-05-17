using BpmEngine.Attributes;
using BpmEngine.Elements.Processes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements
{
    [XMLTag("exts", "DefinitionVariable")]
    [Required("Name")]
    [Required("Type")]
    [ValidParent(typeof(ExtensionElements))]
    internal class DefinitionVariable : AElement
    {
        public string Name { get { return this["Name"]; } }

        private object _value;
        public object Value { get { return _value; } }

        public DefinitionVariable(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
            string text = elem.InnerText;
            if (elem.ChildNodes[0].NodeType == XmlNodeType.CDATA)
                text = ((XmlCDataSection)elem.ChildNodes[0]).InnerText;
            _value = Utility.ExtractVariableValue((VariableTypes)Enum.Parse(typeof(VariableTypes), this["Type"]), text);
        }
    }
}
