using BPMNEngine.Elements.Processes.Events;

namespace BPMNEngine.Scheduling
{
    internal record ProcessSuspendEvent(ProcessInstance Instance, AEvent Event, DateTime EndTime);
}
