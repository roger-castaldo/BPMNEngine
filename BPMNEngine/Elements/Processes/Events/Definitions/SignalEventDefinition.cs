using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions.Extensions;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "signalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class SignalEventDefinition : AParentElement, IEventDefinition
    {
        private IEnumerable<string> BaseTypes
            => Array.Empty<string>()
                .Concat(Children.OfType<SignalDefinition>().Select(sd => sd.Type??"*"))
                .Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .OfType<SignalDefinition>()
                    .Select(ed => ed.Type ?? "*")
                ).Distinct();

        public IEnumerable<string> SignalTypes 
            => BaseTypes.DefaultIfEmpty("*");

        public SignalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Signal;

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Parent is IntermediateThrowEvent)
            {
                var errors = new List<string>();
                if (BaseTypes.Count() > 1)
                    errors.Add("A throw event can only have one signal to be thrown.");
                else if (BaseTypes.Any(s => s=="*"))
                    errors.Add("A throw event cannot signal with a wildcard signal.");
                else if (!BaseTypes.Any(s => s!="*"))
                    errors.Add("A throw must have a signal to throw.");
                var elems = Definition.LocateElementsOfType<IntermediateCatchEvent>();
                bool found = elems
                        .Any(catcher => catcher.Children
                        .Any(child => child is SignalEventDefinition definition && definition.SignalTypes.Contains(SignalTypes.First()))
                    ) ||
                    elems
                        .Any(catcher => catcher.Children
                        .Any(child => child is SignalEventDefinition definition && definition.SignalTypes.Contains("*"))
                    );
                if (!found)
                    errors.Add("A defined signal type needs to have a Catch Event with a corresponding type or all");
                err = (err??Array.Empty<string>()).Concat(errors);
                return res && !errors.Any();
            }
            return res;
        }
    }
}
