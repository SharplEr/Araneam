using System;
using System.Collections.Generic;
using System.Linq;
using ArrayHelper;

namespace IOData
{
    /// <summary>
    /// Набор статических методов генерирующие преобразователи входных значений
    /// </summary>
    public static class DataConverter
    {
        public static Func<string, int> NumericOfString(string[] input, out int max)
        {
            if (input == null) throw new NullReferenceException();
            List<string> strs = new List<string>();

            for (int i = 0; i < input.Length; i++)
                if (!strs.Contains(input[i]))
                    strs.Add(input[i]);
            
            max = strs.Count;

            return (x) => { return strs.IndexOf(x); };
        }

        public static Func<string, double> DiscreteToСontinuous(string[] input, double[] output)
        {
            Dictionary<string, double> dict = new Dictionary<string, double>();
            Dictionary<string, int> counts = new Dictionary<string, int>();
            int i;
            for (i = 0; i < input.Length; i++)
            {
                if (counts.ContainsKey(input[i]))
                {
                    counts[input[i]]++;
                    dict[input[i]] += output[i];
                }
                else
                {
                    counts.Add(input[i], 1);
                    dict.Add(input[i], output[i]);
                }
            }

            string[] v = dict.Keys.ToArray();
            for (int j = 0; j < v.Length; j++)
            {
                dict[v[j]] /= counts[v[j]];
            }

            List<string> sorted = (from kv in dict
                                   orderby kv.Value
                                   select kv.Key).ToList();

            double e = Double.MaxValue;
            int count = 0;
            int n = 0;
            for (i = 1; i < sorted.Count; i++)
            {
                double t = dict[sorted[i]] - dict[sorted[i - 1]];
                if (t > 0)
                {
                    if (count > 0)
                    {
                        if (count > n) n = count;
                        count = 0;
                    }
                    if (t < e) e = t;
                }
                else
                {
                    count++;
                }
            }
            if (count > n) n = count;

            e = e / (1.1*n);

            count = 0;

            for (i = 1; i < sorted.Count; i++)
            {
                double t = dict[sorted[i]] - dict[sorted[i - 1]];
                if (t > 0)
                {
                    count = 0;
                }
                else
                {
                    count++;
                    dict[sorted[i]] -= count * e;
                }
            }

            return (x) => dict[x];
        }

        public static Func<int, double> DiscreteToСontinuous(int[] input, Results output)
        {
            int max = input[0];
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] > max) max = input[i];
            }

            int[] counts = new int[max];
            Dictionary<int, double> dict = new Dictionary<int, double>();

            for(int i = 0; i<max; i++)
                dict.Add(i, 0);

            int t;
            int maxR = output[0].MaxNumber;
            for (int i = 0; i < input.Length; i++)
            {
                t = input[i];
                counts[t]++;
                dict[t] += 2.0 * (output[t].Number - (maxR - 1.0) / 2.0) / (maxR - 1);
            }

            List<int> sorted = (from kv in dict
                                   orderby kv.Value
                                   select kv.Key).ToList();

            double e = Double.MaxValue;
            int count = 0;
            int n = 0;
            for (int i = 1; i < sorted.Count; i++)
            {
                double tt = dict[sorted[i]] - dict[sorted[i - 1]];
                if (tt > 0)
                {
                    if (count > 0)
                    {
                        if (count > n) n = count;
                        count = 0;
                    }
                    if (tt < e) e = tt;
                }
                else
                {
                    count++;
                }
            }
            if (count > n) n = count;

            e = e / (1.1 * n);

            count = 0;

            for (int i = 1; i < sorted.Count; i++)
            {
                double tt = dict[sorted[i]] - dict[sorted[i - 1]];
                if (tt > 0)
                {
                    count = 0;
                }
                else
                {
                    count++;
                    dict[sorted[i]] -= count * e;
                }
            }

            double[] ans = new double[max];
            for (int i = 0; i < max; i++)
                ans[i] = dict[i];

            return (x) => ans[x];
        }

        public static Func<int, double> DiscreteToСontinuous(ArrayShall<int> input, Results output)
        {
            int max = input[0];
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] > max) max = input[i];
            }
            max++;
            int[] counts = new int[max];
            Dictionary<int, double> dict = new Dictionary<int, double>();

            for (int i = 0; i < max; i++)
                dict.Add(i, 0);

            int t;
            int maxR = output[0].MaxNumber;
            for (int i = 0; i < input.Length; i++)
            {
                t = input[i];
                counts[t]++;
                dict[t] += 2.0 * (output[i].Number - (maxR - 1) / 2.0) / (maxR - 1);
            }

            for (int i = 0; i < counts.Length; i++)
            {
                dict[i] /= counts[i];
            }

            List<int> sorted = (from kv in dict
                                orderby kv.Value
                                select kv.Key).ToList();

            double e = Double.MaxValue;
            int count = 0;
            int n = 0;
            for (int i = 1; i < sorted.Count; i++)
            {
                double tt = dict[sorted[i]] - dict[sorted[i - 1]];
                if (tt > 0)
                {
                    if (count > 0)
                    {
                        if (count > n) n = count;
                        count = 0;
                    }
                    if (tt < e) e = tt;
                }
                else
                {
                    count++;
                }
            }
            if (count > n) n = count;

            e = e / (1.1 * n);

            count = 0;

            for (int i = 1; i < sorted.Count; i++)
            {
                double tt = dict[sorted[i]] - dict[sorted[i - 1]];
                if (tt > 0)
                {
                    count = 0;
                }
                else
                {
                    count++;
                    dict[sorted[i]] -= count * e;
                }
            }

            double[] ans = new double[max];
            for (int i = 0; i < max; i++)
                ans[i] = dict[i];

            return (x) => ans[x];
        }

        public static Func<double, int> СontinuousToDiscrete(double[] input, int min)
        {
            input = (double[])input.Clone();
            Array.Sort(input);
            if (input.Length < min) return (x) => Array.IndexOf(input, x);
            double[] groups = new double[input.Length / min];

            int k = input.Length % min;

            for (int i = 0; i < k; i++)
            {
                groups[i] = input[min + i * (min + 1)];
            }
            int m = 2 * min + (k - 1) * (min + 1);
            for (int i = k; i < groups.Length; i++)
            {
                groups[i] = input[m + (i - k) * min];
            }

            return (x) =>
                {
                    int j = 0;
                    while (j < groups.Length)
                    {
                        if (x > groups[j]) j++;
                        else
                        {
                            return j;
                        }
                    }
                    return groups.Length - 1;
                };
        }

        public static Func<double, int> СontinuousToDiscrete(ArrayShall<double> inputas, int min)
        {
            double[] input = inputas.ToArray();
            Array.Sort(input);
            if (input.Length < min) return (x) => Array.IndexOf(input, x);
            double[] groups = new double[input.Length / min];

            int k = input.Length % min;

            for (int i = 0; i < k; i++)
            {
                groups[i] = input[min + i * (min + 1)];
            }
            int m = 2 * min + (k - 1) * (min + 1);
            for (int i = k; i < groups.Length; i++)
            {
                groups[i] = input[m + (i - k) * min];
            }

            return (x) =>
            {
                int j = 0;
                while (j < groups.Length)
                {
                    if (x > groups[j]) j++;
                    else
                    {
                        return j;
                    }
                }
                return groups.Length - 1;
            };
        }

    }
}