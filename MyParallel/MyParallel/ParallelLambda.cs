﻿using System;
using System.Threading;

namespace MyParallel
{
    public static class ParallelLambda
    {
        /// <summary>
        /// Паралелльно выполняет группы методов
        /// </summary>
        /// <param name="fs">Массив методов</param>
        public static Action[] act(this Action[] fs)
        {
            Thread[] t = new Thread[fs.Length];

            for (int i = 0; i < fs.Length; i++)
                t[i] = new Thread(() => fs[i]());

            t.join();
            return fs;
        }

        /// <summary>
        /// Ожидает группу потоков
        /// </summary>
        /// <param name="t">Массив потоков</param>
        public static void join(this Thread[] t)
        {
            for (int i = 0; i < t.Length; i++)
                t[i].Join();
        }

        /// <summary>
        /// Запускает поток в многопоточном апартаменте
        /// </summary>
        public static void InMTA(this Thread t)
        {
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
            t.Join();
        }

        public static void InNThread(this ParameterizedThreadStart f, int n)
        {
            Thread[] ts = new Thread[n];
            for (int i = 0; i < n; i++)
            {
                ts[i] = new Thread(f);
                ts[i].Start(i);
            }
            ts.join();
        }

        public static T Casing<T>(object o, Func<T> f)
        {
            T r;
            lock(o)
            {
                r = f();
            }
            return r;
        }

        public static void Casing(object o, Action f)
        {
            lock (o)
            {
                f();
            }
        }

    }
}
