using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;

namespace NeuralNetwork
{
    public static class Statist
    {
        /// <summary>
        /// Способов угадать m бит из n
        /// </summary>
        /// <param name="m">сколько надо угадать</param>
        /// <param name="n">из скольки надо угадать</param>
        /// <returns>Число способов</returns>
        public static int Guess(int m, int n)
        {
            if (m > n) throw new ArgumentException();
            if (n == m) return 1;
            else if (m == 1) return (1 >> n) - 1;
            else return Guess(m - 1, n - 1) + Guess(m, n - 1);
        }

        public static int[] getRandomIndex(int n)
        {
            int[] ans = new int[n];
            List<int> index = new List<int>();
            Random r = new Random();
            int i, k;
            for (i = 0; i < n; i++) index.Add(i);

            for (i = 0; i < (n - 1); i++)
            {
                k = r.Next(index.Count - 1);
                ans[i] = index[k];
                index.RemoveAt(k);
            }
            ans[n - 1] = index[0];
            return ans;
        }

        public static int Power(int a, int b)
        {
            int re = 1;
            while (b != 0)
            {
                if (b % 2 == 1) re *= a;
                a *= a;
                b >>= 1;
            }
            return re;
        }

        /// <summary>
        /// Нормализует по эллементно массив векторов и возвращает функцию нормализации
        /// </summary>
        public static Action<Vector> Normalization(this Vector[] matrix)
        {
            if (matrix == null) throw new ArgumentNullException();
            if (matrix.Length == 0) throw new ArgumentNullException("Длина ноль");

            int n = matrix[0].Length;
            Vector maxs = new Vector(n).Set(matrix[0]);
            Vector avgs = new Vector(n).Set(matrix[0]);
            Vector vec;
            double x;
            for (int i = 1; i < matrix.Length; i++)
            {
                vec = matrix[i];
                if (vec.Length != n) throw new ArgumentException("Длина всех векторов должа быть одинаковой");
                for (int j = 0; j < n; j++)
                {
                    x = vec[j];
                    if (maxs[j] < x) maxs[j] = x;
                    avgs[j] += x;
                }
            }

            avgs.Multiplication(1.0 / matrix.Length);

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double t = maxs[j] - avgs[j];
                    if (t == 0) matrix[i][j] = 0;
                    else matrix[i][j] = (matrix[i][j] - avgs[j]) / t;
                }
            }

            return (v) =>
                {
                    for (int i = 0; i < n; i++)
                    {
                        double t = maxs[i] - avgs[i];
                        if (t == 0) v[i] = 0;
                        else v[i] = (v[i] - avgs[i]) / t;
                    }
                };
        }
    }
}
