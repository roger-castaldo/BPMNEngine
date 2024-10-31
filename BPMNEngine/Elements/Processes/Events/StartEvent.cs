using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTagAttribute("bpmn", "startEvent")]
    internal record StartEvent : AEvent
    {
        public StartEvent(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        internal async ValueTask<bool> IsEventStartValidAsync(IReadonlyVariables variables, IsEventStartValid isEventStartValid, ILogger logger)
            => (
                ExtensionElement==null ||
                (await ExtensionElement.Extensions.OfType<IStepElementStartCheckExtensionElement>().AllAsync(check => check.IsElementStartValidAsync(variables, this, logger)))
            )
            && isEventStartValid(this, variables);

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (Incoming.Any(id => !OwningDefinition.MessageFlows.Any(mf => mf.ID==id)) && !SubType.HasValue)
                errors = errors.Append("Start Events cannot have an incoming path.");
            if (!Outgoing.Any())
                errors = errors.Append("Start Events must have an outgoing path.");
            else if (Outgoing.Count() > 1)
                errors = errors.Append("Start Events can only have 1 outgoing path.");
            return (isValid&&!errors.Any(), errors);
        }
    }
}
