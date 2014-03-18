using System;
using VectorSpace;

namespace IOData
{
    /// <summary>
    /// Набор статистических функций
    /// </summary>
    public static class Statist
    {
        /// <summary>
        /// Поиск отклонения
        /// </summary>
        /// <param name="n">Число испытаний</param>
        /// <param name="D">Дисперсия</param>
        /// <param name="a">доверительная вероятность</param>
        /// <returns></returns>
        public static double CalcError(int n, double D, double a)
        {
            return Math.Sqrt(D / (1 - a) / n);
        }

        /// <summary>
        /// Поиск квадрата отклонения
        /// </summary>
        /// <param name="n">Число испытаний</param>
        /// <param name="D">Дисперсия</param>
        /// <param name="a">доверительная вероятность</param>
        /// <returns></returns>
        public static double CalcQError(int n, double D, double a)
        {
            return D / (1 - a) / n;
        }

        public static double ExactDifference(double x, double ex, double y, double ey)
        {
            double t = x - y;
            double et = Math.Sqrt(ex * ex + ey * ey);
            double ans = Math.Abs(t) - et;
            if (ans < 0) return 0.0;
            return ans * Math.Sign(t);
        }

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
        public static int[] getRandomIndex(int n, Random r)
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

        public static int[] FisherShuffle(int k, int n, Random r)
        {
            int[] ans = new int[n];
            int i, j, t;
            for (i = 0; i < n; i++)
                ans[i] = i;

            for (i = n - 1; i >= n - k; i--)
            {
                j = r.Next(i);
                t = ans[i];
                ans[i] = ans[j];
                ans[j] = t;
            }

            return ans;
        }

        public static int[] ReservoirSampling(int k, int n, Random r)
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

        public static double Avg(double[] x)
        {
            double ans = 0.0;
            for (int i = 0; i < x.Length; i++)
                ans += x[i];
            return ans / x.Length;
        }

        public static double Correlation(double[] x, double[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException();
            double ax = 0.0;
            double ay = 0.0;

            for(int i =0; i<x.Length; i++)
            {
                ax += x[i];
                ay += y[i];
            }

            ax /= x.Length;
            ay /= y.Length;

            double xy = 0;
            double qx = 0;
            double qy = 0;
            double tx;
            double ty;
            for (int i = 0; i < x.Length; i++)
            {
                tx = (x[i] - ax);
                ty = (y[i] - ay);
                xy += tx*ty;
                qx+=tx*tx;
                qy += ty*ty;
            }

            return xy/Math.Sqrt(qx*qy);
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

        /// <summary>
        /// Инициализация массива объектов
        /// </summary>
        public static void done<T>(this T[] arr) where T: new()
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = new T();
        }
    }
}
