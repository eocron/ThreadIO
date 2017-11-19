using System;
using System.Collections.Generic;

namespace ThreadIO
{
    public static class FluentExtensions
    {
        public static LockScopeBuilder<TKey> Write<TKey>(this LockScopeBuilder<TKey> builder, params TKey[] args)
        {
            if (args == null)
                return builder;
            builder.ToWrite.AddRange(args);
            return builder;
        }

        public static LockScopeBuilder<TKey> Write<TKey>(this LockScopeBuilder<TKey> builder, IEnumerable<TKey> args)
        {
            if (args == null)
                return builder;
            builder.ToWrite.AddRange(args);
            return builder;
        }

        public static LockScopeBuilder<TKey> Read<TKey>(this LockScopeBuilder<TKey> builder, params TKey[] args)
        {
            if (args == null)
                return builder;
            builder.ToRead.AddRange(args);
            return builder;
        }

        public static LockScopeBuilder<TKey> Read<TKey>(this LockScopeBuilder<TKey> builder, IEnumerable<TKey> args)
        {
            if (args == null)
                return builder;
            builder.ToRead.AddRange(args);
            return builder;
        }

        public static LockScopeBuilder<TKey> Timeout<TKey>(this LockScopeBuilder<TKey> builder, TimeSpan timeout)
        {
            builder.WithTimeout = timeout;
            return builder;
        }
    }
}
