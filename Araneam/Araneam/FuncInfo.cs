using System;

namespace Araneam
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Этот класс существует по той причине, что код нельзя сериализовать, подобный класс может быть сериализован а затем востановит код с помощью метода Refresh.
     *      Так же он позволяет удобнее создавать функции активации "на лету" используя только их псевдоним и параметры.
     *      Список всех доступных таким образом функций активации можно найти в структуре Activation
    */ 
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
        /// Восстановление несериализуемых данных
        /// </summary>
        public void Refresh()
        {
            f = Activation.get(name, param);
            df = Activation.get("d" + name, param);
        }
    }
}
