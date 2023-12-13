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
        private IEnumerable<string> BaseTypes
            => Array.Empty<string>()
                .Concat(Children.OfType<MessageDefinition>().Select(md=>md.Name??"*"))
                .Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .OfType<MessageDefinition>()
                    .Select(ed => ed.Name ?? "*")
                ).Distinct();
        public IEnumerable<string> MessageTypes
             => BaseTypes.DefaultIfEmpty("*");

        public MessageEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Parent is IntermediateThrowEvent)
            {
                var errors = new List<string>();
                if (BaseTypes.Count() > 1)
                    errors.Add("A throw event can only have one message to be thrown.");
                else if (BaseTypes.Any(s => s=="*"))
                    errors.Add("A throw event cannot message with a wildcard message.");
                else if (!BaseTypes.Any(s => s!="*"))
                    errors.Add("A throw must have a message to throw.");
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
                err = (err??Array.Empty<string>()).Concat(errors);
                return res&&!errors.Any();
            }
            return res;
        }
    }
}
