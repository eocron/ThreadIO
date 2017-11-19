using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ThreadIO
{
    public sealed class LockScopeDictionary<TKey> : ILockScopeDictionary<TKey>
    {
        private readonly Dictionary<TKey, ValueWrap> _index;

        public LockScopeDictionary() : this(null)
        {
        }

        public LockScopeDictionary(IEnumerable<TKey> keys)
        {
            _index = keys?.ToDictionary(x => x, x=> new ValueWrap()) ?? new Dictionary<TKey, ValueWrap>();
        }

        private LockScope GetLockByKey(TKey key)
        {
            return _index[key].Lock;
        }

        public bool IsReadHeld(TKey key)
        {
            return GetLockByKey(key).IsReadHeld;
        }

        public bool IsWriteHeld(TKey key)
        {
            return GetLockByKey(key).IsWriteHeld;
        }

        public bool TryAdd(TKey key)
        {
            if (_index.ContainsKey(key))
            {
                return false;
            }
            _index[key] = new ValueWrap();
            return true;
        }

        public bool TryRemove(TKey key)
        {
            return _index.Remove(key);
        }

        public bool TryGetWriteScope(TKey key, TimeSpan timeout, out IDisposable scope)
        {
            return GetLockByKey(key).TryGetWriteScope(timeout, out scope);
        }

        public LockScopeBuilder<TKey> GetScope()
        {
            return new LockScopeBuilder<TKey>(this);
        }

        public IDisposable GetWriteScope(TKey key, TimeSpan? timeout = null)
        {
            IDisposable scope;
            TryGetWriteScope(key, timeout ?? LockScope.Infinity, out scope);
            return scope;
        }

        public bool TryGetReadScope(TKey key, TimeSpan timeout, out IDisposable scope)
        {
            return GetLockByKey(key).TryGetReadScope(timeout, out scope);
        }

        public IDisposable GetReadScope(TKey key, TimeSpan? timeout = null)
        {
            IDisposable scope;
            TryGetReadScope(key, timeout ?? LockScope.Infinity, out scope);
            return scope;
        }

        public bool TryGetScope(IEnumerable<TKey> toRead, IEnumerable<TKey> toWrite, TimeSpan timeout, out IDisposable scope)
        {
            scope = null;
            var toReadSet = new HashSet<TKey>();
            var toWriteSet = new HashSet<TKey>();
            if (toWrite != null)
            {
                foreach (var key in toWrite)
                {
                    toWriteSet.Add(key);
                }
            }
            if (toRead != null)
            {
                foreach (var key in toRead)
                {
                    if (!toWriteSet.Contains(key))
                    {
                        toReadSet.Add(key);
                    }
                }
            }

            //We sort commands to prevent deadlocks between two separate scope retrieval
            var commandFlow = toReadSet.Select(x => new LockCommand(x, true))
                .Union(toWriteSet.Select(x => new LockCommand(x, false)))
                .OrderBy(x => _index[x.Key].Order)
                .ToList();

            var leftTimeout = timeout;
            var tmp = new ReadWriteScope();
            try
            {
                var sw = new Stopwatch();
                foreach (var cmd in commandFlow)
                {
                    //calculation of left timeout of overall one
                    leftTimeout = GetLeftTimeout(leftTimeout, sw.Elapsed);
                    sw.Restart();
                    IDisposable partScope;
                    if (cmd.IsRead)
                    {
                        if (GetLockByKey(cmd.Key).TryGetReadScope(leftTimeout, out partScope))
                        {
                            tmp.Add(partScope);
                        }
                        else
                        {
                            //failed to make it in time
                            tmp.Dispose();
                            return false;
                        }
                    }
                    else
                    {
                        if (GetLockByKey(cmd.Key).TryGetWriteScope(leftTimeout, out partScope))
                        {
                            tmp.Add(partScope);
                        }
                        else
                        {
                            //failed to make it in time
                            tmp.Dispose();
                            return false;
                        }
                    }
                    sw.Stop();
                }
            }
            catch
            {
                //failed because of some internal error
                tmp.Dispose();
                throw;
            }
            scope = tmp;
            return true;
        }

        public IDisposable GetScope(IEnumerable<TKey> toRead, IEnumerable<TKey> toWrite, TimeSpan? timeout = null)
        {
            IDisposable result;
            TryGetScope(toRead, toWrite, timeout ?? LockScope.Infinity, out result);
            return result;
        }

        private static TimeSpan GetLeftTimeout(TimeSpan timeout, TimeSpan elapsed)
        {
            if (timeout == LockScope.Infinity)
                return timeout;
            var result = timeout - elapsed;
            if (result <= TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }
            return result;
        }

        private class ReadWriteScope : IDisposable
        {
            private readonly Stack<IDisposable> _toDispose = new Stack<IDisposable>();

            public void Add(IDisposable scope)
            {
                _toDispose.Push(scope);
            }

            public void Dispose()
            {
                while (_toDispose.Count > 0)
                {
                    var d = _toDispose.Pop();
                    d.Dispose();
                }
            }
        }

        private struct LockCommand
        {
            public readonly bool IsRead;

            public readonly TKey Key;

            public LockCommand(TKey x, bool v) : this()
            {
                Key = x;
                IsRead = v;
            }
        }

        private class ValueWrap
        {
            private static long _counter = 0;
            public readonly long Order;
            public readonly LockScope Lock;

            public ValueWrap()
            {
                Order = Interlocked.Increment(ref _counter);
                Lock = new LockScope();
            }
        }

        #region Default stubs
        public IEnumerator<TKey> GetEnumerator()
        {
            return _index.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _index.Count;
        #endregion
    }
}
