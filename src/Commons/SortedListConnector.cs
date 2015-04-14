using System;
using System.Collections.Generic;

namespace Commons
{
    public class SortedListConnector<T1, T2>
    {
        private readonly Func<T1, T2, int> comparator;
        private readonly Queue<Tuple<T1, T2>> crossJoin = new Queue<Tuple<T1, T2>>(4);
        private readonly List<T1> equalList1 = new List<T1>(1);
        private readonly List<T2> equalList2 = new List<T2>(1);
        private readonly List<T1> notProcessed = new List<T1>();
        private readonly List<T1> sortedList1;
        private readonly List<T2> sortedList2;
        private T1 currentItem1;
        private T2 currentItem2;
        private int pos1;
        private int pos2;

        public SortedListConnector(List<T1> sortedList1, List<T2> sortedList2, Func<T1, T2, int> comparator)
        {
            this.sortedList1 = sortedList1;
            this.sortedList2 = sortedList2;
            this.comparator = comparator;
        }

        public List<T1> NotProcessed
        {
            get { return notProcessed; }
        }

        public Tuple<T1, T2> GetNextPair()
        {
            if (crossJoin.Count > 0)
                return crossJoin.Dequeue();
            if (IsEnd())
                return null;

            currentItem1 = sortedList1[pos1];
            currentItem2 = sortedList2[pos2];

            int cmp;
            while ((cmp = comparator(currentItem1, currentItem2)) != 0)
            {
                notProcessed.Add(currentItem1);
                if (!MoveNext(cmp))
                    return null;
                currentItem1 = sortedList1[pos1];
                currentItem2 = sortedList2[pos2];
            }

            FillItemEqualTo(currentItem2);
            FillItemEqualTo(currentItem1);
            foreach (T1 item1 in equalList1)
            {
                foreach (T2 item2 in equalList2)
                    crossJoin.Enqueue(new Tuple<T1, T2>(item1, item2));
            }
            pos1++;
            pos2++;
            return crossJoin.Dequeue();
        }

        private void FillItemEqualTo(T2 item2)
        {
            equalList1.Clear();
            equalList1.Add(sortedList1[pos1]);
            while (pos1 < sortedList1.Count - 1 && comparator(sortedList1[pos1 + 1], item2) == 0)
            {
                pos1++;
                equalList1.Add(sortedList1[pos1]);
            }
        }

        private void FillItemEqualTo(T1 item1)
        {
            equalList2.Clear();
            equalList2.Add(sortedList2[pos2]);
            while (pos2 < sortedList2.Count - 1 && comparator(item1, sortedList2[pos2 + 1]) == 0)
            {
                pos2++;
                equalList2.Add(sortedList2[pos2]);
            }
        }

        private bool IsEnd()
        {
            bool res = pos1 >= sortedList1.Count || pos2 >= sortedList2.Count;
            if (res)
            {
                while (pos1 < sortedList1.Count)
                    notProcessed.Add(sortedList1[pos1++]);
            }
            return res;
        }

        private bool MoveNext(int cmp)
        {
            if (cmp < 0)
                pos1++;
            else if (cmp > 0)
                pos2++;
            return !IsEnd();
        }
    }
}