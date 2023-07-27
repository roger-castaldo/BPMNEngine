using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTag("bpmn","textAnnotation")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    [ValidParent(typeof(IProcess))]
    internal class TextAnnotation : AParentElement
    {
        public string Content => Children
                    .OfType<Text>()
                    .Select(elem=>elem.Value)
                    .FirstOrDefault() ?? String.Empty;

        public TextAnnotation(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out string[] err)
        {
            if (String.IsNullOrEmpty(Content))
            {
                err = new string[] { "No content was specified." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
