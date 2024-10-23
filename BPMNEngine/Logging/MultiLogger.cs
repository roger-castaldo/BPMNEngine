using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Logging
{
    internal class MultiLogger(IEnumerable<ILogger?> loggers) : ILogger
    {
        private const string ElementIDKey = "ElementID";
        private const string InstanceIDKey = "ProcessInstance";

        public static IDisposable? DefineElementLogScope(IElement element, ILogger? logger)
            => DefineElementLogScope(element.ID, logger);

        public static IDisposable? DefineElementLogScope(string elementID, ILogger? logger)
            => logger?.BeginScope<string>($"{ElementIDKey}[{elementID}]");

        public IDisposable BeginElementScope(IElement element)
            => BeginElementScope(element.ID);

        public IDisposable BeginElementScope(string elementID)
            => new DisposableScope(loggers
                    .OfType<ILogger>()
                    .Select(l => DefineElementLogScope(elementID, l)!)
                    .ToArray()
                );

        public IDisposable BeginInstanceScope(ProcessInstance instance, IElement? element = null)
            => BeginInstanceScope(instance, element?.ID);

        public IDisposable BeginInstanceScope(ProcessInstance instance, string? elementID = null)
            => new DisposableScope(loggers
                .OfType<ILogger>()
                .Select(l =>
                {
                    if (!string.IsNullOrWhiteSpace(elementID))
                    {
                        if (l is ProcessLog)
                            return DefineElementLogScope(elementID, l);
                        else
                            return l.BeginScope<string>($"{InstanceIDKey}[{instance.ID}]|{ElementIDKey}[{elementID}]");
                    }else if (l is not ProcessLog)
                        return l.BeginScope<string>($"{InstanceIDKey}[{instance.ID}]");
                    return null;
                }).OfType<IDisposable>()
                .ToArray()
            );

        IDisposable ILogger.BeginScope<TState>(TState state)
            => new DisposableScope(loggers.OfType<ILogger>().Select(l => l.BeginScope(state)!).ToArray());
        bool ILogger.IsEnabled(LogLevel logLevel)
            => loggers.Any(l => l?.IsEnabled(logLevel)??false);
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => loggers.Where(l => l?.IsEnabled(logLevel)??false).ForEach(logger => logger?.Log(logLevel, eventId, state, exception, formatter));

        private sealed class DisposableScope(IDisposable[] scopes)
            : IDisposable
        {
            private bool disposedValue;

            void IDisposable.Dispose()
            {
                if (!disposedValue)
                {
                    scopes.Where(s=>s!=null).ForEach(s => s.Dispose());
                    disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }
        }
    }
}
