using System;
using VectorSpace;
using ArrayHelper;

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
        public static double CalcQError(int n, double D, double a)
        {
            return D / (1 - a) / n;
        }
        
        /// <summary>
        /// Гарантированное растояние x-y
        /// </summary>
        /// <param name="x">первая величина</param>
        /// <param name="ex">ошибка первой величины</param>
        /// <param name="y">вторая величина</param>
        /// <param name="ey">ошибка второй величины</param>
        public static double ExactDifference(double x, double ex, double y, double ey)
        {
            //Приравниваем NaN к минус бесконечности.

            bool NinfX = Double.IsNegativeInfinity(x) || Double.IsInfinity(ex) || Double.IsNaN(x);
            bool NinfY = Double.IsNegativeInfinity(y) || Double.IsInfinity(ey) || Double.IsNaN(y);

            if (NinfX)
                if (NinfY) return 0.0; //Будем считать, что бесконечности одного порядка
                else return Double.NegativeInfinity;
            else
                if (NinfY) return Double.PositiveInfinity;

            bool PinfX = Double.IsPositiveInfinity(x);
            bool PinfY = Double.IsPositiveInfinity(y);

            if (PinfX)
                if (PinfY) return 0.0;  //Будем считать, что бесконечности одного порядка
                else return Double.PositiveInfinity;
            else
                if (PinfY) return Double.NegativeInfinity;

            double t = x - y;

            double et = (Double.IsNaN(ex) || Double.IsNaN(ey)) ? 0.0 : Math.Sqrt(ex * ex + ey * ey);
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

        /// <summary>
        /// Перемешивает массив индексов 2-х мерного массива
        /// </summary>
        public static void shuffle2DIndex(Tuple<int,int>[] indexes, Random r)
        {

            int k;

            Tuple<int, int> t;

            for (int i = indexes.Length - 1; i >= 0; i--)
            {
                k = r.Next(i);
                t = indexes[i];
                indexes[i] = indexes[k];
                indexes[k] = t;
            }
        }

        public static double Entropy(int[] counts)
        {
            int length = 0;
            for (int i = 0; i < counts.Length; i++)
                length += counts[i];

            return Entropy(counts, length);
        }

        public static double Entropy(int[] counts, int length)
        {
            double ans = 0.0;
            double p;

            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] == 0) continue;
                p = (double)counts[i] / length;
                ans -= p * Math.Log(p, 2);
            }

            return ans;
        }

        public static double Gain(int[] counts, int length, int[]baseCounts, int baseLength)
        {
            double baseEntropy = Entropy(baseCounts, baseLength);

            double entropy = Entropy(counts, length);

            int[] antiCounts = (int[])baseCounts.Clone();

            for (int i = 0; i < antiCounts.Length; i++)
                antiCounts[i] -= counts[i];

            int antiLength = baseLength-length;

            double antiEntropy = Entropy(antiCounts, antiLength);

            return baseEntropy - (length * entropy + antiLength * antiEntropy) / baseLength;
        }

        public static double GainRatio(int[] counts, int length, int[] baseCounts, int baseLength)
        {
            double baseEntropy = Entropy(baseCounts, baseLength);

            double entropy = Entropy(counts, length);

            int[] antiCounts = (int[])baseCounts.Clone();

            for (int i = 0; i < antiCounts.Length; i++)
                antiCounts[i] -= counts[i];

            int antiLength = baseLength - length;

            double antiEntropy = Entropy(antiCounts, antiLength);

            double gain = baseEntropy - (length * entropy + antiLength * antiEntropy) / baseLength;

            double split = 0;
            double p;

            if (length > 0)
            {
                p = (double)length / baseLength;
                split -= p * Math.Log(p, 2);
            }

            if (antiLength > 0)
            {
                p = (double)antiLength / baseLength;
                split -= p * Math.Log(p, 2);
            }

            return gain/split;
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
        /// Возвращает массив индексов от 0 до заданного числа
        /// </summary>
        /// <param name="length">Длина</param>
        public static int[] GetIndex(int length)
        {
            int[] ans = new int[length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = i;

            return ans;
        }

        public static int[] GetIndexFromTo(int s, int f)
        {
            int[] ans = new int[f-s];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = s+i;

            return ans;
        }

        /// <summary>
        /// Возвращает массив индексов с пропуском
        /// </summary>
        /// <param name="length">Общая длина массива индексов</param>
        /// <param name="exept">Исключаемый индекс</param>
        public static int[] GetIndex(int length, int exept)
        {
            if (exept >= length) return GetIndex(length);

            int[] ans = new int[length];

            for (int i = 0; i < exept; i++)
                ans[i] = i;

            for (int i = exept+1; i <= length; i++)
                ans[i-1] = i;

            return ans;
        }
    }
}