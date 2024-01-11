using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine
{
    public sealed partial class BusinessProcess
    {
        internal void WriteLogLine(string elementID, LogLevel level, StackFrame sf, DateTime timestamp, string message)
            => WriteLogLine((IElement)(elementID == null ? null : GetElement(elementID)), level, sf, timestamp, message);

        internal void WriteLogLine(IElement element, LogLevel level, StackFrame sf, DateTime timestamp, string message)
            => delegates.Logging.LogLine?.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);

        internal Exception WriteLogException(string elementID, StackFrame sf, DateTime timestamp, Exception exception)
            => WriteLogException((IElement)(elementID == null ? null : GetElement(elementID)), sf, timestamp, exception);

        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if (delegates.Logging.LogException != null)
            {
                delegates.Logging.LogException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
                if (exception is InvalidProcessDefinitionException processDefinitionException)
                {
                    processDefinitionException.ProcessExceptions
                        .ForEach(e => { delegates.Logging.LogException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, e); });
                }
            }
            return exception;
        }
    }
}
