using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadIO;

namespace Tests
{
    [TestClass]
    public class ExampleTest
    {
        public interface IBank
        {
            decimal CurrentCash { get; }

            void Withdraw(decimal amount);

            void Deposit(decimal amount);
        }

        public class Bank : IBank
        {
            private readonly List<IBank> _children;
            private decimal _selfCash;

            public decimal CurrentCash
            {
                get { return _selfCash + _children.Sum(x => x.CurrentCash); }
            }

            public Bank(params IBank[] children)
            {
                _children = children?.ToList() ?? new List<IBank>();
            }

            public void Withdraw(decimal amount)
            {
                _selfCash -= amount;
            }

            public void Deposit(decimal amount)
            {
                _selfCash += amount;
            }
        }

        [TestMethod]
        public void Test()
        {
            var graph = new LockScopeGraph<IBank>();

            var bankA = new Bank();
            graph.TryAdd(bankA);

            var bankB = new Bank();
            graph.TryAdd(bankB);

            var bankC = new Bank(bankA, bankB);
            graph.TryAdd(bankC);
            graph.TryAddEdge(bankC, bankA);
            graph.TryAddEdge(bankC, bankB);

            using (graph.GetScope().Read(bankC).ToDisposable())
            {
                var cash = bankC.CurrentCash;
            }
        }
    }
}
