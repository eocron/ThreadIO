using System;
using System.Threading;

namespace ThreadIO
{
    public sealed class LockScope : ILockScope
    {
        private readonly ReaderWriterLockSlim _slim = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        public static readonly TimeSpan Infinity = TimeSpan.FromMilliseconds(-1);

        public bool IsReadHeld => _slim.IsReadLockHeld;

        public bool IsWriteHeld => _slim.IsWriteLockHeld;

        public bool TryGetWriteScope(TimeSpan timeout, out IDisposable scope)
        {
            scope = default(IDisposable);
            if (_slim.TryEnterWriteLock(timeout))
            {
                scope = new WriteScope(_slim);
                return true;
            }
            return false;
        }

        public IDisposable GetWriteScope(TimeSpan? timeout = null)
        {
            IDisposable scope;
            TryGetWriteScope(timeout ?? Infinity, out scope);
            return scope;
        }

        public bool TryGetReadScope(TimeSpan timeout, out IDisposable scope)
        {
            scope = default(IDisposable);
            if (_slim.TryEnterReadLock(timeout))
            {
                scope = new ReadScope(_slim);
                return true;
            }
            return false;
        }

        public IDisposable GetReadScope(TimeSpan? timeout = null)
        {
            IDisposable scope;
            TryGetReadScope(timeout ?? Infinity, out scope);
            return scope;
        }


        private struct ReadScope : IDisposable
        {
            private readonly ReaderWriterLockSlim _slim;

            public ReadScope(ReaderWriterLockSlim slim)
            {
                _slim = slim;
            }

            public void Dispose()
            {
                _slim.ExitReadLock();
            }
        }

        private struct WriteScope : IDisposable
        {
            private readonly ReaderWriterLockSlim _slim;

            public WriteScope(ReaderWriterLockSlim slim)
            {
                _slim = slim;
            }

            public void Dispose()
            {
                _slim.ExitWriteLock();
            }
        }
    }
}