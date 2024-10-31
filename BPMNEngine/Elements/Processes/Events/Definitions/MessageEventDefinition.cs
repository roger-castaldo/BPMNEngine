using BPMNEngine.Attributes;
using BPMNEngine.Extensions.EventDefinitions;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "messageEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record MessageEventDefinition : AParentElement, IEventDefinition
    {
        private IEnumerable<string> BaseTypes
            => Array.Empty<string>()
                .Concat(Children.OfType<MessageDefinition>().Select(md => md.Name??"*"))
                .Concat(
                    ExtensionElement?.Extensions
                    .OfType<MessageDefinition>()
                    .Select(ed => ed.Name ?? "*")
                    ?? []
                ).Distinct();
        public IEnumerable<string> MessageTypes
             => BaseTypes.DefaultIfEmpty("*");

        public MessageEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public EventSubTypes Type => EventSubTypes.Message;

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (Parent is IntermediateThrowEvent)
            {
                var errs = new List<string>();
                if (BaseTypes.Count() > 1)
                    errs.Add("A throw event can only have one message to be thrown.");
                else if (BaseTypes.Any(s => s=="*"))
                    errs.Add("A throw event cannot message with a wildcard message.");
                else if (!BaseTypes.Any(s => s!="*"))
                    errs.Add("A throw must have a message to throw.");
                var elems = OwningDefinition.LocateElementsOfType<IntermediateCatchEvent>();
                bool found = elems
                    .Any(catcher => catcher.Children
                        .Any(child => child is MessageEventDefinition definition && definition.MessageTypes.Contains(MessageTypes.First()))
                    ) ||
                    elems
                    .Any(catcher => catcher.Children
                        .Any(child => child is MessageEventDefinition definition && definition.MessageTypes.Contains("*"))
                    );
                if (!found)
                    errs.Add("A defined message needs to have a Catch Event with a corresponding type or all");
                isValid &= errs.Count==0;
                errors = errors.Concat(errs);
            }
            return (isValid, errors);
        }
    }
}
