using System;
using System.Collections.Generic;

namespace ArrayHelper
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Двуликий Янус (Janus Bifrons) божество с двумя головами. Часто используют как метафору чего-то изменчевого,
     *      прибывающего в разных формах. Какое хочу название такое и даю.
     * 1) Класс хранит массивы разных типов. Предполагается что это одни и теже данные в разных форматах.
     *      Хотя это и не обязательно.
     * 2) Метод Get<T> возвращает типизированный массив заданного типа.
     * 2.1) Надо указать в точности тот тип, который используется. Предки запрещены!
     * 3) Типы определяются автоматически из массива массивов object.
     * 4) !!!Все типы должны быть разными!!!
     * 5) !!!Предполагается что переданные массивы не будут изменяться извне.
     * 5.1) Изменение будет потоко небезопасным.
     * 6) Добавление новых данных потокобезопасно.
    */
    /// <summary>
    /// Двуликий Янус. Это класс, который предствляет хранение одних и тех же данных в разных форматах.
    /// </summary>
    public class Janus
    {
        readonly Dictionary<Type, object[]> data;

        public Janus(object[][] o)
        {
            data = new Dictionary<Type, object[]>(o.Length);

            for (int i = 0; i < o.Length; i++)
                data.Add(o[i].GetType().GetElementType(), o[i]);
        }

        /// <summary>
        /// Возвращает массив заданного типа, если он доступен
        /// </summary>
        /// <typeparam name="T">Необходимый тип</typeparam>
        /// <returns>Массив заданного типа</returns>
        public T[] Get<T>()
        {
            Type t = typeof(T);

            if (!data.ContainsKey(t)) return null;

            return data[t].Convert((x) => (T)x);
        }

        readonly object locko = new object();

        public void Add(object[] o)
        {
            lock (locko)
            {
                data.Add(o.GetType().GetElementType(), o);
            }
        }
    }
}
