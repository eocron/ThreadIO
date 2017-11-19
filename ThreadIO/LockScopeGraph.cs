using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ThreadIO
{
    public sealed class LockScopeGraph<TKey> : ILockScopeGraph<TKey>
    {
        private readonly LockScopeDictionary<TKey> _dict;
        private readonly Graph<TKey> _nodes;

        public LockScopeGraph()
        {
            _dict = new LockScopeDictionary<TKey>();
            _nodes = new Graph<TKey>();
        }

        public LockScopeGraph(IEnumerable<TKey> keys)
        {
            var enumerable = keys as TKey[] ?? keys?.ToArray() ?? new TKey[0];
            _dict = new LockScopeDictionary<TKey>(enumerable);
            _nodes = new Graph<TKey>(enumerable);
        }

        public bool TryAddEdge(TKey from, TKey to)
        {
            return _nodes.TryAddEdge(from, to);
        }

        public bool TryRemoveEdge(TKey from, TKey to)
        {
            return _nodes.TryRemoveEdge(from, to);
        }

        public IDisposable GetReadScope(TKey key, TimeSpan? timeout = null)
        {
            IDisposable result;
            TryGetReadScope(key, timeout ?? LockScope.Infinity, out result);
            return result;
        }

        public IDisposable GetScope(IEnumerable<TKey> toRead, IEnumerable<TKey> toWrite, TimeSpan? timeout = null)
        {
            IDisposable result;
            TryGetScope(toRead, toWrite, timeout ?? LockScope.Infinity, out result);
            return result;
        }

        public LockScopeBuilder<TKey> GetScope()
        {
            return new LockScopeBuilder<TKey>(this);
        }

        public IDisposable GetWriteScope(TKey key, TimeSpan? timeout = null)
        {
            IDisposable result;
            TryGetWriteScope(key, timeout ?? LockScope.Infinity, out result);
            return result;
        }

        public bool IsReadHeld(TKey key)
        {
            return _dict.IsReadHeld(key);
        }

        public bool IsWriteHeld(TKey key)
        {
            return _dict.IsWriteHeld(key);
        }

        public bool TryAdd(TKey key)
        {
            if (_dict.TryAdd(key))
            {
                _nodes.TryAddNode(key);
                return true;
            }
            return false;
        }

        public bool TryGetReadScope(TKey key, TimeSpan timeout, out IDisposable scope)
        {
            return TryGetScope(new[] { key }, null, timeout, out scope);
        }

        public bool TryGetScope(IEnumerable<TKey> toRead, IEnumerable<TKey> toWrite, TimeSpan timeout, out IDisposable scope)
        {
            //instead of using initial r/w keys, we find sub-graphs by them and use its keys to lock on
            var toWrite2 = _nodes.GetAllAscendants(toWrite).ToList();
            var toRead2 = _nodes.GetAllDescendants(toRead?.Union(toWrite2) ?? toWrite2).ToList();
            return _dict.TryGetScope(toRead2, toWrite2, timeout, out scope);
        }

        public bool TryGetWriteScope(TKey key, TimeSpan timeout, out IDisposable scope)
        {
            return TryGetScope(null, new[] {key}, timeout, out scope);
        }

        public bool TryRemove(TKey key)
        {
            if (_dict.TryRemove(key))
            {
                _nodes.TryRemoveNode(key);
                return true;
            }
            return false;
        }

        #region Default stubs
        public IEnumerator<TKey> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dict.Count;
        #endregion
    }
}
