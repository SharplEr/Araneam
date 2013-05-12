using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Araneam
{
    
    [Serializable]
    /// <summary>
    /// Инкапсулирует информацию о функции активации
    /// </summary>
    public class FuncInfo
    {
        /// <summary>
        /// Имя класса функций
        /// </summary>
        readonly public string name;
        /// <summary>
        /// Параметры
        /// </summary>
        readonly public double[] param;
        /// <summary>
        /// Делегат на функцию
        /// </summary>
        [NonSerialized]
        public Func<double, double> f;
        /// <summary>
        /// Делегат на производную функции по аргументу, выраженную через функцию
        /// </summary>
        [NonSerialized]
        public Func<double, double> df;

        public FuncInfo(string nm, params double[] p)
        {
            name = nm;
            param = p;
            Refresh();
        }

        /// <summary>
        /// Воставление несериализуемых данных
        /// </summary>
        public void Refresh()
        {
            f = Activation.get(name, param);
            df = Activation.get("d" + name, param);
        }
    }
}
