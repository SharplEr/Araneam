using System;
using System.Threading;
using ArrayHelper;

namespace MyParallel
{
    public abstract class ParallelWorkerWithProgress : IDisposable
    {
        private readonly Thread[] Workers;
        protected readonly int n;

        private readonly int d;
        private readonly int tc;

        protected object go = new object();
        protected AutoResetEvent[] ready;
        protected AutoResetEvent[] pause;
        protected bool exit = false;
        protected bool deadEnd = false;
        protected Timer timer = null;

        protected double[] progress;
        protected Action<double> f;

        /// <summary>
        /// Конструктор связывает обработчик с массивом
        /// </summary>
        /// <param name="threadCount">Число потоков обработчика</param>
        /// <param name="v">Обрабатываемый массив</param>
        protected ParallelWorkerWithProgress(int threadCount, int nn, Action<double> ff)
        {
            n = nn;
            tc = threadCount;
            progress = new double[threadCount];
            f = ff;
            if (threadCount > 1)
            {
                Workers = new Thread[threadCount];
                ready = new AutoResetEvent[threadCount];
                pause = new AutoResetEvent[threadCount];

                d = n/threadCount;

                for (int i = 0; i < threadCount; i++)
                {
                    ready[i] = new AutoResetEvent(false);
                    pause[i] = new AutoResetEvent(false);
                    Workers[i] = new Thread(DoOne);
                    Workers[i].Start(i);
                }
            }
        }

        private static int fullCount = 0;
        private static readonly object naming = new object();

        /// <summary>
        /// Конструктор связывает обработчик с массивом и дает потокам уникальные имена
        /// </summary>
        /// <param name="threadCount">Число потоков обработчика</param>
        /// <param name="v">Обрабатываемый массив</param>
        /// <param name="name">Начало имени для потоков</param>
        protected ParallelWorkerWithProgress(int threadCount, int nn, string name, Action<double> ff)
        {
            n = nn;
            tc = threadCount;
            f = ff;
            progress = new double[threadCount];
            if (threadCount > 1)
            {
                Workers = new Thread[threadCount];
                ready = new AutoResetEvent[threadCount];
                pause = new AutoResetEvent[threadCount];

                d = n/threadCount;

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
        protected void Run(int due, int period)
        {
            if (deadEnd) throw new Exception("Обработчик уже освобожден");
            for (int i = 0; i < progress.Length; i++)
                progress[i] = 0.0;
            if (n > 1)
            {
                if (timer == null) timer = new Timer(Sum, null, due, period);
                else timer.Change(due, period);
            }
            if (tc > 1)
            {
                WaitHandle.WaitAll(pause);

                lock (go) Monitor.PulseAll(go);

                WaitHandle.WaitAll(ready);
            }
            else
            {
                DoFromTo(0, n, (x) => progress[0] = x);
            }
            Sum(null);
            if (n > 1)
                timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Обработчик части массива
        /// </summary>
        protected void DoOne(object nm)
        {
            int num = (int) nm;
            int mod = n%Workers.Length;
            int str;
            int end;
            if (num < mod)
            {
                str = (d + 1)*num;
                end = str + d + 1;
            }
            else
            {
                str = (d + 1)*mod + d*(num - mod);
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

                DoFromTo(str, end, (x) => progress[num] = x);

                //Сигнализация о завершении
                myReady.Set();
            }
        }

        /// <summary>
        /// Сложение всех прогрессов отдельных потоков
        /// </summary>
        protected void Sum(object o)
        {
            double t = 0.0;
            for (int i = 0; i < progress.Length; i++)
                t += progress[i];
            f(t/progress.Length);
        }

        /// <summary>
        /// Обработка части массива
        /// </summary>
        /// <param name="start">С</param>
        /// <param name="finish">До</param>
        protected abstract void DoFromTo(int start, int finish, Action<double> progr);

        /// <summary>
        /// Освобождение ресурсов после использования и завершение всех потоков
        /// </summary>
        public void Dispose()
        {
            if (deadEnd) return;
            f = (x) => { };
            if (tc > 1)
            {
                exit = true;
                Run(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                ready.Let(x => x.Dispose());
                pause.Let(x => x.Dispose());
            }
            timer.Dispose();
            deadEnd = true;
        }
    }
}
