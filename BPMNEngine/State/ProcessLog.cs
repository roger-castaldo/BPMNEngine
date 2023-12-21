using BPMNEngine.Interfaces.State;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace BPMNEngine.State
{
    internal sealed class ProcessLog : IStateContainer
    {
        private class ReadOnlyProcessLog : IReadonlyStateLogContainer
        {
            public ReadOnlyProcessLog(ProcessLog log, int length){
                Log = log.content.ToString()[..length];
            }

            public string Log { get; private init; }

            void IReadOnlyStateContainer.Append(XmlWriter writer)
            {
                writer.WriteCData(Log);
            }


            void IReadOnlyStateContainer.Append(Utf8JsonWriter writer)
            {
                writer.WriteStringValue(Log);
            }
        }

        private readonly StateLock stateLock;
        private readonly StringBuilder content;

        public ProcessLog(StateLock stateLock)
        {
            this.stateLock = stateLock;
            content = new StringBuilder();
        }

        public void LogLine(string elementID, AssemblyName assembly, string fileName, int lineNumber, LogLevel level, DateTime timestamp, string message)
        {
            stateLock.EnterWriteLock();
            content.AppendLine($"{timestamp.ToString(Constants.DATETIME_FORMAT)}|{level}|{assembly.Name}|{fileName}[{lineNumber}]|Element[{elementID}]|{message}");
            stateLock.ExitWriteLock();
        }

        public void LogException(string elementID, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception)
        {
            LogLine(elementID, assembly, fileName, lineNumber, LogLevel.Error, timestamp, GenerateExceptionLine(exception));
        }

        private static string GenerateExceptionLine(Exception exception)
        {
            var sb = new StringBuilder();
            bool isInner = false;
            while (exception != null)
            {
                sb.AppendLine(@$"{(isInner ? "INNER_EXCEPTION:" : "")}MESSAGE:{exception.Message}
STACKTRACE:{exception.StackTrace}");
                isInner = true;
                exception = exception.InnerException;
            }
            return sb.ToString();
        }

        public void Load(XmlReader reader)
        {
            reader.MoveToContent();
            reader.Read();
            content.Clear();
            if (reader.NodeType == XmlNodeType.CDATA)
            {
                content.Append(reader.Value);
                reader.Read();
            }
        }

        public void Load(Utf8JsonReader reader)
        {
            content.Clear();
            reader.Read();
            content.Append(reader.GetString());
            reader.Read();
        }

        public IReadOnlyStateContainer Clone()
        {
            return new ReadOnlyProcessLog(this, content.Length);
        }

        public void Dispose()
        {
            content.Clear();
        }
    }
}
