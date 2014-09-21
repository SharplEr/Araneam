using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using IOData;

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
