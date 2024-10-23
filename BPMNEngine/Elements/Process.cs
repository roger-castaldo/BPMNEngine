using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmn", "process")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal record Process : AParentElement, IProcess
    {
        public Process(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public ImmutableArray<StartEvent> StartEvents
            => Children.OfType<StartEvent>().ToImmutableArray();

        public async ValueTask<bool> IsStartValidAsync(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid,ILogger logger)
            => (
                ExtensionElement==null ||
                (await ExtensionElement.Children.OfType<IStepElementStartCheckExtensionElement>().AllAsync<IStepElementStartCheckExtensionElement>(check=>check.IsElementStartValidAsync(variables, this, logger)))
            )
            && isProcessStartValid(this, variables);

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (!Children.Any())
            {
                errors = errors.Append("No child elements found in Process.");
                isValid = false;
            }
            return (isValid, errors);   
        }
    }
}
