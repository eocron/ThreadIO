using System;
using System.Collections.Generic;

namespace ThreadIO
{
    public interface ILockScopeDictionary<TKey> : IReadOnlyCollection<TKey>
    {
        /// <summary>
        /// Checks if read lock is held for specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsReadHeld(TKey key);

        /// <summary>
        /// Checks if write lock is held for specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsWriteHeld(TKey key);

        IDisposable GetReadScope(TKey key, TimeSpan? timeout = default(TimeSpan?));
        IDisposable GetWriteScope(TKey key, TimeSpan? timeout = default(TimeSpan?));
        bool TryGetReadScope(TKey key, TimeSpan timeout, out IDisposable scope);
        bool TryGetWriteScope(TKey key, TimeSpan timeout, out IDisposable scope);
        bool TryGetScope(IEnumerable<TKey> toRead, IEnumerable<TKey> toWrite, TimeSpan timeout, out IDisposable scope);

        /// <summary>
        /// Retrieves complex lock scope. Duplicates and repetitions allowed.
        /// </summary>
        /// <param name="toRead">keys to read</param>
        /// <param name="toWrite">keys to write</param>
        /// <param name="timeout"></param>
        /// <returns>Indefenetly waits for scope if timeout not specified, returns null if timeout exceeded</returns>
        IDisposable GetScope(IEnumerable<TKey> toRead, IEnumerable<TKey> toWrite, TimeSpan? timeout = default(TimeSpan?));

        /// <summary>
        /// Creates fluent-like builder to chain various scope options in single call.
        /// </summary>
        /// <returns></returns>
        LockScopeBuilder<TKey> GetScope();

        /// <summary>
        /// Tries to add lock scope by specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool TryAdd(TKey key);

        /// <summary>
        /// Tries to remove lock scope by specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool TryRemove(TKey key);
    }
}