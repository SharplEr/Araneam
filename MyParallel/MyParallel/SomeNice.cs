using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyParallel
{
    public static class SomeNice
    {
        /// <summary>
        /// Переводит массив типа T в массив типа U
        /// </summary>
        public static U[] Convert<T, U>(this T[] vector, Func<T, U> f)
        {
            U[] ans = new U[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                ans[i] = f(vector[i]);
            return ans;
        }

        /// <summary>
        /// Типизированный Clone()
        /// </summary>
        public static T CloneOk<T>(this T o) where T : ICloneable
        {
            return (T)o.Clone();
        }

        /// <summary>
        /// Типизированный Clone() для массивов
        /// </summary>
        public static T[] CloneOk<T>(this T[] o) where T : ICloneable
        {
            T[] a = new T[o.Length];
            for (int i = 0; i < o.Length; i++)
            {
                a[i] = o[i].CloneOk();
            }
            return a;
        }

        public static void let<T>(this IEnumerable<T> o, Action<T> f)
        {
            if (o == null) return;
            foreach(T item in o)
                if (item!=null) f(item);
        }
    }
}
