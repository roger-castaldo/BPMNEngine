namespace BPMNEngine.DelegateContainers
{
    /// <summary>
    /// This class is used to house all the Logging delegates for a business process. 
    /// This can be defined at either the BusinessProcess constructor level for defining it 
    /// against all instances or at the BeginProcess level to defining it against a 
    /// specific instance
    /// </summary>
    public class ProcessLogging
    {
        /// <summary>
        /// A delegate called to append a log line entry from the process
        /// </summary>
        public LogLine LogLine { get; init; }
        /// <summary>
        /// A delegate called to append a logged exception from the process
        /// </summary>
        public LogException LogException { get; init; }

        internal static ProcessLogging Merge(ProcessLogging source,ProcessLogging append)
        {
            if (source==null && append==null) return new ProcessLogging() { };
            if (source==null) return append;
            if (append==null) return source;
            return new ProcessLogging()
            {
                LogLine = (source.LogLine!=null&&append.LogLine!=null ?
                                source.LogLine+append.LogLine
                                : (source.LogLine??append.LogLine)),
                LogException = (source.LogException!=null&&append.LogException!=null ?
                                source.LogException+append.LogException
                                : (source.LogException??append.LogException))
            };
        }
    }
}
