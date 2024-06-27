using BPMNEngine.Interfaces.State;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace BPMNEngine.State
{
    internal sealed class ProcessLog(StateLock stateLock) : IStateContainer
    {
        private sealed class ReadOnlyProcessLog(ProcessLog log, int length) : IReadonlyStateLogContainer
        {
            public string Log { get; private init; } = log.content.ToString()[..length];

            void IReadOnlyStateContainer.Append(XmlWriter writer)
                => writer.WriteCData(Log);

            void IReadOnlyStateContainer.Append(Utf8JsonWriter writer)
                => writer.WriteStringValue(Log);
        }

        private readonly StateLock stateLock = stateLock;
        private readonly StringBuilder content = new();

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

        public XmlReader Load(XmlReader reader, Version version)
        {
            reader.MoveToContent();
            reader.Read();
            content.Clear();
            if (reader.NodeType == XmlNodeType.CDATA)
            {
                content.Append(reader.Value);
                reader.Read();
            }
            return reader;
        }

        public Utf8JsonReader Load(Utf8JsonReader reader, Version version)
        {
            content.Clear();
            reader.Read();
            content.Append(reader.GetString());
            return reader;
        }

        public IReadOnlyStateContainer Clone()
            => new ReadOnlyProcessLog(this, content.Length);

        public void Dispose()
            => content.Clear();
    }
}
