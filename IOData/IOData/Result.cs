using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;

namespace IOData
{
    /*
     * spectrum представляет вектор со значениями +1 и -1.
     * +1 в элементе i - обозначает принадлежность к i классу
     * только один элемент равен +1, остальные равны -1.
    */
    public class Result
    {
        //Номер класса
        int? number;
        //Спектр принадлежности классам
        Vector spectrum;

        //Число классов, то есть длина спектра
        int max;

        public int MaxNumber
        {
            get
            { return max; }
        }

        public int Number
        {
            get
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

                return number.Value;
            }
        }

        public Vector Spectrum
        {
            get
            {
                if (spectrum == null)
                    spectrum = new Vector(max, (i) => (i != number) ? -1.0 : 1.0);

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

        public Result(Vector s)
        {
            spectrum = s;

            max = s.Length;
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