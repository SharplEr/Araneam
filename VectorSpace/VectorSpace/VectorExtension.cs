using System;
using MyParallel;
using System.Drawing;

namespace VectorSpace
{
    public static class VectorExtension
    {
        ///// <summary>
        ///// Умножает вектор на скаляр
        ///// </summary>
        ///// <param name="v">Умножаемый вектор</param>
        ///// <param name="k">Скаляр</param>
        //public static Vector Multiplication(this Vector v, Double k)
        //{
        //    for (int i = 0; i < v.Length; i++) v[i] = k * v[i];

        //    return v;
        //}


        //public static Vector Addication(this Vector vf, Vector vs)
        //{
        //    for (int i = 0; i < vf.Length; i++) vf[i] += vs[i];

        //    return vf;
        //}

        //public static Vector Set(this Vector v, Func<int, Double> f)
        //{
        //    for (int i = 0; i < v.Length; i++)
        //        v[i] = f(i);
        //    return v;
        //}

        //public static Vector Set(this Vector v, Func<int, Double> f, int k)
        //{
        //    ParallelArray.set<Double>(v.element, f, k);
        //    return v;
        //}

        //public static Vector Set(this Vector v, Vector vs)
        //{
        //    for (int i = 0; i < v.Length; i++)
        //        v[i] = vs[i];
        //    return v;
        //}

        ///// <summary>
        ///// Создаёт случайный вектор от -1 до +1
        ///// </summary>
        ///// <param name="v">Вектор в котором будет результат</param>
        //public static Vector SetRandomVector(this Vector v)
        //{
        //    Random r = new Random();

        //    for (int i = 0; i < v.Length; i++) v[i] = 2*r.NextDouble()-1;

        //    return v;
        //}

        //public static void Normalization(this Vector v)
        //{
        //    double max = v[0];

        //    for (int i = 1; i < v.Length; i++)
        //        if (max<v[i]) max = v[i];

        //    for (int i = 0; i < v.Length; i++)
        //    {
        //        v[i]/=max;
        //    }
        //}

        /*
        public static explicit operator Vector(Bitmap map)
        {
            Vector v = new Vector(map.Height*map.Width*3);
            int i, j;
            int k, m;
            for (i = 0; i < map.Width; i++)
            {
                k = i * map.Height;
                for (j = 0; j < map.Height; j++)
                {
                    m = k + j;
                    v[m] = map.GetPixel(i, j).R / 256.0;
                    v[m+1] = map.GetPixel(i, j).G / 256.0;
                    v[m+2] = map.GetPixel(i, j).B / 256.0;
                }
            }

           return v;
        }

       
        public static Bitmap GetBitmap(this Vector v, int Height)
        {
            int w = v.Length/3/Height;
            int h = v.Length / 3 / w;

            Bitmap map = new Bitmap(w, h);

            int k, m, i, j;

            for (i = 0; i < map.Width; i++)
            {
                k = i * map.Height;
                for (j = 0; j < map.Height; j++)
                {
                    m = k + j;
                    map.SetPixel(i, j, Color.FromArgb((int)Math.Round(v[m] * 256), (int)Math.Round(v[m + 1] * 256), (int)Math.Round(v[m] * 256)));
                }
            }

            return map;
        }
         */ 
    }
}