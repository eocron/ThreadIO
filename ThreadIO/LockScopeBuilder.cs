using System;
using System.Collections.Generic;

namespace ThreadIO
{
    public sealed class LockScopeBuilder<TKey>
    {
        private readonly ILockScopeDictionary<TKey> _dict;

        internal readonly List<TKey> ToRead;

        internal readonly List<TKey> ToWrite;

        internal TimeSpan WithTimeout { get; set; }

        internal LockScopeBuilder(ILockScopeDictionary<TKey> dict)
        {
            _dict = dict;
            ToRead = new List<TKey>();
            ToWrite = new List<TKey>();
            WithTimeout = LockScope.Infinity;
        }

        public IDisposable ToDisposable()
        {
            return _dict.GetScope(ToRead, ToWrite, WithTimeout);
        }

        public bool TryDisposable(out IDisposable scope)
        {
            return _dict.TryGetScope(ToRead, ToWrite, WithTimeout, out scope);
        }
    }
}
