using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;

namespace UnitTestNetwork
{
    [TestClass]
    public class UnitTestNeuron
    {
        [TestMethod]
        public void TestNeuronClone()
        {
            const int n = 5;
            Neuron neuron = new Neuron(n, Math.Sin);

            for (int i = 0; i < n; i++)
            {
                neuron.synapse[i] = i+1;
                neuron.Weight[i] = 1.0/(i + 1);
            }

            double t = neuron.CalcS();

            Neuron newNeuron = (Neuron) neuron.Clone();

            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(i + 1, newNeuron.synapse[i], "Данные не были скопированны");
                Assert.AreEqual(1.0 / (i + 1), newNeuron.Weight[i], "Данные не были скопированны");
            }

            Assert.AreEqual(t, newNeuron.CalcS(), "Функция активации не была скопированна");

            for (int i = 0; i < n; i++)
            {
                newNeuron.synapse[i] *= 2;
                newNeuron.Weight[i] *= 2;
            }

            newNeuron.activationFunction = Math.Cos;

            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(i + 1, neuron.synapse[i], "Изменения в копии затрагивают оригинал");
                Assert.AreEqual(1.0 / (i + 1), neuron.Weight[i], "Изменения в копии затрагивают оригинал");
            }

            Assert.AreEqual(t, neuron.CalcS(), "Изменения в копии затрагивают оригинал");
        }
    }
}
