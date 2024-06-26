using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions.Extensions;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "errorEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record ErrorEventDefinition : AParentElement, IEventDefinition
    {
        private IEnumerable<string> BaseTypes
            => Array.Empty<string>()
                .Concat(Children.OfType<ErrorDefinition>().Select(ed => ed.Type??"*"))
                .Concat(ExtensionElement?.Children
                    .OfType<ErrorDefinition>()
                    .Select(ed => ed.Type ?? "*")
                    ?? []
                ).Distinct();
        public IEnumerable<string> ErrorTypes
            => BaseTypes.DefaultIfEmpty("*");

        public EventSubTypes Type 
            => EventSubTypes.Error;

        public ErrorEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (Parent is IntermediateThrowEvent)
            {
                var errors = new List<string>();
                if (BaseTypes.Count() > 1)
                    errors.Add("A throw event can only have one error to be thrown.");
                else if (BaseTypes.Any(s => s=="*"))
                    errors.Add("A throw event cannot error with a wildcard error.");
                else if (!BaseTypes.Any(s => s!="*"))
                    errors.Add("A throw must have a error to throw.");
                var elems = OwningDefinition.LocateElementsOfType<IntermediateCatchEvent>();
                bool found = elems
                    .Any(catcher => catcher.Children
                        .Any(child => child is ErrorEventDefinition definition && definition.ErrorTypes.Contains(ErrorTypes.First()))
                    ) ||
                    elems
                    .Any(catcher => catcher.Children
                        .Any(child => child is ErrorEventDefinition definition && definition.ErrorTypes.Contains("*"))
                    );
                if (!found)
                    errors.Add("A defined error message type needs to have a Catch Event with a corresponding type or all");
                err = (err?? []).Concat(errors);
                return res && errors.Count==0;
            }
            return res;
        }
    }
}
