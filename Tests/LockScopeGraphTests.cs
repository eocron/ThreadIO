using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadIO;

namespace Tests
{
    [TestClass]
    public class LockScopeGraphTests
    {
        [TestMethod]
        public void Test1()
        {
            var g = new LockScopeGraph<string>();
            g.TryAdd("A");
            g.TryAdd("B");

            using (g.GetWriteScope("A"))
            {
                g.AssertWrite("A");
                g.AssertOff("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");
        }

        [TestMethod]
        public void Test2()
        {
            var g = new LockScopeGraph<string>();
            g.TryAdd("A");
            g.TryAdd("B");
            g.TryAddEdge("A", "B");
            using (g.GetWriteScope("A"))
            {
                g.AssertWrite("A");
                g.AssertRead("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");

            using (g.GetReadScope("A"))
            {
                g.AssertRead("A");
                g.AssertRead("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");

            using (g.GetReadScope("B"))
            {
                g.AssertOff("A");
                g.AssertRead("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");
        }

        [TestMethod]
        public void Test3()
        {
            var g = new LockScopeGraph<string>();
            g.TryAdd("A");
            g.TryAdd("B");
            g.TryAddEdge("A", "B");
            g.TryAddEdge("B", "A");

            using (g.GetReadScope("A"))
            {
                g.AssertRead("A");
                g.AssertRead("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");

            using (g.GetWriteScope("A"))
            {
                g.AssertWrite("A");
                g.AssertWrite("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");

            using (g.GetReadScope("B"))
            {
                g.AssertRead("A");
                g.AssertRead("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");

            using (g.GetWriteScope("B"))
            {
                g.AssertWrite("A");
                g.AssertWrite("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");
        }


        [TestMethod]
        public void Test4()
        {
            var g = new LockScopeGraph<string>();
            g.TryAdd("A");
            g.TryAddEdge("A", "A");

            using (g.GetScope().Read("A").ToDisposable())
            {
                g.AssertRead("A");
            }
            g.AssertOff("A");

            using (g.GetWriteScope("A"))
            {
                g.AssertWrite("A");
            }
            g.AssertOff("A");
        }

        [TestMethod]
        public void Test5()
        {
            var g = new LockScopeGraph<string>();
            g.TryAdd("A");
            g.TryAdd("B");
            g.TryAddEdge("A", "B");
            using (g.GetScope().Write("A").ToDisposable())
            {
                g.AssertWrite("A");
                g.AssertRead("B");
                g.AssertGetRead("B");
                g.AssertNotGetRead("A");
                g.AssertNotGetWrite("B");
                g.AssertNotGetWrite("A","B");
                g.AssertNotGetRead("A","B");
            }
            g.AssertOff("A");
            g.AssertOff("B");
        }

        [TestMethod]
        public void Test6()
        {
            var g = new LockScopeGraph<string>();
            g.TryAdd("A");
            g.TryAdd("B");
            g.TryAddEdge("A", "B");
            using (g.GetScope().ToDisposable())
            {
                g.AssertOff("A");
                g.AssertOff("B");
            }
            g.AssertOff("A");
            g.AssertOff("B");
        }


        [TestMethod]
        public void Test7()
        {
            var g = new LockScopeGraph<string>();
            using (g.GetScope().ToDisposable())
            {
            }
        }


        [TestMethod]
        public void Test8()
        {
            var g = new LockScopeGraph<string>();
            using (g.GetScope(null, null, null))
            {
            }
        }
    }
}
