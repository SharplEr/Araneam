using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using ArrayHelper;

namespace IOData
{
    public class Results: ICloneable
    {
        Result[] items;

        int maxNumber;

        public int MaxNumber
        {
            get { return maxNumber; }
        }

        public Results(Func<int, Result> f, int n)
        {
            items = new Result[n];

            if (n < 1) throw new ArgumentNullException();

            for (int i = 0; i < n; i++)
                items[i] = f(i);

            maxNumber = items[0].MaxNumber;
        }

        public Result this[int i]
        {
            get { return items[i]; }
        }

        public int Length
        {
            get { return items.Length; }
        }

        public int[] ToNumbers()
        {
            int[] ans = new int[items.Length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = items[i].Number;
            return ans;
        }

        public Vector[] ToSpectrums()
        {
            Vector[] ans = new Vector[items.Length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = items[i].Spectrum;
            return ans;
        }

        public Object Clone()
        {
            return new Results((i) => items[i].CloneOk(), items.Length);
        }

        public Results CloneShuffle(int[] indexer)
        {
            return new Results((i) => items[indexer[i]].CloneOk(), indexer.Length);
        }

        int[] counts;

        /// <summary>
        /// Число примеров принадлежащих разным классам.
        /// </summary>
        public int[] Counts
        {
            get
            {
                if (counts == null)
                {
                    counts = new int[maxNumber];
                    for (int i = 0; i < items.Length; i++)
                        counts[items[i].Number]++;
                }
                return counts;
            }
        }

        double equable = Double.NaN;

        /// <summary>
        /// Отношение числа самого частовстречаемого класса, к самому редковстречаемому.
        /// </summary>
        public double Equable
        {
            get
            {
                if (Double.IsNaN(equable))
                {
                    int max = Counts[0];
                    int min = Counts[0];
                    for (int i = 1; i < Counts.Length; i++)
                    {
                        if (max < Counts[i]) max = Counts[i];
                        if (min > Counts[i]) min = Counts[i];
                    }
                    if (min == 0) equable = Double.PositiveInfinity;
                    else equable = max / min;
                }
                return equable;
            }
        }
    }
}