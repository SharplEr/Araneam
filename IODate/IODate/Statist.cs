using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;

namespace IODate
{
    public static class Statist
    {
        static Random r = new Random();

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

        /// <summary>
        /// Возвращает массив случайных индексов от 0 до n-1
        /// </summary>
        public static int[] getRandomIndex(int n)
        {
            int[] ans = new int[n];

            int i, k, t;
            for (i = 0; i < n; i++)
                ans[i] = i;

            for (i = n - 1; i >= 0; i--)
            {
                k = r.Next(i);
                t = ans[i];
                ans[i] = ans[k];
                ans[k] = t;
            }

            return ans;
        }

        static int[] ReservoirSampling(int k, int n)
        {
            int[] ans = new int[k];
            for (int i = 0; i < k; i++)
                ans[i] = i;

            int j;
            for (int i = k; i < n; i++)
            {
                j = r.Next(i);
                if (j < k) ans[j] = i;
            }

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
        public static Action<Vector> Normalization(this Vector[] matrix, double c)
        {
            if (matrix == null) throw new ArgumentNullException();
            if (matrix.Length == 0) throw new ArgumentNullException("Длина ноль");

            int n = matrix[0].Length;
            Vector maxs = new Vector(n).Set(matrix[0]);
            Vector mins = new Vector(n).Set(matrix[0]);
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
                    if (mins[j] > x) mins[j] = x;
                }
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double t = maxs[j] - mins[j];
                    if (t == 0) matrix[i][j] = c;
                    else matrix[i][j] = 2.0 * (matrix[i][j] - maxs[j]) / t + 1;
                }
            }

            return (v) =>
            {
                for (int i = 0; i < n; i++)
                {
                    double t = maxs[i] - mins[i];
                    if (t == 0) v[i] = c;
                    else v[i] = 2.0 * (v[i] - maxs[i]) / t + 1;
                }
            };
        }
    }
}
