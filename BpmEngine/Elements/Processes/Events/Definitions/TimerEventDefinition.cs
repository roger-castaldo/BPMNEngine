using BpmEngine.Attributes;
using BpmEngine.Elements.Processes.Events.Definitions.TimerDefinition;
using BpmEngine.Elements.Processes.Scripts;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "timerEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class TimerEventDefinition : AParentElement
    {
        public TimerEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public TimeSpan? GetTimeout(IReadonlyVariables variables) {
            DateTime now = DateTime.Now;
            DateTime? end = null;
            var dtValue = Children.FirstOrDefault(ie => ie is XDateString || ie is AScript);
            if (dtValue != null)
                end = (dtValue is XDateString ? ((XDateString)dtValue).GetTime(variables) : (DateTime)((AScript)dtValue).Invoke(variables));
            if (!end.HasValue && this.ExtensionElement != null && ((ExtensionElements)ExtensionElement).Children!=null)
            {
                dtValue = ((ExtensionElements)ExtensionElement).Children.FirstOrDefault(ie => ie is XDateString || ie is AScript);
                if (dtValue != null)
                    end = (dtValue is XDateString ? ((XDateString)dtValue).GetTime(variables) : (DateTime)((AScript)dtValue).Invoke(variables));
            }
            return (end.HasValue ? end.Value.Subtract(now) : (TimeSpan?)null);
        }
    }
}
