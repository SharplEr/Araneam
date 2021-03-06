﻿using System;
using ArrayHelper;
using VectorSpace;

namespace IOData
{
    /*
     * spectrum представляет вектор со значениями +1 и -1.
     * +1 в элементе i - обозначает принадлежность к i классу
     * только один элемент равен +1, остальные равны -1.
    */
    [Serializable]
    public class Result: ICloneable
    {
        //Номер класса
        int? number;
        //Спектр принадлежности классам
        Vector spectrum;

        //Число классов, то есть длина спектра
        readonly int max;

        readonly Object bloker = new Object();

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
                    if (spectrum.IsNull) spectrum = new Vector(max, (i) => (i != number) ? -1.0 : 1.0);
                    return spectrum;
                }
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

        public Result(Vector s)
        {
            max = s.Length;
            
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