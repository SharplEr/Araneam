using System;
using MyParallel;
using System.Text;

namespace VectorSpace
{
    /// <summary>
    /// Представление вектора с вещественными координатами
    /// </summary>
    [Serializable]
    public class Vector: ICloneable
    {
        /// <summary>
        /// Координаты вектора
        /// </summary>
        public Double[] element;

        /// <summary>
        /// Конструктор создает вектор с заданной размерностью
        /// </summary>
        /// <param name="n">Размерность</param>
        public Vector(int n)
        {
            if (n <= 0) throw new ArgumentException("размерность вектора должен быть больше 0");
            element = new Double[n];
        }

        /// <summary>
        /// Конструктор создает вектор заданной размерности и инициализирует
        /// </summary>
        /// <param name="n">Размерность</param>
        /// <param name="f">Функция инициализации</param>
        public Vector(int n, Func<int, Double> f)
        {
            if (n <= 0) throw new ArgumentException("размерность вектора должен быть больше 0");
            element = new Double[n];
            for (int i = 0; i < n; i++) element[i] = f(i);
        }

        public Vector(int n, Func<int, Double> f, double x)
        {
            element = new Double[n+1];
            for (int i = 0; i < n; i++) element[i] = f(i);
            element[n] = x;
        }

        public Double this[int index]
        {
            get
            {
                return element[index];
            }

            set
            {
                element[index] = value;
            }
        }

        public Object Clone()
        {
            Vector v = new Vector(element.Length);
            //Магическая константа, при размере массива больше которой стандартный метод обгоняет простую итерацию
            if (element.Length < 50)
            {
                for (int i = 0; i < element.Length; i++)
                    v.element[i] = element[i];
            }
            else
                element.CopyTo(v.element, 0);
            return v;
        }

        public static Vector operator +(Vector vf, Vector vc)
        {
            Vector ans = new Vector(vf.Length);

            for (int i = 0; i < vf.Length; i++) ans[i] = vf[i] + vc[i];

            return ans;
        }

        public static Vector operator -(Vector vf, Vector vc)
        {
            Vector ans = new Vector(vf.Length);

            for (int i = 0; i < vf.Length; i++) ans[i] = vf[i] - vc[i];

            return ans;
        }

        public unsafe static Double operator *(Vector vf, Vector vs)
        {
            double ans = 0.0;
            fixed(double* pvf0 = vf.element, pvs0 = vs.element)
            {
                double* pvf = pvf0;
                double* pvs = pvs0;
                double* pend = pvf0+vf.Length;

                while(pvf<pend)
                {
                    ans += (*pvf) * (*pvs);
                    pvf++;
                    pvs++;
                }
            }
            return ans;
        }

        public static Vector operator *(Double k, Vector v)
        {
            Vector ans = new Vector(v.Length);

            for (int i = 0; i < v.Length; i++) ans[i] = k * v[i];

            return ans;
        }

        /// <summary>
        /// Квадрат нормы вектора
        /// </summary>
        /// <param name="v">Вектор</param>
        /// <returns>Квадрат нормы вектора</returns>
        public static unsafe explicit operator Double(Vector v)
        {
            double ans = 0.0;
            fixed (double* pv0 = v.element)
            {
                double* pv = pv0;
                double* pend = pv0 + v.Length;
                double t;
                while (pv < pend)
                {
                    t = *pv;
                    ans += t * t;
                    pv++;
                }
            }
            return ans;
        }

        /// <summary>
        /// Размерность вектора
        /// </summary>
        public int Length
        {
            get
            {
                return element.Length;
            }
        }

        /// <summary>
        /// Умножение вектор на скаляр
        /// </summary>
        public unsafe Vector Multiplication(Double k)
        {
            fixed (double* pv0 = this.element)
            {
                double* pv = pv0;
                double* pend = pv0 + this.Length;

                while (pv < pend)
                {
                    *pv *= k;
                    pv++;
                }
            }
            return this;
        }
        
        public Vector Multiplication(Vector v)
        {
            for (int i = 0; i < element.Length; i++) element[i] = v[i] * element[i];

            return this;
        }

        public unsafe Vector Addication(Vector v)
        {
            fixed (double* pthis = element, pv = v.element)
            {
                double* pend = pthis + element.Length;
                double* tt = pthis, tv = pv;
                while (tt < pend)
                {
                    *tt += *tv;
                    tt++;
                    tv++;
                }
            }

            return this;
        }

        public unsafe Vector Addication(double x)
        {

            fixed (double* pv0 = this.element)
            {
                double* pv = pv0;
                double* pend = pv0 + this.Length;

                while (pv < pend)
                {
                    *pv += x;
                    pv++;
                }
            }
            return this;
        }

        public Vector Set(Func<int, Double> f)
        {
            for (int i = 0; i < element.Length; i++)
                element[i] = f(i);
            return this;
        }

        //!
        public Vector Set(Vector v)
        {
            if (element.Length < 50)
            {
                for (int i = 0; i < element.Length; i++)
                    element[i] = v[i];
            }
            else
                v.element.CopyTo(element, 0);

            return this;
        }

        /// <summary>
        /// Заполняет случайными коориданатами от -1 до +1
        /// </summary>
        public Vector SetRandom()
        {
            Random r = new Random();

            for (int i = 0; i < element.Length; i++) element[i] = 2 * r.NextDouble() - 1;

            return this;
        }

        /// <summary>
        /// Заполняет случайными координатами от a до b
        /// </summary>
        public Vector SetRandom(double a, double b)
        {
            Random r = new Random();

            for (int i = 0; i < element.Length; i++) element[i] = (b-a) * r.NextDouble() + a;

            return this;
        }

        /// <summary>
        /// Нормализует вектор (каждая координата будет от -1 до 1)
        /// </summary>
        public Vector Normalization()
        {
            double max = Math.Abs(element[0]);
            double t;

            for (int i = 1; i < element.Length; i++)
            {
                t = Math.Abs(element[i]);
                if (max < t) max = t;
            }

            for (int i = 0; i < element.Length; i++)
            {
                element[i] /= max;
            }

            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(");

            sb.Append(element[0]);

            for (int i = 1; i < Length; i++)
            {
                sb.Append(", ");
                sb.Append(element[i]);
            }

            sb.Append(")");
            return sb.ToString();
        }
    }
}