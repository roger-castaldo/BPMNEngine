using BPMNEngine.Interfaces.State;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;

namespace BPMNEngine.State
{
    internal sealed class ProcessLog : IStateContainer
    {
        private class ReadOnlyProcessLog : IReadOnlyStateContainer
        {
            private readonly ProcessLog _log;
            private readonly int _length;

            public ReadOnlyProcessLog(ProcessLog log, int length)
            {
                _log=log;
                _length=length;
            }

            private string Content
            {
                get
                {
                    _log._stateLock.EnterReadLock();
                    var result = _log._content.ToString()[.._length];
                    _log._stateLock.ExitReadLock();
                    return result;
                }
            }

            void IReadOnlyStateContainer.Append(XmlWriter writer)
            {
                writer.WriteCData(Content);
            }


            void IReadOnlyStateContainer.Append(Utf8JsonWriter writer)
            {
                writer.WriteStringValue(Content);
            }
        }

        private readonly ReaderWriterLockSlim _stateLock;
        private readonly StringBuilder _content;

        public ProcessLog(ReaderWriterLockSlim stateLock)
        {
            _stateLock = stateLock;
            _content = new StringBuilder();
        }

        public void LogLine(string elementID, AssemblyName assembly, string fileName, int lineNumber, LogLevel level, DateTime timestamp, string message)
        {
            _stateLock.EnterWriteLock();
            _content.AppendFormat("{0}|{1}|{2}|{3}[{4}]|Element[{5}]|{6}\r\n", new object[]
                {
                timestamp.ToString(Constants.DATETIME_FORMAT),
                level,
                assembly.Name,
                fileName,
                lineNumber,
                elementID,
                message
                }
            );
            _stateLock.ExitWriteLock();
        }

        public void LogException(string elementID, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception)
        {
            LogLine(elementID, assembly, fileName, lineNumber, LogLevel.Error, timestamp, GenerateExceptionLine(exception));
            if (exception is InvalidProcessDefinitionException processDefinitionException)
                processDefinitionException.ProcessExceptions.ForEach(e => LogLine(elementID, assembly, fileName, lineNumber, LogLevel.Error, timestamp, GenerateExceptionLine(e)));
        }

        private static string GenerateExceptionLine(Exception exception)
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
                isInner ? "INNER_EXCEPTION:" : ""
            }));
                isInner = true;
                exception = exception.InnerException;
            }
            return sb.ToString();
        }

        public void Load(XmlReader reader)
        {
            reader.MoveToContent();
            reader.Read();
            _content.Clear();
            if (reader.NodeType == XmlNodeType.CDATA)
            {
                _content.Append(reader.Value);
                reader.Read();
            }
        }

        public void Load(Utf8JsonReader reader)
        {
            _content.Clear();
            _content.Append(reader.GetString());
            reader.Read();
        }

        public IReadOnlyStateContainer Clone()
        {
            return new ReadOnlyProcessLog(this, _content.Length);
        }

        public void Dispose()
        {
            _content.Clear();
        }
    }
}
