using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorSpace;
using Araneam;
using System.Threading;
using MyParallel;
using ArrayHelper;

namespace UnitTestNetwork
{
    [TestClass]
    public class UnitTestLayer
    {
        [TestMethod]
        public void Test_LayerInvers()
        {
            Vector v = new Vector(4);
            int[][] inp = new int[2][];
            inp[0] = new int[] { 0, 2 };
            inp[1] = new int[] { 1, 2, 3 };

            NeuronLayer nl = new NeuronLayer(2, inp, false, 0, "no");
            new Thread(()=>nl.Input = v).InMTA();

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
        public void Test_LayerCalc()
        {
            Vector v = new Vector(4);
            int[][] inp = new int[2][];
            inp[0] = new int[] { 0, 2 };
            inp[1] = new int[] { 1, 2, 3 };

            v[0] = 1;
            v[1] = 2;
            v[2] = 3;
            v[3] = 4;

            NeuronLayer nl = new NeuronLayer(2, inp, false, 0, "no");
            new Thread(() => nl.Input = v).InMTA();
            
            nl.neuros[0].weight[0] = 0.5;
            nl.neuros[0].weight[1] = 1.5;

            nl.neuros[1].weight[0] = 1.0;
            nl.neuros[1].weight[1] = 2.0;
            nl.neuros[1].weight[2] = 3.0;

            Vector ans = null;

            new Thread(() => ans = nl.Calc()).InMTA();

            Assert.AreEqual(ans[0], 5, "ошибка в 1 элементе");
            Assert.AreEqual(ans[1], 20, "ошибка во 2 элементе");
        }

        [TestMethod]
        public void TestLayerIndexer()
        {
            NeuronLayer nl = new NeuronLayer(2, 3, false, 0, "no");

            Assert.AreNotEqual(null, nl.InputIndex, "Массив индексов не инициализирован");

            Assert.AreEqual(2, nl.InputIndex.Length, "Число нейронов неверное");
            Assert.AreEqual(3, nl.InputIndex[0].Length, "Число входов для первого нейрона неверное");
            Assert.AreEqual(3, nl.InputIndex[1].Length, "Число входов для второго нейрона неверное");

            for (int i = 0; i < nl.InputIndex.Length; i++)
                for (int j = 0; j < nl.InputIndex[i].Length; j++)
                    Assert.AreEqual(j, nl.InputIndex[i][j], "Неверная ссылка");

            new Thread(() => nl.Dispose()).Start();

            nl = new NeuronLayer(2, 3, true, 0, "no");

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
        public void Test_LayerConstructor()
        {
            NeuronLayer nl = new NeuronLayer(2, 3, false, 0, "no");

            Assert.AreEqual(2, nl.neuros.Length, "Число нейронов неверное");

            Assert.AreEqual(3, nl.neuros[0].Length, "Число синапсов у первого нейрона неверное");
            Assert.AreEqual(3, nl.neuros[1].Length, "Число синапсов у второго нейрона неверное");
            Assert.AreEqual(2, nl.Output.Length, "Размерность выхода неверная");

            new Thread(() => nl.Dispose()).Start();

            nl = new NeuronLayer(2, 4, true, 0, "no");

            Assert.AreEqual(nl.neuros.Length, 2, "Число нейронов неверное");

            Assert.AreEqual(4, nl.neuros[0].Length, "Число синапсов у первого нейрона неверное");
            Assert.AreEqual(4, nl.neuros[1].Length, "Число синапсов у второго нейрона неверное");

            Assert.AreEqual(3, nl.Output.Length, "Размерность выхода неверная");

            new Thread(() => nl.Dispose()).Start();
        }

        [TestMethod]
        public void Test_LayerCalcDer()
        {
            Vector v = new Vector(4);
            int[][] inp = new int[2][];
            inp[0] = new int[] { 0, 2 };
            inp[1] = new int[] { 1, 2, 3 };

            v[0] = 1;
            v[1] = 2;
            v[2] = 3;
            v[3] = 4;

            NeuronLayer nl = new NeuronLayer(2, inp, false, 0, "no");
            new Thread(() => nl.Input = v).InMTA();
            
            nl.neuros[0].weight[0] = 0.5;
            nl.neuros[0].weight[1] = 1.5;

            nl.neuros[1].weight[0] = 1.0;
            nl.neuros[1].weight[1] = 2.0;
            nl.neuros[1].weight[2] = 3.0;

            Vector ans = nl.CalcDer();

            Assert.AreEqual(ans[0], 1, "ошибка в 1 элементе");
            Assert.AreEqual(ans[1], 1, "ошибка во 2 элементе");
        }

        [TestMethod]
        public void Test_LayerClone()
        {
            const int n = 3;
            const int m = 2;

            NeuronLayer nl = new NeuronLayer(n, m, false, 0, "sigmoid", 0.5);

            nl.NormalInitialize();

            new Thread(() => nl.Input = new Vector(m, j => j + 1)).InMTA();

            Vector t1 = null;
            Vector t2 = null;

            new Thread(() => t1 = nl.Calc()).InMTA();

            NeuronLayer newNl = nl.CloneOk();

            for(int i = 0; i<nl.neuros.Length; i++)
                for (int j = 0; j < nl.neuros[i].Length; j++)
                {
                    Assert.AreEqual(nl.neuros[i].weight[j], newNl.neuros[i].weight[j], "Весы неверно скопированны");
                }

            new Thread(() => t2 = newNl.Calc()).InMTA();

            for (int i = 0; i < t1.Length; i++)
                Assert.AreEqual(t1[i], t2[i], "Неверно вычисляет после клонирования");

            nl.NormalInitialize();

            new Thread(() => t1 = nl.Calc()).InMTA();

            for (int i = 0; i < t1.Length; i++)
                Assert.AreNotEqual(t2[i], t1[i], "Вычисления сломались");

            new Thread(()=> nl.Dispose()).InMTA();

            t2 = null;

            new Thread(() => t2 = newNl.Calc()).InMTA();

            Assert.AreNotEqual(null, t2, "После высвобождение оригинала сломался потомок");

            for (int i = 0; i < t1.Length; i++)
                Assert.AreNotEqual(t1[i], t2[i], "Вычисления сломались");
        }
    }
}