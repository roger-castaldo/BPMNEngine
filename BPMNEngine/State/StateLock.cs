using System.Threading;

namespace BPMNEngine.State
{
    internal class StateLock : IDisposable
    {
        private static readonly TimeSpan READ_TIMEOUT = TimeSpan.FromSeconds(2.5);
        private static readonly TimeSpan WRITE_TIMEOUT = TimeSpan.FromSeconds(5);
        private readonly ReaderWriterLockSlim stateEvent;
        private readonly Guid? stateID;
        private bool disposedValue;

        public int CurrentReadCount => stateEvent.CurrentReadCount;
        public int WaitingWriteCount => stateEvent.WaitingWriteCount;

        public StateLock(Guid? stateID)
        {
            this.stateID = stateID;
            stateEvent  = new(LockRecursionPolicy.NoRecursion);
        }

        internal void EnterReadLock()
        {
            if (disposedValue) throw new ObjectDisposedException(nameof(stateEvent));
            if (!stateEvent.TryEnterReadLock(READ_TIMEOUT))
                throw new StateLockTimeoutException(stateID,CurrentReadCount,WaitingWriteCount);
        }

        internal void ExitReadLock()
        {
            if (disposedValue) throw new ObjectDisposedException(nameof(stateEvent));
            stateEvent.ExitReadLock();
        }

        internal void EnterWriteLock()
        {
            if (disposedValue) throw new ObjectDisposedException(nameof(stateEvent));
            if (!stateEvent.TryEnterWriteLock(WRITE_TIMEOUT))
                throw new StateLockTimeoutException(stateID, CurrentReadCount, WaitingWriteCount);
        }

        internal void ExitWriteLock() { 
            if (disposedValue) throw new ObjectDisposedException(nameof(stateEvent));
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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StateLock()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
