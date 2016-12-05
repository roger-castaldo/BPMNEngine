using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn","collaboration")]
    [RequiredAttribute("id")]
    internal class Collaboration : AParentElement
    {
        public Collaboration(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override bool IsValid(out string[] err)
        {
            if (Children.Length == 0)
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
