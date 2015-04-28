using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayHelper
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) См Janus.cs.
     * 1) Это более сложная версия. Тут предполагается, что не все типы могут потребоваться.
     * 2) Поэтому происходит преобразование по требованию. Оно происходит лишь однажды, при первом обращении.
     * 3) Что бы сократить число функций перевода, имеется базовый тип данных,
     *          и функции перевода из него в другие.
    */
    class JanusConvertable<B>
    {
        B[] baseData;

        Dictionary<Type, object[]> data;
        Dictionary<Type, Func<B[], object[]>> convert;

        public JanusConvertable(B[] bdata, Dictionary<Type, Func<B[], object[]>> fs)
        {
            baseData = bdata;

            convert = fs;

            data = new Dictionary<Type, object[]>(convert.Count);
            Type[] temp = convert.Keys.ToArray();
            for (int i = 0; i < temp.Length; i++)
                data.Add(temp[i], null);
        }

        public T[] Get<T>()
        {
            Type t = typeof(T);
            object[] ans = data[t];

            if (ans == null)
            {
                data[t] = convert[t](baseData);
                ans = data[t];
            }

            return ans.Convert((x) => (T)x);
        }

        public B[] Get()
        { return baseData; }
    }
}