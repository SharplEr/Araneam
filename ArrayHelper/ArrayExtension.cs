using System;
using System.Collections.Generic;
using System.IO;

namespace ArrayHelper
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Этот класс - набор статических методов для обработки массивов.
     * 1) Некоторые из них напоминают чем-то монады. Почти все являются методами расширения (кроме Equals)
     * 2) Стоит обратить внимания на Done(), CloneOk() и Let(). Они очень маленькие но решают такие назойливые
     *      вещи, вроде невозможности нормально инициировать массив. Или глупую нетипизированность Clone().
    */
    public static class ArrayExtension
    {
        /// <summary>
        /// Инициализация массива объектов
        /// </summary>
        public static void Done<T>(this T[] arr) where T : new()
        {
            if (arr==null) return;
            for (int i = 0; i < arr.Length; i++)
                arr[i] = new T();
        }

        /// <summary>
        /// Получение массива из некоторой функции
        /// </summary>
        /// <typeparam name="T">Тип массива</typeparam>
        /// <param name="f">Функция генератор</param>
        /// <param name="n">Число элементов</param>
        public static T[] ToArray<T>(this Func<int, T> f, int n)
        {
            if (f==null) throw new ArgumentException("Нет функции");
            T[] ans = new T[n];
            for (int i = 0; i < n; i++)
                ans[i] = f(i);
            return ans;
        }
        /// <summary>
        /// Поучение массива из нумератора
        /// </summary>
        /// <typeparam name="T">Тип массива</typeparam>
        /// <param name="geter">Нумератор</param>
        /// <param name="n">Число элементов</param>
        public static T[] ToArray<T>(this IEnumerator<T> geter, int n)
        {
            if (geter==null) throw new ArgumentException("geter==null");
            T[] ans = new T[n];

            geter.Reset();
            geter.MoveNext();

            for (int i = 0; i < n; i++)
            {
                ans[i] = geter.Current;
                geter.MoveNext();
            }

            return ans;
        }

        /// <summary>
        /// Переводит массив типа T в массив типа U
        /// </summary>
        /// <typeparam name="T">Первый тип</typeparam>
        /// <typeparam name="TOut">Второй тип</typeparam>
        /// <param name="vector">Массив</param>
        /// <param name="f">Функция конвертации</param>
        public static TOut[] Convert<T, TOut>(this T[] vector, Func<T, TOut> f)
        {
            if (vector == null) return null;
            if (f==null) throw new ArgumentException("функция преобразования == null");

            TOut[] ans = new TOut[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                ans[i] = f(vector[i]);
            return ans;
        }

        /// <summary>
        /// Типизированный Clone()
        /// </summary>
        public static T CloneOk<T>(this T o) where T : ICloneable
        {
            if (o == null) return default(T);
            return (T)o.Clone();
        }

        /// <summary>
        /// Типизированный Clone() для массивов
        /// </summary>
        public static T[] CloneOk<T>(this T[] o) where T : ICloneable
        {
            if (o == null) return null;

            T[] a = new T[o.Length];
            for (int i = 0; i < o.Length; i++)
            {
                a[i] = o[i].CloneOk();
            }
            return a;
        }

        /// <summary>
        /// Типизированный Clone() с произвольным перемешиванием
        /// </summary>
        /// <param name="o">Массив</param>
        /// <param name="indexer">Индексы</param>
        public static T[] CloneShuffle<T>(this T[] o, int[] indexer) where T:ICloneable
        {
            if (o == null) return null;
            if (indexer == null) throw new ArgumentException("indexer == null");

            T[] a = new T[indexer.Length];

            for (int i = 0; i < a.Length; i++)
                a[i] = (T)o[indexer[i]].Clone();

            return a;
        }

        /// <summary>
        /// Типизированный Clone() с произвольным перемешиванием
        /// </summary>
        /// <param name="o">Массив</param>
        /// <param name="indexer">Индексы</param>
        public static T[] CloneShuffleStruct<T>(this T[] o, int[] indexer) where T : struct
        {
            if (o == null) return null;
            if (indexer == null) throw new ArgumentException("indexer == null");

            T[] a = new T[indexer.Length];

            for (int i = 0; i < a.Length; i++)
                a[i] = o[indexer[i]];

            return a;
        }

        /// <summary>
        /// Выполняет некоторое действие с каждым не равным null элементом перечисления
        /// </summary>
        /// <param name="o">Перечисление</param>
        /// <param name="f">Действие</param>
        public static void Let<T>(this IEnumerable<T> o, Action<T> f)
        {
            if (o == null || f == null) return;
            foreach (T item in o)
                if (item != null) f(item);
        }

        /// <summary>
        /// Сравнение двух массивов
        /// </summary>
        /// <param name="arr1">Первый массив</param>
        /// <param name="arr2">Второй массив</param>
        public static bool Equals<T>(T[] arr1, T[] arr2) where T : IEquatable<T>
        {
            if (arr1 == null || arr2 == null) return arr1 == arr2;

            if (arr1.Length != arr2.Length) return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (!arr1[i].Equals(arr2[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Удаение повторяющихся массивов
        /// </summary>
        /// <param name="arr">Массив массивов</param>
        public static List<T[]> Dist<T>(this T[][] arr) where T : IEquatable<T>
        {
            List<T[]> ans = new List<T[]>();
            
            if (arr!=null)
            for (int i = 0; i < arr.Length; i++ )
            {
                T[] t = arr[i];

                bool flag = true;

                for (int j = 0; j < ans.Count; j++)
                    if (Equals(ans[j], t)) flag = false;

                if (flag) ans.Add(t);
            }

            return ans;
        }

        /// <summary>
        /// Проверяет являются ли элементы массива объектами заданного типа.
        /// И если да, то возвращает типизированный массив.
        /// </summary>
        /// <typeparam name="T">Необходимый тип</typeparam>
        /// <param name="o">Массив</param>
        /// <returns>Типизированный массив</returns>
        public static T[] CheckType<T>(this object[] o)
        {
            if (o == null) return null;

            Type t = typeof(T);

            for(int i = 0; i < o.Length; i++)
                if(!t.IsAssignableFrom(o[i].GetType())) return null;

            return o.Convert((x) => (T)x);
        }

        public static void WriteTo<T>(this T[] o, TextWriter writer, string sep)
        {
            if (o == null || writer == null) return;

            int len = o.Length - 1;
            for (int i = 0; i < len; i++)
                writer.Write(o[i].ToString()+sep);
            writer.Write(o[o.Length - 1].ToString());
        }

        public static void WriteTo<T>(this T[] o, string name, string sep)
        {
            using (var writer = new StreamWriter(name))
                o.WriteTo(writer, sep);
        }
    }
}
