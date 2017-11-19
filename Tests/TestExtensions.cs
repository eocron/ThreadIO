using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadIO;

namespace Tests
{
    public static class TestExtensions
    {
        public static void AssertOff<T>(this ILockScopeDictionary<T> dict, T key)
        {
            Assert.IsFalse(dict.IsWriteHeld(key));
            Assert.IsFalse(dict.IsReadHeld(key));
        }

        public static void AssertWrite<T>(this ILockScopeDictionary<T> dict, T key)
        {
            Assert.IsTrue(dict.IsWriteHeld(key));
            Assert.IsFalse(dict.IsReadHeld(key));
        }

        public static void AssertRead<T>(this ILockScopeDictionary<T> dict, T key)
        {
            Assert.IsFalse(dict.IsWriteHeld(key));
            Assert.IsTrue(dict.IsReadHeld(key));
        }

        public static void AssertGetRead<T>(this ILockScopeDictionary<T> dict, params T[] keys)
        {
            InvokeInThread(() =>
            {
                IDisposable scope;
                if (!dict.TryGetScope(keys, null, TimeSpan.FromMilliseconds(0), out scope))
                {
                    Assert.Fail("Timeout get read");
                }
                scope.Dispose();
            });
        }

        public static void AssertNotGetRead<T>(this ILockScopeDictionary<T> dict,params T[] keys)
        {
            InvokeInThread(() =>
            {
                IDisposable scope;
                if (dict.TryGetScope(keys, null, TimeSpan.FromMilliseconds(0), out scope))
                {
                    scope.Dispose();
                    Assert.Fail("Read was get");
                }
            });
        }

        public static void AssertGetWrite<T>(this ILockScopeDictionary<T> dict, params T[] keys)
        {
            InvokeInThread(() =>
            {
                IDisposable scope;
                if (!dict.TryGetScope(null, keys, TimeSpan.FromMilliseconds(0), out scope))
                {
                    Assert.Fail("Timeout get write");
                }
                scope.Dispose();
            });
        }

        public static void AssertNotGetWrite<T>(this ILockScopeDictionary<T> dict, params T[] keys)
        {
            InvokeInThread(() =>
            {
                IDisposable scope;
                if (dict.TryGetScope(null, keys, TimeSpan.FromMilliseconds(0), out scope))
                {
                    scope.Dispose();
                    Assert.Fail("Write was get");
                }
            });
        }

        public static void InvokeInThread(Action action)
        {
            Exception ex = null;
            var t = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    ex = e;
                }
            });
            t.Start();
            t.Join();
            if (ex != null)
                throw ex;
        }
    }
}
