using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.State
{
    internal sealed class LogContainer : AStateContainer
    {
        private const string _PROCESS_LOG_ELEMENT = "ProcessLog";
        protected override string _ContainerName
        {
            get { return _PROCESS_LOG_ELEMENT; }
        }

        public LogContainer(ProcessState state)
            : base(state) { }

        public void LogLine(string elementID, AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message)
        {
            AppendValue(string.Format("{0}|{1}|{2}|{3}[{4}]|Element[{5}]|{6}\r\n", new object[]
                {
                    timestamp.ToString(Constants.DATETIME_FORMAT),
                    level,
                    assembly.Name,
                    fileName,
                    lineNumber,
                    elementID,
                    message
                }));
        }

        public void LogException(string elementID, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception)
        {
            LogLine(elementID, assembly, fileName, lineNumber, LogLevels.Error, timestamp, _GenerateExceptionLine(exception));
            if (exception is InvalidProcessDefinitionException processDefinitionException)
            {
                foreach (Exception e in processDefinitionException.ProcessExceptions)
                    LogLine(elementID, assembly, fileName, lineNumber, LogLevels.Error, timestamp, _GenerateExceptionLine(e));
            }
        }

        private string _GenerateExceptionLine(Exception exception)
        {
            var sb = new StringBuilder();
            bool isInner = false;
            while (exception != null)
            {
                sb.AppendLine(string.Format(@"{2}MESSAGE:{0}
STACKTRACE:{1}", new object[]
            {
                exception.Message,
                exception.StackTrace,
                (isInner ? "INNER_EXCEPTION:" : "")
            }));
                isInner = true;
                exception = exception.InnerException;
            }
            return sb.ToString();
        }
    }
}
