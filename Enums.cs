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
        Failed
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
        Null
    }
}
