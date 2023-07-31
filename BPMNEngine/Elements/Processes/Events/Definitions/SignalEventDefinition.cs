using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions.Extensions;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "signalEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class SignalEventDefinition : AParentElement
    {
        public IEnumerable<string> SignalTypes 
            => Array.Empty<string>()
                .Concat(
                    Children
                    .OfType<SignalDefinition>()
                    .Select(ed => ed.Type ?? "*")
                ).Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .OfType<SignalDefinition>()
                    .Select(ed => ed.Type ?? "*")
                ).Distinct()
                .DefaultIfEmpty("*");

        public SignalEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public override bool IsValid(out string[] err)
        {
            if (SignalTypes.Any())
            {
                var errors = new List<string>();
                if (Parent is IntermediateThrowEvent)
                {
                    if (SignalTypes.Count() > 1)
                        errors.Add("A throw event can only have one error to be thrown.");
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
