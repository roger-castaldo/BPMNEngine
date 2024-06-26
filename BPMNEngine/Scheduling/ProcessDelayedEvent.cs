using BPMNEngine.Elements.Processes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNEngine.Scheduling
{
    internal record ProcessDelayedEvent(ProcessInstance Instance,BoundaryEvent Event,DateTime StartTime,string SourceID);
}
