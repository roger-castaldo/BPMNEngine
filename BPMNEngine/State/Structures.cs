using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace BPMNEngine.State
{
    internal readonly struct sStepSuspension
    {
        public string Id { get; init; }
        public int StepIndex { get; init; }
        public DateTime EndTime { get; init; }
    }

    internal readonly struct sSuspendedStep
    {
        public string IncomingID { get; init; }
        public string ElementID { get; init; }
    }

    internal readonly struct sDelayedStartEvent
    {
        public string IncomingID {get;init;}

        public string ElementID { get; init; }

        public DateTime StartTime { get; init; }
        public TimeSpan Delay => StartTime.Subtract(DateTime.Now);
    }
}
