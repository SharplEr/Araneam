using System.Collections.Generic;
using VectorSpace;

namespace Araneam
{
    /// <summary>
    /// Класс представлят один элемент обучающей выборки
    /// </summary>
    public class Simple
    {
        public Vector input;
        public List<double> errors;
        public double avgError;
    }
}
