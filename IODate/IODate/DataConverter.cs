using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOData
{
    public static class DataConverter
    {
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
            return (x) => sorted.IndexOf(x);
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
    }
}
