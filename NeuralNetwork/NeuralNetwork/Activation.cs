using System;

namespace Araneam
{
    /// <summary>
    /// Набор функций активации
    /// </summary>
    public struct Activation
    {
        /// <summary>
        /// Возвращает функцию активации по имени и набору параметров
        /// </summary>
        /// <param name="name">Общее имя функции активации</param>
        /// <param name="p">Некоторый массив параметров</param>
        public static Func<Double, Double> get(string name, params Double[] p)
        {
            switch (name)
            {
                case "threshold":
                    return getThreshold();
                case "piecewiseLiner":
                    return getPiecewiseLiner();
                case "sigmoid":
                    if (p != null)
                        return getSigmoid(p[0]);
                    break;
                case "dsigmoid":
                    if (p != null)
                        return getSigmoidDerivative(p[0]);
                    break;
                case "tanh":
                    if (p != null)
                        return getTanh(p[0], p[1]);
                    break;
                case "dtanh":
                    if (p != null)
                        return getTanhDerivative(p[0], p[1]);
                    break;
                case "no": return x => x;
                case "dno": return x => 1.0;
            }
            return null;
        }
        #region Функции активации

        static Func<Double, Double> getThreshold()
        {
            return x => (x >= 0) ? 1 : 0;
        }

        static Func<Double, Double> getPiecewiseLiner()
        {
            return x =>
                {
                    if (x >= 0.5) return 1;
                    else if (x > -0.5) return x;
                    else return 0;
                };
        }

        static Func<Double, Double> getSigmoid(Double a)
        {
            return x => 1.0 / (1 + Math.Exp(-a * x));
        }

        static Func<Double, Double> getSigmoidDerivative(Double a)
        {
            return x => a * x * (1.0 - x);
        }

        static Func<Double, Double> getTanhDerivative(Double a, Double b)
        {

            return x => b / a * (a - x) * (a + x);
        }

        static Func<Double, Double> getTanh(Double a, Double b)
        {
            return x => a * Math.Tanh(b * x);
        }

        static Func<Double, Double> getMultiquadric(Double c)
        {
            return x => Math.Sqrt(x*x + c*c);
        }

        static Func<Double, Double> getInverseMultiquadric(Double c)
        {
            return x => 1.0/Math.Sqrt(x * x + c * c);
        }

        static Func<Double, Double> getGaussian(Double a)
        {
            return x => Math.Exp(-x * x / (2.0 * a * a));
        }

        #endregion
    }
}