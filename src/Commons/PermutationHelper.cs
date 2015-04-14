using System.Collections.Generic;

namespace Commons
{
    public class PermutationHelper<T>
    {
        private readonly List<int> indexes;
        private readonly int size;
        private readonly List<List<T>> values;

        public PermutationHelper(List<List<T>> values)
        {
            this.values = values;
            indexes = this.values.ConvertAll(l => 0);
            size = indexes.Count;
        }

        public void Reset()
        {
            for (int i = 0; i < size; i++)
                indexes[i] = 0;
        }

        public List<T> GetNext()
        {
            if (!IsCorrect())
                return null;
            var res = new List<T>(size);
            for (int i = 0; i < size; i++)
                res.Add(values[i][indexes[i]]);
            MoveNext();
            return res;
        }

        private void MoveNext()
        {
            for (int i = 0; i < size; i++)
            {
                indexes[i] += 1;
                if (indexes[i] < values[i].Count)
                    return;
                if (i != size - 1)
                    indexes[i] = 0;
            }
        }

        private bool IsCorrect()
        {
            return indexes[size - 1] < values[size - 1].Count;
        }
    }
}