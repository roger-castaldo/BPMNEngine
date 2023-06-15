using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Elements.Processes.Events
{
    internal enum EventSubTypes
    {
        Message,
        Timer,
        Escalation,
        Conditional,
        Link,
        Compensation,
        Signal,
        Error,
        Terminate
    }
}
