using System;
using VectorSpace;
using ArrayHelper;

namespace IOData
{
    /*
     * spectrum представляет вектор со значениями +1 и -1.
     * +1 в элементе i - обозначает принадлежность к i классу
     * только один элемент равен +1, остальные равны -1.
    */
    public class Result: ICloneable
    {
        //Номер класса
        int? number;
        //Спектр принадлежности классам
        Vector spectrum;

        //Число классов, то есть длина спектра
        int max;

        Object bloker = new Object();

        public int MaxNumber
        {
            get
            { return max; }
        }

        public int Number
        {
            get
            {
                lock (bloker)
                {
                    if (number == null)
                    {
                        for (int i = 0; i < spectrum.Length; i++)
                            if (spectrum[i] == 1.0)
                            {
                                number = i;
                                break;
                            }
                    }
                }
                return number.Value;
            }
        }

        public Vector Spectrum
        {
            get
            {
                lock (bloker)
                {
                    if (spectrum == null)
                        spectrum = new Vector(max, (i) => (i != number) ? -1.0 : 1.0);
                }
                return spectrum;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="n">Номер классов</param>
        /// <param name="maxN">Общее число классов</param>
        public Result(int n, int maxN)
        {
            number = n;

            max = maxN;
        }

        //Можно разрешать конфликт двумя способами. 1. Максимальный элемент и есть няшка. 2. Элемент ближайший к 1 и есть няшка.
        //Поскольку жизнь не справедлива хуй знает что лучше.
        public Result(Vector s)
        {
            max = s.Length;
            /*//Первый вариант, логичный но меньше подходит смыслу обучения нейронной сети
            int maxi = 0;
            double maxz = s[0];
            for (int i = 1; i < max; i++)
                if (maxz < s[i])
                {
                    maxi = i;
                    maxz = s[i];
                }
  */
            
            int maxi = 0;
            double maxEquals = Math.Abs(1.0 - s[0]);
            for (int i = 1; i < max; i++)
                if (maxEquals > Math.Abs(1.0-s[i]))
                {
                    maxi = i;
                    maxEquals = Math.Abs(1.0 - s[i]);
                }

            spectrum = new Vector(max, (i) => (i != maxi) ? -1.0 : 1.0);
        }

        Result(Result r)
        {
            number = r.number;
            max = r.max;
            spectrum = r.spectrum.CloneOk();
        }

        public Object Clone()
        {
            return new Result(this);
        }

        public static Boolean operator ==(Result r1, Result r2)
        {
            return r1.Number == r2.Number;   
        }

        public static Boolean operator !=(Result r1, Result r2)
        {
            return r1.Number != r2.Number;   
        }

    }
}