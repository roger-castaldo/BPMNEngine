using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions.Extensions;
using BPMNEngine.Interfaces.Elements;
using System.Linq;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "errorEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class ErrorEventDefinition : AParentElement
    {
        private IEnumerable<string> BaseTypes
            => Array.Empty<string>()
                .Concat(Children.OfType<ErrorDefinition>().Select(ed => ed.Type??"*"))
                .Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .OfType<ErrorDefinition>()
                    .Select(ed => ed.Type ?? "*")
                ).Distinct();
        public IEnumerable<string> ErrorTypes
            => BaseTypes.DefaultIfEmpty("*");

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
                var elems = Definition.LocateElementsOfType<IntermediateCatchEvent>();
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
                err = (err??Array.Empty<string>()).Concat(errors);
                return res && !errors.Any();
            }
            return res;
        }
    }
}
