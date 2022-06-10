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
        Suspended,
        Aborted,
        WaitingStart
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
        Hashtable,
        Guid
    }
    /// <summary>
    /// This enumartor is used to specify the level of logging a process will log within its state.  It is also used to specify the log level messages recieved through the log line delegate are applied to.
    /// </summary>
    public enum LogLevels
    {
        /// <summary>
        /// No Logging
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Fatal messages only
        /// </summary>
        Fatal = 0x01,
        /// <summary>
        /// Error and Fatal messages only
        /// </summary>
        Error = 0x02,
        /// <summary>
        /// Informational, Error and Fatal messages
        /// </summary>
        Info = 0x03,
        /// <summary>
        /// Debug messages, this will log the most information and will log all other levels.
        /// </summary>
        Debug = 0x04
    }
    /// <summary>
    /// This enumartor is used to specify the format of the diagram image to output
    /// </summary>
    public enum ImageOuputTypes
    {
        /// <summary>
        /// JPEG
        /// </summary>
        Jpeg,
        /// <summary>
        /// PNG
        /// </summary>
        Png
    }
}
