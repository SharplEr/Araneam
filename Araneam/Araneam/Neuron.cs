using System;
using VectorSpace;

namespace Araneam
{
    /*
     * Нейроны должны считать быстро, поэтому каждый новый вид расчета активации должен быть добавлен
     * как отедельный метод.
     * К тому же нейрон не будет наследоваться по всей видимости,
     * поскольку виртуальных методов в нем тоже быть не должно.
    */ 
    /// <summary>
    /// Представление одного нейрона
    /// </summary>
    [Serializable]
    public class Neuron: ICloneable
    {
        /// <summary>
        /// Входной сигнал
        /// </summary>
        [NonSerialized]
        public Vector synapse;

        /// <summary>
        /// Весы
        /// </summary>
        public Vector weight;

        /// <summary>
        /// Размерность нейрона
        /// </summary>
        public int Length
        {
            get;
            private set;
        }

        [NonSerialized]
        public Func<Double, Double> activationFunction;

        public Neuron(int n, Func<Double, Double> activFun)
        {
            Length = n;
            synapse = new Vector(Length);
            weight = new Vector(Length);
            activationFunction = activFun;
        }

        /// <summary>
        /// Расчет активации нейрона основанный на скалярном произведении
        /// </summary>
        /// <returns>Активация нейрона</returns>
        public Double CalcS()
        {
            return activationFunction(synapse * weight);
        }

        /// <summary>
        /// Расчет активации нейрона основанный на евклидовой норме
        /// </summary>
        /// <returns>Активация нейрона</returns>
        public Double CalcE()
        {
            return activationFunction((double)(synapse - weight));
        }

        public object Clone()
        {
            Neuron n = new Neuron(Length, activationFunction);
            n.weight.Set(weight);
            n.synapse.Set(synapse);
            return n;
        }

        public bool haveNaN()
        {
            for (int i = 0; i < weight.Length; i++)
                if (Double.IsNaN(weight[i]) || (Double.IsInfinity(weight[i])))
                    return true;
            return false;
        }
    }
}