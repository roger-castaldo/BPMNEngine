using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.TimerDefinition;
using Org.Reddragonit.BpmEngine.Elements.Processes.Scripts;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions
{
    [XMLTag("bpmn", "timerEventDefinition")]
    [ValidParent(typeof(AEvent))]
    internal class TimerEventDefinition : AParentElement
    {
        public TimerEventDefinition(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public TimeSpan? GetTimeout(ProcessVariablesContainer variables) {
            DateTime now = DateTime.Now;
            DateTime? end = null;
            foreach (IElement ie in Children)
            {
                if (ie is XDateString)
                {
                    end = ((XDateString)ie).GetTime(variables);
                    break;
                }else if (ie is AScript)
                {
                    end = (DateTime)((AScript)ie).Invoke(variables);
                    break;
                }
            }
            if (!end.HasValue && this.ExtensionElement != null)
            {
                foreach (IElement ie in ((ExtensionElements)this.ExtensionElement).Children)
                {
                    if (ie is XDateString)
                    {
                        end = ((XDateString)ie).GetTime(variables);
                        break;
                    }
                    else if (ie is AScript)
                    {
                        end = (DateTime)((AScript)ie).Invoke(variables);
                        break;
                    }
                }
            }
            return (end.HasValue ? end.Value.Subtract(now) : (TimeSpan?)null);
        }
    }
}
