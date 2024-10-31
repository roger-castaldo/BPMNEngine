using BPMNEngine.Attributes;
using BPMNEngine.Extensions;
using BPMNEngine.Extensions.Scripts;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTagAttribute("bpmn", "timerEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal record TimerEventDefinition : AParentElement, IEventDefinition
    {
        private readonly XDateString? dateString;
        private readonly AScript? script;

        public TimerEventDefinition(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent,elementFactory) {
            dateString = (XDateString?)SubNodes.OfType<XmlElement>()
                .Where(e => elementFactory.IsOfType<XDateString>(e))
                .Select(e => elementFactory.ProduceExtensionElement(e, this))
                .FirstOrDefault();
            script = (AScript?)SubNodes.OfType<XmlElement>()
                .Where(e => elementFactory.IsOfType<AScript>(e))
                .Select(e => elementFactory.ProduceExtensionElement(e, this))
                .FirstOrDefault();
        }

        public EventSubTypes Type
            => EventSubTypes.Timer;

        public TimeSpan? GetTimeout(IReadonlyVariables variables, ILogger? logger)
        {
            DateTime now = DateTime.Now;
            DateTime? end = dateString?.GetTime(variables)??(DateTime?)script?.Invoke(variables,logger);
            if (end==null && this.ExtensionElement != null && ExtensionElement.Extensions.Length!=0)
            {
                IExtensionElement? dtValue = ExtensionElement.Extensions.FirstOrDefault(ie => ie is XDateString || ie is AScript);
                if (dtValue != null)
                    end = dtValue is XDateString @string ? @string.GetTime(variables) : (DateTime)((AScript)dtValue).Invoke(variables, logger);
            }
            return (end.HasValue ? end.Value.Subtract(now) : (TimeSpan?)null);
        }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
            => dateString?.IsValid(logger) ?? script?.IsValid(logger) ?? base.IsValid(logger);
    }
}
