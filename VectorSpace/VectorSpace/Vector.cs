using System;
using System.Text;
//using Yeppp;

namespace VectorSpace
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Этот класс - реализация n-мерного вектора и сопутствующих методов для удобной работы
     * 1) В классе имеется только 1 поле - double[] element, любые изменения происходят только в нем после выполения методов
     * 2) Методы из класса не вызывают друг-друга за исключением конструкторов (вызываемых в операторах)
     * 3) Все методы, кроме операторов, модифицируют текущий класс. Операторы наоборот создают новый объект.
     * 4) Все методы (кроме ToString() и операторов) имеют fluent interface, возвращая ссылку на текущий объект.
    */
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

        public static Vector operator *(Vector[] matrix, Vector v)
        {
            Vector ans = new Vector(matrix.Length);

            for (int i = 0; i < matrix.Length; i++) ans[i] = matrix[i] * v;

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
                //ans = Yeppp.Core.SumSquares_V64f_S64f(pv0, v.Length);
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
                //Yeppp.Core.Multiply_IV64fS64f_IV64f(pv0, k, this.Length);
                
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
        
        public unsafe Vector Multiplication(Vector v)
        {
            /*
            fixed (double* pvf = this.element, pvs = v.element)
            {
                Yeppp.Core.Multiply_IV64fV64f_IV64f(pvf, pvs, this.Length);
            }*/
            
            for (int i = 0; i < element.Length; i++) element[i] = v[i] * element[i];

            return this;
        }

        public unsafe Vector Addication(Vector v)
        {
            fixed (double* pthis = element, pv = v.element)
            {
                //Yeppp.Core.Add_IV64fV64f_IV64f(pthis, pv, this.Length);

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
                //Yeppp.Core.Add_IV64fS64f_IV64f(pv0, x, this.Length);
                
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
        /// Заполняет случайными координатами от a
        /// </summary>
        public Vector SetRandomAroundZero(double a)
        {
            Random r = new Random();

            for (int i = 0; i < element.Length; i++) element[i] = 2 * a * r.NextDouble() - a;

            return this;
        }

        /// <summary>
        /// Заполняет случайными координатами от a
        /// </summary>
        public Vector SetRandomAroundZero(double a, double epsilon)
        {
            Random r = new Random();

            double b = a - epsilon;
            double t;

            for (int i = 0; i < element.Length; i++)
            {
                t = 2 * b * r.NextDouble() - b;
                if (t >= 0) element[i] = t + epsilon;
                element[i] = t - epsilon;
            }

            return this;
        }

        /// <summary>
        /// Заполняет случайными координатами от a
        /// </summary>
        public Vector SetRandomAroundZero(double a, double epsilon, Random r)
        {
            double b = a - epsilon;
            double t;

            for (int i = 0; i < element.Length; i++)
            {
                t = 2 * b * r.NextDouble() - b;
                if (t >= 0) element[i] = t + epsilon;
                element[i] = t - epsilon;
            }

            return this;
        }

        /// <summary>
        /// Нормализует вектор (каждая координата будет от -1 до 1)
        /// </summary>
        public Vector Normalization()
        {
            return this.Multiplication(1.0 / (double)this);
        }

        public Vector InvPer()
        {
            double sum = element[0];
            for (int i = 1; i < element.Length; i++)
                sum += element[i];

            double a = 2.0 / element.Length;

            for (int i = 0; i < element.Length; i++)
                element[i] = a - element[i] / sum;

            return this;
        }

        public Vector AddThreshold(double threshold)
        {
            Vector m = new Vector(2);

            m[0] = (Math.Sign(threshold) - element[0]) * Math.Abs(threshold);
            m[1] = (-Math.Sign(threshold) - element[1]) * Math.Abs(threshold);

            return this.Addication(m);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(");

            sb.Append(element[0]);

            for (int i = 1; i < Length; i++)
            {
                sb.Append("; ");
                sb.Append(element[i]);
            }

            sb.Append(")");
            return sb.ToString();
        }
    }
}