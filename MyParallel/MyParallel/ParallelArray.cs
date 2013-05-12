using System;
using System.Threading;

namespace MyParallel
{
    public static class ParallelArray
    {
        #region неиспользуемая муть
        /*
        /// <summary>
        /// Паралелльно заполняет массив значениями.
        /// Метод работает синхронно по отношению к вызывающему коду
        /// </summary>
        /// <typeparam name="T">тип элемента массива</typeparam>
        /// <param name="vector">массив</param>
        /// <param name="f">метод возвращающий значение по индексу</param>
        public static void set<T>(T[] vector, Func<int, T> f)
        {
            int pc = Environment.ProcessorCount;
            int j = 0;
            int d = vector.Length / pc;
            pc--;
            Thread[] t = new Thread[pc];
            int i;

            for (i = 0; i < pc; i++)
            {
                t[i] = new Thread((start) => 
                {
                    int str = (int)start;
                    for (int k = str; k < str+d; k++) vector[k] = f(k);
                });

                t[i].Start(j);
                j += d;
            }

            for (; j < vector.Length; j++)
                vector[j] = f(j);

            ParallelLambda.join(t);
        }

        /// <summary>
        /// Паралелльно заполняет массив значениями.
        /// Метод работает синхронно по отношению к вызывающему коду
        /// </summary>
        /// <typeparam name="T">Тип элемента массива</typeparam>
        /// <param name="vector">Массив</param>
        /// <param name="f">Метод возвращающий значение по индекс</param>
        /// <param name="pc">Число потоков</param>
        public static void set<T>(T[] vector, Func<int, T> f, int pc)
        {
            int j = 0;
            int d = vector.Length / pc;
            pc--;
            Thread[] t = new Thread[pc];
            int i;

            for (i = 0; i < pc; i++)
            {
                t[i] = new Thread((start) =>
                {
                    int str = (int)start;
                    for (int k = str; k < str + d; k++) vector[k] = f(k);
                });

                t[i].Start(j);
                j += d;
            }

            for (; j < vector.Length; j++)
                vector[j] = f(j);

            ParallelLambda.join(t);
        }
         */ 
        #endregion

        public static T[] Set<T>(this T[] vector, Func<int, T> f, int pc)
        {
            if (vector == null) return null;

            int j = 0;
            int d = vector.Length / pc;
            pc--;
            Thread[] t = new Thread[pc];
            int i;

            for (i = 0; i < pc; i++)
            {
                t[i] = new Thread((start) =>
                {
                    int str = (int)start;
                    for (int k = str; k < str + d; k++) vector[k] = f(k);
                });

                t[i].Start(j);
                j += d;
            }

            for (; j < vector.Length; j++)
                vector[j] = f(j);

            t.join();
            return vector;
        }

        public static T[] SetWithProgress<T>(this T[] vector, Func<int, T> f, Action<int> progress)
        {

            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = f(i);
                progress(100 * i / (vector.Length-1));
            }
            return vector;
        }

        public static U[] Convert<T, U>(this T[] vector, Func<T, U> f)
        {
            U[] ans = new U[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                ans[i] = f(vector[i]);
            return ans;
        }


    }

}
