using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.Extensions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "messageEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class MessageEventDefinition : AParentElement
    {
        public MessageEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public IEnumerable<string> MessageTypes => new string[] { }.Concat(Children
                    .Where(elem => elem is MessageDefinition)
                    .Select(elem => (MessageDefinition)elem)
                    .Select(ed => ed.Name ?? "*")
                ).Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .Where(elem => elem is MessageDefinition)
                    .Select(elem => (MessageDefinition)elem)
                    .Select(ed => ed.Name ?? "*")
                ).Distinct()
                .DefaultIfEmpty("*");

        public override bool IsValid(out string[] err)
        {
            if (MessageTypes.Any())
            {
                List<string> errors = new List<string>();
                IElement[] elems = Definition.LocateElementsOfType((Parent is IntermediateThrowEvent ? typeof(IntermediateCatchEvent) : typeof(IntermediateThrowEvent)));
                if (Parent is IntermediateThrowEvent)
                {
                    if (MessageTypes.Count() > 1)
                        errors.Add("A throw event can only have one error to be thrown.");
                    bool found = elems
                        .Select(elem => (IntermediateCatchEvent)elem)
                        .Any(catcher => catcher.Children
                            .Any(child => child is MessageEventDefinition && ((MessageEventDefinition)child).MessageTypes.Contains(MessageTypes.First()))
                        ) ||
                        elems
                        .Select(elem => (IntermediateCatchEvent)elem)
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
