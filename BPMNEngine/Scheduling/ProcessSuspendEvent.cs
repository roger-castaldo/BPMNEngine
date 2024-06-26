using BPMNEngine.Elements.Processes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNEngine.Scheduling
{
    internal record ProcessSuspendEvent(ProcessInstance Instance, AEvent Event,DateTime EndTime);
}
