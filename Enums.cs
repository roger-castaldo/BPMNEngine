using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    internal enum StepStatuses
    {
        Waiting,
        Succeeded,
        NotRun,
        Failed,
        Suspended
    }

    internal enum VariableTypes
    {
        DateTime,
        Integer,
        Short,
        Long,
        Double,
        Decimal,
        String,
        Char,
        Boolean,
        Float,
        Byte,
        Null,
        File,
        Hashtable
    }
    public enum LogLevels
    {
        None = 0x00,
        Fatal = 0x01,
        Error = 0x02,
        Info = 0x03,
        Debug = 0x04
    }
}
