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
    [XMLTag("bpmn", "errorEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class ErrorEventDefinition : AParentElement
    {
        public ErrorEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }



        public IEnumerable<string> ErrorTypes => new string[] {}.Concat(Children
                    .Where(elem => elem is ErrorDefinition)
                    .Select(elem => (ErrorDefinition)elem)
                    .Select(ed => ed.Type ?? "*")
                ).Concat(ExtensionElement==null || ((IParentElement)ExtensionElement).Children==null ? Array.Empty<string>() :
                    ((IParentElement)ExtensionElement).Children
                    .Where(elem => elem is ErrorDefinition)
                    .Select(elem => (ErrorDefinition)elem)
                    .Select(ed => ed.Type ?? "*")
                ).Distinct()
                .DefaultIfEmpty("*");

        public override bool IsValid(out string[] err)
        {
            if (ErrorTypes.Any())
            {
                List<string> errors = new List<string>();
                IElement[] elems = Definition.LocateElementsOfType((Parent is IntermediateThrowEvent ? typeof(IntermediateCatchEvent) : typeof(IntermediateThrowEvent)));
                if (Parent is IntermediateThrowEvent)
                {
                    if (ErrorTypes.Count() > 1)
                        errors.Add("A throw event can only have one error to be thrown.");
                    bool found = elems
                        .Select(elem => (IntermediateCatchEvent)elem)
                        .Any(catcher => catcher.Children
                            .Any(child => child is ErrorEventDefinition && ((ErrorEventDefinition)child).ErrorTypes.Contains(ErrorTypes.First()))
                        ) ||
                        elems
                        .Select(elem => (IntermediateCatchEvent)elem)
                        .Any(catcher => catcher.Children
                            .Any(child => child is ErrorEventDefinition && ((ErrorEventDefinition)child).ErrorTypes.Contains("*"))
                        );
                    if (!found)
                        errors.Add("A defined error message type needs to have a Catch Event with a corresponding type or all");
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
