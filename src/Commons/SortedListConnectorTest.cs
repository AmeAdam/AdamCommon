using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Commons
{
    [TestFixture]
    public class SortedListConnectorTest
    {
        private static void CheckPair(Tuple<A, B> pair, string expectedA, string expectedB)
        {
            Assert.IsNotNull(pair);
            Assert.IsNotNull(pair.Item1);
            Assert.IsNotNull(pair.Item2);
            Assert.AreEqual(expectedA, pair.Item1.Value);
            Assert.AreEqual(expectedB, pair.Item2.Value);
        }

        [Test]
        public void EmptyList()
        {
            var aList = new List<A>();
            var bList = new List<B>
                {
                    new B {Key = 1, Value = "B1"},
                    new B {Key = 1, Value = "B2"},
                    new B {Key = 2, Value = "B3"},
                    new B {Key = 2, Value = "B4"},
                };

            var sorter1 = new SortedListConnector<A, B>(aList, bList, (a, b) => a.Key.CompareTo(b.Key));
            Assert.IsNull(sorter1.GetNextPair());

            var sorter2 = new SortedListConnector<B, A>(bList, aList, (a, b) => a.Key.CompareTo(b.Key));
            Assert.IsNull(sorter2.GetNextPair());
            CollectionAssert.AreEquivalent(bList, sorter2.NotProcessed);

            bList = new List<B>();
            sorter2 = new SortedListConnector<B, A>(bList, aList, (a, b) => a.Key.CompareTo(b.Key));
            Assert.IsNull(sorter2.GetNextPair());
        }

        [Test]
        public void ManyToMany()
        {
            var aList = new List<A>
                {
                    new A {Key = 1, Value = "A1"},
                    new A {Key = 2, Value = "A2"},
                    new A {Key = 2, Value = "A3"},
                    new A {Key = 3, Value = "A4"},
                };

            var bList = new List<B>
                {
                    new B {Key = 1, Value = "B1"},
                    new B {Key = 1, Value = "B2"},
                    new B {Key = 2, Value = "B3"},
                    new B {Key = 2, Value = "B4"},
                };

            var sorter = new SortedListConnector<A, B>(aList, bList, (a, b) => a.Key.CompareTo(b.Key));
            CheckPair(sorter.GetNextPair(), "A1", "B1");
            CheckPair(sorter.GetNextPair(), "A1", "B2");
            CheckPair(sorter.GetNextPair(), "A2", "B3");
            CheckPair(sorter.GetNextPair(), "A2", "B4");
            CheckPair(sorter.GetNextPair(), "A3", "B3");
            CheckPair(sorter.GetNextPair(), "A3", "B4");
            Assert.IsNull(sorter.GetNextPair());
            CollectionAssert.AreEquivalent(new[] {aList[3]}, sorter.NotProcessed);
        }

        [Test]
        public void ManyToOne()
        {
            var aList = new List<A>
                {
                    new A {Key = 1, Value = "A1"},
                    new A {Key = 2, Value = "A2"},
                    new A {Key = 2, Value = "A3"},
                    new A {Key = 3, Value = "A4"},
                    new A {Key = 4, Value = "A5"},
                };

            var bList = new List<B>
                {
                    new B {Key = 1, Value = "B1"},
                    new B {Key = 2, Value = "B2"},
                    new B {Key = 3, Value = "B3"},
                };

            var sorter = new SortedListConnector<A, B>(aList, bList, (a, b) => a.Key.CompareTo(b.Key));
            CheckPair(sorter.GetNextPair(), "A1", "B1");
            CheckPair(sorter.GetNextPair(), "A2", "B2");
            CheckPair(sorter.GetNextPair(), "A3", "B2");
            CheckPair(sorter.GetNextPair(), "A4", "B3");
            Assert.IsNull(sorter.GetNextPair());
            CollectionAssert.AreEquivalent(new[] {aList[4]}, sorter.NotProcessed);
        }

        [Test]
        public void OneToMany()
        {
            var aList = new List<A>
                {
                    new A {Key = 0, Value = "A0"},
                    new A {Key = 1, Value = "A1"},
                    new A {Key = 2, Value = "A2"},
                    new A {Key = 3, Value = "A3"},
                };

            var bList = new List<B>
                {
                    new B {Key = 1, Value = "B1"},
                    new B {Key = 2, Value = "B2"},
                    new B {Key = 2, Value = "B3"},
                    new B {Key = 4, Value = "B4"},
                };

            var sorter = new SortedListConnector<A, B>(aList, bList, (a, b) => a.Key.CompareTo(b.Key));
            CheckPair(sorter.GetNextPair(), "A1", "B1");
            CheckPair(sorter.GetNextPair(), "A2", "B2");
            CheckPair(sorter.GetNextPair(), "A2", "B3");
            Assert.IsNull(sorter.GetNextPair());
            CollectionAssert.AreEquivalent(new[] {aList[0], aList[3]}, sorter.NotProcessed);
        }

        [Test]
        public void OneToOne()
        {
            var aList = new List<A>
                {
                    new A {Key = 1, Value = "A1"},
                    new A {Key = 2, Value = "A2"},
                    new A {Key = 3, Value = "A3"},
                };

            var bList = new List<B>
                {
                    new B {Key = 1, Value = "B1"},
                    new B {Key = 2, Value = "B2"},
                    new B {Key = 3, Value = "B3"},
                };

            var sorter = new SortedListConnector<A, B>(aList, bList, (a, b) => a.Key.CompareTo(b.Key));
            CheckPair(sorter.GetNextPair(), "A1", "B1");
            CheckPair(sorter.GetNextPair(), "A2", "B2");
            CheckPair(sorter.GetNextPair(), "A3", "B3");
            Assert.IsNull(sorter.GetNextPair());
            CollectionAssert.IsEmpty(sorter.NotProcessed);
        }
    }

    internal class A
    {
        public int Key;
        public string Value;

        public override string ToString()
        {
            return Key + " " + Value;
        }
    }

    internal class B
    {
        public int Key;
        public string Value;

        public override string ToString()
        {
            return Key + " " + Value;
        }
    }
}