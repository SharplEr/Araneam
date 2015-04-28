using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayHelper
{

    /// <summary>
    /// Представляет оболочку для данных, которые можно представить как конечный массив
    /// </summary>
    /// <typeparam name="T">Тип массива</typeparam>
    public class ArrayShall<T>: ICloneable
    {
        Func<int, T> getItem;
        int length;

        public ArrayShall(Func<int, T> f, int n)
        {
            getItem = f;
            length = n;
        }

        public T this[int i]
        {
            get
            {
                if ((i < 0) || (i >= length)) throw new ArgumentOutOfRangeException(i.ToString());
                return getItem(i);
            }
        }

        public int Length
        {
            get { return length; }
        }

        public Object Clone()
        {
            return this;
        }

        public T[] ToArray()
        {
            T[] ans = new T[length];

            for (int i = 0; i < length; i++)
                ans[i] = getItem(i);

            return ans;
        }
    }
}