using System;
using VectorSpace;

namespace Araneam
{
    /// <summary>
    /// Представление одного нейрона
    /// </summary>
    [Serializable]
    public struct Neuron: ICloneable
    {
        [NonSerialized]
        public Vector synapse;

        public Vector weight;
        public readonly int Length;

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
    }
}