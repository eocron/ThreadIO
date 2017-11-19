using System;
using System.Collections.Generic;

namespace ThreadIO
{
    public static class FluentExtensions
    {
        /// <summary>
        /// Adds key to fluent builder for write lock
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static LockScopeBuilder<TKey> Write<TKey>(this LockScopeBuilder<TKey> builder, params TKey[] args)
        {
            if (args == null)
                return builder;
            builder.ToWrite.AddRange(args);
            return builder;
        }

        /// <summary>
        /// Adds key to fluent builder for write lock
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static LockScopeBuilder<TKey> Write<TKey>(this LockScopeBuilder<TKey> builder, IEnumerable<TKey> args)
        {
            if (args == null)
                return builder;
            builder.ToWrite.AddRange(args);
            return builder;
        }

        /// <summary>
        /// Adds key to fluent builder for read lock
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static LockScopeBuilder<TKey> Read<TKey>(this LockScopeBuilder<TKey> builder, params TKey[] args)
        {
            if (args == null)
                return builder;
            builder.ToRead.AddRange(args);
            return builder;
        }

        /// <summary>
        /// Adds key to fluent builder for read lock
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static LockScopeBuilder<TKey> Read<TKey>(this LockScopeBuilder<TKey> builder, IEnumerable<TKey> args)
        {
            if (args == null)
                return builder;
            builder.ToRead.AddRange(args);
            return builder;
        }

        /// <summary>
        /// Adds timeout for lock operation
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="builder"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static LockScopeBuilder<TKey> Timeout<TKey>(this LockScopeBuilder<TKey> builder, TimeSpan timeout)
        {
            builder.WithTimeout = timeout;
            return builder;
        }
    }
}
