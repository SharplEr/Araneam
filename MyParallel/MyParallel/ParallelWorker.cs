﻿using System;
using System.Threading;
using ArrayHelper;

namespace MyParallel
{
    public abstract class ParallelWorker : IDisposable
    {
        readonly Thread[] Workers;
        readonly protected int n;

        readonly int d;
        readonly int tc;

        protected object go = new object();
        protected AutoResetEvent[] ready;
        protected AutoResetEvent[] pause;
        protected bool exit = false;
        protected bool deadEnd = false;

        /// <summary>
        /// Конструктор связывает обработчик с массивом
        /// </summary>
        /// <param name="threadCount">Число потоков обработчика</param>
        /// <param name="v">Обрабатываемый массив</param>
        protected ParallelWorker(int threadCount, int nn)
        {
            n = nn;
            tc = threadCount;
            if (threadCount > 1)
            {
                Workers = new Thread[threadCount];
                ready = new AutoResetEvent[threadCount];
                pause = new AutoResetEvent[threadCount];

                d = n / threadCount;

                for (int i = 0; i < threadCount; i++)
                {
                    ready[i] = new AutoResetEvent(false);
                    pause[i] = new AutoResetEvent(false);
                    Workers[i] = new Thread(DoOne);
                    Workers[i].Start(i);
                }
            }
        }
        static int fullCount = 0;
        static readonly object naming = new object();
        /// <summary>
        /// Конструктор связывает обработчик с массивом и дает потокам уникальные имена
        /// </summary>
        /// <param name="threadCount">Число потоков обработчика</param>
        /// <param name="v">Обрабатываемый массив</param>
        /// <param name="name">Начало имени для потоков</param>
        protected ParallelWorker(int threadCount, int nn, string name)
        {
            n = nn;
            tc = threadCount;
            if (threadCount > 1)
            {
                Workers = new Thread[threadCount];
                ready = new AutoResetEvent[threadCount];
                pause = new AutoResetEvent[threadCount];

                d = n / threadCount;

                for (int i = 0; i < threadCount; i++)
                {
                    ready[i] = new AutoResetEvent(false);
                    pause[i] = new AutoResetEvent(false);
                    Workers[i] = new Thread(DoOne);
                    lock (naming)
                    {
                        fullCount++;
                        Workers[i].Name = name + fullCount.ToString();
                    }
                    Workers[i].Start(i);
                }
            }
        }

        /// <summary>
        /// Запуск потоков
        /// </summary>
        [MTAThread]
        protected void Run()
        {
            if (deadEnd) throw new Exception("Обработчик уже освобожден");
            if (tc > 1)
            {
                WaitHandle.WaitAll(pause);

                lock (go) Monitor.PulseAll(go);

                WaitHandle.WaitAll(ready);
            }
            else
            {
                DoFromTo(0, n);
            }
        }

        /// <summary>
        /// Обработчик части массива
        /// </summary>
        protected void DoOne(object nm)
        {
            int num = (int)nm;
            int mod = n % Workers.Length;
            int str;
            int end;
            if (num < mod)
            {
                str = (d + 1) * num;
                end = str + d + 1;
            }
            else
            {
                str = (d + 1) * mod + d * (num - mod);
                end = str + d;
            }
            AutoResetEvent myReady = ready[num];
            while (true)
            {
                //Ожидание
                lock (go)
                {
                    pause[num].Set();
                    Monitor.Wait(go);
                }

                //Завершение потока
                if (exit)
                {
                    myReady.Set();
                    return;
                }

                DoFromTo(str, end);

                //Сигнализация о завершении
                myReady.Set();
            }
        }

        /// <summary>
        /// Обработка части массива
        /// </summary>
        /// <param name="start">С</param>
        /// <param name="finish">До</param>
        abstract protected void DoFromTo(int start, int finish);

        /// <summary>
        /// Освобождение ресурсов после использования и завершение всех потоков
        /// </summary>
        public void Dispose()
        {
            if (deadEnd) return;
            if (tc > 1)
            {
                exit = true;
                Run();
                ready.Let(x => x.Dispose());
                pause.Let(x => x.Dispose());
            }
            deadEnd = true;
        }
    }
}
