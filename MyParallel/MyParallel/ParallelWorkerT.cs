using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyParallel
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Это базовый класс для многопоточной обработки данных, которые можно представить в виде одномерного массива.
     * 1!) Работа с ним должна производиться в однопоточном апартаменте, потому что жизнь несправедлива
     * 2!) После работы требуется вызвать метод Dispose()
     * 3) Для реализации достаточно реализовать метод DoFromTo()
     *      3.1) Так же имеет смысл сделать свой конструктор для инициализации новых полей, если необходимо.
     *      3.2) И для передачи неких параметров следует добавить метод Run() принимающий эти параметры, присваивающих их новым полям а в конце вызывающий метод Run()
     * 4) Для запуска обработки достаточно вызвать метод Run() или новый метод Run с параметрами - см. пункт 3.2
     * 5) Изменения будет сохранены в массив, который передается конструктору.
     * 6) Использовать этот класс целесообразно если
        public static T[] Set<T>(this T[] vector, Func<int, T> f, int pc)
     *      6.A) Данный массив будет модифицироваться множество раз, в противном случае лучше воспользоваться методом Set<T>() из ParallelArray
     *      6.B) Обработка каждого отделнього элемента занимает одинаковое время, иначе данный обработчик может оказаться не эффективным.
    */ 

    /// <summary>
    /// Базовый класс для параллельного обработчика массивов
    /// </summary>
    /// <typeparam name="T">Тип массива</typeparam>
    public abstract class ParallelWorker<T>
    {
        readonly Thread[] Workers;
        readonly protected T[] vector;

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
        public ParallelWorker(int threadCount, T[] v)
        {
            vector = v;
            tc = threadCount;
            if (threadCount > 1)
            {
                Workers = new Thread[threadCount];
                ready = new AutoResetEvent[threadCount];
                pause = new AutoResetEvent[threadCount];

                d = vector.Length / threadCount;

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
        static object naming = new object();
        /// <summary>
        /// Конструктор связывает обработчик с массивом и дает потокам уникальные имена
        /// </summary>
        /// <param name="threadCount">Число потоков обработчика</param>
        /// <param name="v">Обрабатываемый массив</param>
        /// <param name="name">Начало имени для потоков</param>
        public ParallelWorker(int threadCount, T[] v, string name)
        {            
            vector = v;
            tc = threadCount;
            if (threadCount > 1)
            {
                Workers = new Thread[threadCount];
                ready = new AutoResetEvent[threadCount];
                pause = new AutoResetEvent[threadCount];

                d = vector.Length / threadCount;

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
        [MTAThreadAttribute]
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
                DoFromTo(0, vector.Length);
            }
        }

        /// <summary>
        /// Обработчик части массива
        /// </summary>
        protected void DoOne(object nm)
        {
            int num = (int)nm;
            int mod = vector.Length % Workers.Length;
            int str;
            int end;
            if (num < mod)
            {
                str = (d+1) * num;
                end = str + d + 1;
            }
            else
            {
                str = (d+1) * mod + d*(num-mod);
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
                for (int i = 0; i < Workers.Length; i++)
                    ready[i].Close();
                for (int i = 0; i < Workers.Length; i++)
                    pause[i].Close();
            }
            deadEnd = true;
        }
    }
}
