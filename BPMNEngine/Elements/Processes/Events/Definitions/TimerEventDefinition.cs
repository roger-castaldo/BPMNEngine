using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions.TimerDefinition;
using BPMNEngine.Elements.Processes.Scripts;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "timerEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class TimerEventDefinition : AParentElement,IEventDefinition
    {
        public TimerEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public EventSubTypes Type => EventSubTypes.Timer;

        public TimeSpan? GetTimeout(IReadonlyVariables variables) {
            DateTime now = DateTime.Now;
            DateTime? end = null;
            var dtValue = Children.FirstOrDefault(ie => ie is XDateString || ie is AScript);
            if (dtValue != null)
                end = (dtValue is XDateString @string ? @string.GetTime(variables) : (DateTime)((AScript)dtValue).Invoke(variables));
            if (!end.HasValue && this.ExtensionElement != null && ((ExtensionElements)ExtensionElement).Children!=null)
            {
                dtValue = ((ExtensionElements)ExtensionElement).Children.FirstOrDefault(ie => ie is XDateString || ie is AScript);
                if (dtValue != null)
                    end = dtValue is XDateString @string ? @string.GetTime(variables) : (DateTime)((AScript)dtValue).Invoke(variables);
            }
            return (end.HasValue ? end.Value.Subtract(now) : (TimeSpan?)null);
        }
    }
}
