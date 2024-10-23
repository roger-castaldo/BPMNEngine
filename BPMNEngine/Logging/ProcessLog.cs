using BPMNEngine.Interfaces.State;
using BPMNEngine.State;
using System.Text;
using System.Text.Json;

namespace BPMNEngine.Logging
{
    internal sealed class ProcessLog(StateLock stateLock,LogLevel InstanceLogLevel) : IStateContainer,ILogger
    {
        private sealed class ReadOnlyProcessLog(ProcessLog log, int length) : IReadonlyStateLogContainer
        {
            public string Log { get; private init; } = log.content.ToString()[..length];

            void IReadOnlyStateContainer.Append(XmlWriter writer)
                => writer.WriteCData(Log);

            void IReadOnlyStateContainer.Append(Utf8JsonWriter writer)
                => writer.WriteStringValue(Log);
        }

        private readonly StringBuilder content = new();
        private readonly AsyncLocal<LogScope?> scope = new();

        #region ILogger
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            var preScope = scope.Value;
            scope.Value = new(state, () =>
            {
                scope.Value=preScope;
            });
            return scope.Value;
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
            => (int)logLevel >= (int)InstanceLogLevel;
        
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!((ILogger)this).IsEnabled(logLevel))
                return;

            var logEntry = formatter(state, exception);
            if (!string.IsNullOrEmpty(logEntry))
            {
                stateLock.EnterWriteLock();
                content.AppendLine($"{DateTime.Now.ToString(Constants.DATETIME_FORMAT)}|{logLevel}|{scope.Value?.Value.ToString()}|{logEntry}");
                stateLock.ExitWriteLock();
            }
        }
        #endregion

        #region IStateContainer
        XmlReader IStateContainer.Load(XmlReader reader, Version version)
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

        Utf8JsonReader IStateContainer.Load(Utf8JsonReader reader, Version version)
        {
            content.Clear();
            reader.Read();
            content.Append(reader.GetString());
            return reader;
        }

        IReadOnlyStateContainer IStateContainer.Clone()
            => new ReadOnlyProcessLog(this, content.Length);
        #endregion

        public void Dispose()
            => content.Clear();
    }
}
