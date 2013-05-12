using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorSpace;
using NeuralNetwork;
using System.Threading;

namespace UnitTestNetwork
{
    [TestClass]
    public class UnitTestLayer
    {
        [TestMethod]
        public void TestLayerInvers()
        {
            Vector v = new Vector(4);
            int[][] inp = new int[2][];
            inp[0] = new int[] { 0, 2 };
            inp[1] = new int[] { 1, 2, 3 };

            NeuronLayer nl = new NeuronLayer(2, inp, false,"no");
            Thread t = new Thread(()=>nl.Input = v);
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
            t.Join();

            nl.CalcInvers(false);
            Assert.AreEqual(4, nl.InversIndex.Length);
            Assert.AreEqual(nl.InversIndex[0].index[0], 0, "ошибка для первого входа");
            Assert.AreEqual(nl.InversIndex[0].subIndex[0], 0, "ошибка для первого входа");

            Assert.AreEqual(nl.InversIndex[1].index[0], 1, "ошибка для второго входа");
            Assert.AreEqual(nl.InversIndex[1].subIndex[0], 0, "ошибка для второго входа");

            Assert.AreEqual(nl.InversIndex[2].index[0], 0, "ошибка для 3 входа");
            Assert.AreEqual(nl.InversIndex[2].subIndex[0], 1, "ошибка для 3 входа");

            Assert.AreEqual(nl.InversIndex[2].index[1], 1, "ошибка для 3_2 входа");
            Assert.AreEqual(nl.InversIndex[2].subIndex[1], 1, "ошибка для 3_2 входа");

            Assert.AreEqual(nl.InversIndex[3].index[0], 1, "ошибка для 4 входа");
            Assert.AreEqual(nl.InversIndex[3].subIndex[0], 2, "ошибка для 4 входа");

            nl.CalcInvers(true);
            Assert.AreEqual(3, nl.InversIndex.Length);
            Assert.AreEqual(nl.InversIndex[0].index[0], 0, "ошибка для первого входа");
            Assert.AreEqual(nl.InversIndex[0].subIndex[0], 0, "ошибка для первого входа");

            Assert.AreEqual(nl.InversIndex[1].index[0], 1, "ошибка для второго входа");
            Assert.AreEqual(nl.InversIndex[1].subIndex[0], 0, "ошибка для второго входа");

            Assert.AreEqual(nl.InversIndex[2].index[0], 0, "ошибка для 3 входа");
            Assert.AreEqual(nl.InversIndex[2].subIndex[0], 1, "ошибка для 3 входа");

            Assert.AreEqual(nl.InversIndex[2].index[1], 1, "ошибка для 3_2 входа");
            Assert.AreEqual(nl.InversIndex[2].subIndex[1], 1, "ошибка для 3_2 входа");
        }

        [TestMethod]
        public void TestLayerCalc()
        {
            Vector v = new Vector(4);
            int[][] inp = new int[2][];
            inp[0] = new int[] { 0, 2 };
            inp[1] = new int[] { 1, 2, 3 };

            v[0] = 1;
            v[1] = 2;
            v[2] = 3;
            v[3] = 4;

            NeuronLayer nl = new NeuronLayer(2, inp, false,"no");
            Thread t = new Thread(() => nl.Input = v);
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
            t.Join();
            nl.neuros[0].weight[0] = 0.5;
            nl.neuros[0].weight[1] = 1.5;

            nl.neuros[1].weight[0] = 1.0;
            nl.neuros[1].weight[1] = 2.0;
            nl.neuros[1].weight[2] = 3.0;

            Vector ans = null;

            t = new Thread(() => ans = nl.Calc());
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
            t.Join();

            Assert.AreEqual(ans[0], 5, "ошибка в 1 элементе");
            Assert.AreEqual(ans[1], 20, "ошибка во 2 элементе");
        }

        [TestMethod]
        public void TestLayerIndexer()
        {
            NeuronLayer nl = new NeuronLayer(2, 3, false, "no");

            Assert.AreNotEqual(null, nl.InputIndex, "Массив индексов не инициализирован");

            Assert.AreEqual(2, nl.InputIndex.Length, "Число нейронов неверное");
            Assert.AreEqual(3, nl.InputIndex[0].Length, "Число входов для первого нейрона неверное");
            Assert.AreEqual(3, nl.InputIndex[1].Length, "Число входов для второго нейрона неверное");

            for (int i = 0; i < nl.InputIndex.Length; i++)
                for (int j = 0; j < nl.InputIndex[i].Length; j++)
                    Assert.AreEqual(j, nl.InputIndex[i][j], "Неверная ссылка");

            new Thread(() => nl.Dispose()).Start();

            nl = new NeuronLayer(2, 3, true, "no");

            Assert.AreNotEqual(null, nl.InputIndex, "Массив индексов не инициализирован");

            Assert.AreEqual(2, nl.InputIndex.Length, "Число нейронов неверное");
            Assert.AreEqual(3, nl.InputIndex[0].Length, "Число входов для первого нейрона неверное");
            Assert.AreEqual(3, nl.InputIndex[1].Length, "Число входов для второго нейрона неверное");

            for (int i = 0; i < nl.InputIndex.Length; i++)
                for (int j = 0; j < nl.InputIndex[i].Length; j++)
                    Assert.AreEqual(j, nl.InputIndex[i][j], "Неверная ссылка");

            new Thread(() => nl.Dispose()).Start();
        }

        [TestMethod]
        public void TestLayerConstructor()
        {
            NeuronLayer nl = new NeuronLayer(2, 3, false, "no");

            Assert.AreEqual(2, nl.neuros.Length, "Число нейронов неверное");

            Assert.AreEqual(3, nl.neuros[0].Length, "Число синапсов у первого нейрона неверное");
            Assert.AreEqual(3, nl.neuros[1].Length, "Число синапсов у второго нейрона неверное");
            Assert.AreEqual(2, nl.output.Length, "Размерность выхода неверная");

            new Thread(() => nl.Dispose()).Start();

            nl = new NeuronLayer(2, 4, true, "no");

            Assert.AreEqual(nl.neuros.Length, 2, "Число нейронов неверное");

            Assert.AreEqual(4, nl.neuros[0].Length, "Число синапсов у первого нейрона неверное");
            Assert.AreEqual(4, nl.neuros[1].Length, "Число синапсов у второго нейрона неверное");

            Assert.AreEqual(3, nl.output.Length, "Размерность выхода неверная");

            new Thread(() => nl.Dispose()).Start();
        }

        [TestMethod]
        public void TestLayerCalcDer()
        {
            Vector v = new Vector(4);
            int[][] inp = new int[2][];
            inp[0] = new int[] { 0, 2 };
            inp[1] = new int[] { 1, 2, 3 };

            v[0] = 1;
            v[1] = 2;
            v[2] = 3;
            v[3] = 4;

            NeuronLayer nl = new NeuronLayer(2, inp, false, "no");
            Thread t = new Thread(() => nl.Input = v);
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
            t.Join();
            nl.neuros[0].weight[0] = 0.5;
            nl.neuros[0].weight[1] = 1.5;

            nl.neuros[1].weight[0] = 1.0;
            nl.neuros[1].weight[1] = 2.0;
            nl.neuros[1].weight[2] = 3.0;

            Vector ans = nl.CalcDer();

            Assert.AreEqual(ans[0], 1, "ошибка в 1 элементе");
            Assert.AreEqual(ans[1], 1, "ошибка во 2 элементе");
            Console.WriteLine("all ok");
        }
    }
}