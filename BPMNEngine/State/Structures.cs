namespace BPMNEngine.State
{
    internal readonly struct SStepSuspension
    {
        public string ID { get; init; }
        public DateTime EndTime { get; init; }
    }

    internal readonly struct SSuspendedStep
    {
        public string IncomingID { get; init; }
        public string ElementID { get; init; }
    }

    internal readonly struct SDelayedStartEvent
    {
        public string IncomingID { get; init; }

        public string ElementID { get; init; }

        public DateTime StartTime { get; init; }
        public TimeSpan Delay => StartTime.Subtract(DateTime.Now);
    }
}
