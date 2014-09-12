using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;

namespace Araneam
{
    /// <summary>
    /// Подключаемая к нейронной сети вычислительная часть
    /// </summary>
    /// <typeparam name="T">Тип входных значений</typeparam>
    public abstract class Plug<T>
    {
        protected int dimension;

        /// <summary>
        /// Возвращает длину векторов, которые обязуется возвращать метод Calc()
        /// </summary>
        public int Dimension
        { get { return dimension; } }

        public abstract Vector Calc(T input);

        public virtual Vector Calc(T input, double m)
        {
            return Calc(input).Multiplication(1.0 / m);
        }
    }
}
