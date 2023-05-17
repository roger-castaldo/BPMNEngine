using BpmEngine.Attributes;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements
{
    [XMLTag("bpmn","collaboration")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Collaboration : AParentElement
    {
        public Collaboration(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out string[] err)
        {
            if (!Children.Any())
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
