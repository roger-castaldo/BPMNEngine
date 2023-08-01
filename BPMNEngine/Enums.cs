namespace BPMNEngine
{
    /// <summary>
    /// Enumeration of the statuses used for each step of a process
    /// </summary>
    public enum StepStatuses
    {
        /// <summary>
        /// Waiting for input to complete execution
        /// </summary>
        Waiting,
        /// <summary>
        /// Completed successfully
        /// </summary>
        Succeeded,
        /// <summary>
        /// Step not run (typically used for drawing only and not stored in the state)
        /// </summary>
        NotRun,
        /// <summary>
        /// Failed during execution
        /// </summary>
        Failed,
        /// <summary>
        /// Suspended until a timeout has passed
        /// </summary>
        Suspended,
        /// <summary>
        /// Aborted after being started or Suspended or Waiting on Start
        /// </summary>
        Aborted,
        /// <summary>
        /// Delayed start until a timeout has passed
        /// </summary>
        WaitingStart,
        /// <summary>
        /// Started the satep
        /// </summary>
        Started
    }

    internal enum VariableTypes
    {
        DateTime,
        Integer,
        Short,
        Long,
        UnsignedInteger,
        UnsignedShort,
        UnsignedLong,
        Double,
        Decimal,
        String,
        Char,
        Boolean,
        Float,
        Byte,
        Null,
        File,
        Guid
    }
}
