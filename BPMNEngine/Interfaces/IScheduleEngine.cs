using BPMNEngine.Elements.Processes.Events;

namespace BPMNEngine.Interfaces
{
    internal interface IScheduleEngine : IDisposable
    {
        void Sleep(TimeSpan value, ProcessInstance process, AEvent evnt);
        void DelayStart(TimeSpan value, ProcessInstance process, BoundaryEvent evnt, string sourceID);
        void AbortDelayedEvent(ProcessInstance process, BoundaryEvent evnt, string sourceID);
        void AbortSuspendedElement(ProcessInstance process, string id);
        void UnloadProcess(BusinessProcess process);
        void UnloadProcess(ProcessInstance process);
    }
}
