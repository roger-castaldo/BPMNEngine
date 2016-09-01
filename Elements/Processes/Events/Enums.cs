using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    internal enum EventSubTypes
    {
        Message,
        Timer,
        Escalation,
        Conditional,
        Link,
        Compensation,
        Signal
    }
}
