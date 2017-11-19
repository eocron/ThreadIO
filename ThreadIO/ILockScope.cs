using System;

namespace ThreadIO
{
    public interface ILockScope
    {
        bool IsReadHeld { get; }
        bool IsWriteHeld { get; }

        IDisposable GetReadScope(TimeSpan? timeout = default(TimeSpan?));
        IDisposable GetWriteScope(TimeSpan? timeout = default(TimeSpan?));
        bool TryGetReadScope(TimeSpan timeout, out IDisposable scope);
        bool TryGetWriteScope(TimeSpan timeout, out IDisposable scope);
    }
}