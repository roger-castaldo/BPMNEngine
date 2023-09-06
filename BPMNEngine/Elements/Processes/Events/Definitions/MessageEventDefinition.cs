using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions.Extensions;
using BPMNEngine.Interfaces.Elements;
using System.Linq;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "messageEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class MessageEventDefinition : AParentElement
    {
        public IEnumerable<string> MessageTypes 
            => Array.Empty<string>()
                .Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .OfType<MessageDefinition>()
                    .Select(ed => ed.Name ?? "*")
                ).Distinct()
                .DefaultIfEmpty("*");

        public MessageEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (MessageTypes.Any())
            {
                var errors = new List<string>();
                if (Parent is IntermediateThrowEvent)
                {
                    if (MessageTypes.Count() > 1)
                        errors.Add("A throw event can only have one error to be thrown.");
                    var elems = Definition.LocateElementsOfType<IntermediateCatchEvent>();
                    bool found = elems
                        .Any(catcher => catcher.Children
                            .Any(child => child is MessageEventDefinition definition && definition.MessageTypes.Contains(MessageTypes.First()))
                        ) ||
                        elems
                        .Any(catcher => catcher.Children
                            .Any(child => child is MessageEventDefinition definition && definition.MessageTypes.Contains("*"))
                        );
                    if (!found)
                        errors.Add("A defined message needs to have a Catch Event with a corresponding type or all");
                }
                err = (err??Array.Empty<string>()).Concat(errors);
                return res&&!errors.Any();
            }
            return res;
        }
    }
}
