using System;
using System.Threading;
using System.Collections.Concurrent;

namespace MyParallel
{
    public static class ParallelArray
    {
        /// <summary>
        /// Функция единичной паралельной обработки одномерного массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector">Обрабатываемый массив</param>
        /// <param name="f">Функция обработчик по индексу</param>
        /// <param name="pc">Число потоков</param>
        /// <returns></returns>
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
    }

}
