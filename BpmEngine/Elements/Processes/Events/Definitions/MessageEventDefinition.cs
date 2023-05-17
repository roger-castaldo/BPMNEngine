using BpmEngine.Attributes;
using BpmEngine.Elements.Processes.Events.Definitions.Extensions;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "messageEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class MessageEventDefinition : AParentElement
    {
        public MessageEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public IEnumerable<string> MessageTypes => new string[] { }.Concat(
                Children
                    .OfType<MessageDefinition>()
                    .Select(ed => ed.Name ?? "*")
                ).Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .OfType<MessageDefinition>()
                    .Select(ed => ed.Name ?? "*")
                ).Distinct()
                .DefaultIfEmpty("*");

        public override bool IsValid(out string[] err)
        {
            if (MessageTypes.Any())
            {
                List<string> errors = new List<string>();
                if (Parent is IntermediateThrowEvent)
                {
                    if (MessageTypes.Count() > 1)
                        errors.Add("A throw event can only have one error to be thrown.");
                    var elems = Definition.LocateElementsOfType<IntermediateCatchEvent>();
                    bool found = elems
                        .Any(catcher => catcher.Children
                            .Any(child => child is MessageEventDefinition && ((MessageEventDefinition)child).MessageTypes.Contains(MessageTypes.First()))
                        ) ||
                        elems
                        .Any(catcher => catcher.Children
                            .Any(child => child is MessageEventDefinition && ((MessageEventDefinition)child).MessageTypes.Contains("*"))
                        );
                    if (!found)
                        errors.Add("A defined message needs to have a Catch Event with a corresponding type or all");
                }
                if (errors.Count > 0)
                {
                    err = errors.ToArray();
                    return true;
                }
            }
            return base.IsValid(out err);
        }
    }
}
