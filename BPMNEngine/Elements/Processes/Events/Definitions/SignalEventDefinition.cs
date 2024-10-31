using BPMNEngine.Attributes;
using BPMNEngine.Extensions.EventDefinitions;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "signalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record SignalEventDefinition : AParentElement, IEventDefinition
    {
        private IEnumerable<string> BaseTypes
            => Array.Empty<string>()
                .Concat(Children.OfType<SignalDefinition>().Select(sd => sd.Type??"*"))
                .Concat(
                    ExtensionElement?.Extensions
                    .OfType<SignalDefinition>()
                    .Select(ed => ed.Type ?? "*")
                    ?? []
                ).Distinct();

        public IEnumerable<string> SignalTypes
            => BaseTypes.DefaultIfEmpty("*");

        public SignalEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public EventSubTypes Type
            => EventSubTypes.Signal;

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (Parent is IntermediateThrowEvent)
            {
                var errs = new List<string>();
                if (BaseTypes.Count() > 1)
                    errs.Add("A throw event can only have one signal to be thrown.");
                else if (BaseTypes.Any(s => s=="*"))
                    errs.Add("A throw event cannot signal with a wildcard signal.");
                else if (!BaseTypes.Any(s => s!="*"))
                    errs.Add("A throw must have a signal to throw.");
                var elems = OwningDefinition.LocateElementsOfType<IntermediateCatchEvent>();
                bool found = elems
                        .Any(catcher => catcher.Children
                        .Any(child => child is SignalEventDefinition definition && definition.SignalTypes.Contains(SignalTypes.First()))
                    ) ||
                    elems
                        .Any(catcher => catcher.Children
                        .Any(child => child is SignalEventDefinition definition && definition.SignalTypes.Contains("*"))
                    );
                if (!found)
                    errs.Add("A defined signal type needs to have a Catch Event with a corresponding type or all");
                isValid &= errs.Count==0;
                errors = errors.Concat(errs);
            }
            return (isValid, errors);
        }
    }
}
