namespace BPMNEngine.State
{
    internal class StateLock(Guid? stateID) : IDisposable
    {
        private static readonly TimeSpan READ_TIMEOUT = TimeSpan.FromSeconds(2.5);
        private static readonly TimeSpan WRITE_TIMEOUT = TimeSpan.FromSeconds(5);
        private readonly ReaderWriterLockSlim stateEvent = new(LockRecursionPolicy.NoRecursion);
        private bool disposedValue;

        public int CurrentReadCount => stateEvent.CurrentReadCount;
        public int WaitingWriteCount => stateEvent.WaitingWriteCount;

        internal void EnterReadLock()
        {
            ObjectDisposedException.ThrowIf(disposedValue, stateEvent);
            if (!stateEvent.TryEnterReadLock(READ_TIMEOUT))
                throw new StateLockTimeoutException(stateID, CurrentReadCount, WaitingWriteCount);
        }

        internal void ExitReadLock()
        {
            ObjectDisposedException.ThrowIf(disposedValue, stateEvent);
            stateEvent.ExitReadLock();
        }

        internal void EnterWriteLock()
        {
            ObjectDisposedException.ThrowIf(disposedValue, stateEvent);
            if (!stateEvent.TryEnterWriteLock(WRITE_TIMEOUT))
                throw new StateLockTimeoutException(stateID, CurrentReadCount, WaitingWriteCount);
        }

        internal void ExitWriteLock()
        {
            ObjectDisposedException.ThrowIf(disposedValue, stateEvent);
            stateEvent.ExitWriteLock();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stateEvent.Dispose();
                }
                disposedValue=true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
