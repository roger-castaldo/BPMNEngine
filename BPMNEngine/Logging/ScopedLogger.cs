namespace BPMNEngine.Logging
{
    internal class ScopedLogger(ILogger logger,IDisposable scope) : ILogger, IDisposable
    {
        private bool disposedValue;

        IDisposable? ILogger.BeginScope<TState>(TState state)
            => logger.BeginScope<TState>(state);
        bool ILogger.IsEnabled(LogLevel logLevel)
            => logger.IsEnabled(logLevel);
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            =>logger.Log<TState>(logLevel, eventId, state, exception, formatter);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    scope.Dispose();
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ScopedLogger()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
