using BPMNEngine.Elements.Processes.Events;

namespace BPMNEngine.Scheduling
{
    internal record ProcessDelayedEvent(ProcessInstance Instance, BoundaryEvent Event, DateTime StartTime, string SourceID);
}
