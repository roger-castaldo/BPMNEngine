namespace BPMNEngine
{
    internal enum StepStatuses
    {
        Waiting,
        Succeeded,
        NotRun,
        Failed,
        Suspended,
        Aborted,
        WaitingStart,
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
