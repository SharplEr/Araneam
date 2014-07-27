using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;
using VectorSpace;
using System.Threading;
using MyParallel;
using ArrayHelper;

namespace UnitTestNetwork
{    
    [TestClass]
    public class UnitTestBP
    {
        class BPNW1 : BackPropagationNetwork
        {
            public BPNW1()
                : base(0.9, 1000, 2)
            {
                layers[0] = new NeuronLayer(2, 2, false, 0, "no");

                layers[1] = new NeuronLayer(1, 2, false, 0, "no");
                layers[1].CalcInvers(layers[0].WithThreshold);
            }

            public Vector[] LG(Vector e)
            {
                setLocalGrads(e);
                return LocalGrads;
            }
        }

        [TestMethod]
        public void Test_BackPropagationLocalGrad()
        {
            BPNW1 nw = new BPNW1();
            
            Vector x = new Vector(2);
            x[0] = 0;
            x[1] = 1;

            Vector d = new Vector(1);
            d[0] = 1;

            Vector e = null;
            Vector[] lg = null;
            new Thread(() =>
            {
                e =  d-nw.Calculation(x);
                lg = nw.LG(e);
            }).InMTA();

            Assert.AreEqual(1, e[0]);

            Assert.AreEqual(2, lg[0].Length);
            Assert.AreEqual(0, lg[0][0]);
            Assert.AreEqual(0, lg[0][1]);
            Assert.AreEqual(1, lg[1].Length);
            Assert.AreEqual(1, lg[1][0]);
        }

        class BPNW2 : BackPropagationNetwork
        {
            public BPNW2()
                : base(0.5, 100, 2)
            {

                layers[0] = new NeuronLayer(2, 3, true, 0, "no");
                layers[1] = new NeuronLayer(1, 3, false, 0, "no");
                
                layers[0].NormalInitialize();

                layers[1].CalcInvers(layers[0].WithThreshold);
                layers[1].NormalInitialize();
            }

            public Vector[] LG(Vector e)
            {
                setLocalGrads(e);
                return LocalGrads;
            }
        }

        [TestMethod]
        public void Test_BackPropagationXORLearn()
        {
            BPNW2 nw = new BPNW2();

            Vector[] x = new Vector[4];
            Vector[] d = new Vector[4];

            x[0] = new Vector(3);
            x[0][0] = 0;
            x[0][1] = 0;
            x[0][2] = 1;
            x[1] = new Vector(3);
            x[1][0] = 0;
            x[1][1] = 1;
            x[1][2] = 1;
            x[2] = new Vector(3);
            x[2][0] = 1;
            x[2][1] = 0;
            x[2][2] = 1;
            x[3] = new Vector(3);
            x[3][0] = 1;
            x[3][1] = 1;
            x[3][2] = 1;

            d[0] = new Vector(1);
            d[0][0] = 0;
            d[1] = new Vector(1);
            d[1][0] = 1;
            d[2] = new Vector(1);
            d[2][0] = 1;
            d[3] = new Vector(1);
            d[3][0] = 0;

            Vector[] y = new Vector[4];

            new Thread(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        nw.Learn(x[i % 4], d[i % 4]);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        y[i] = nw.Calculation(x[i]).CloneOk();
                    }

                    nw.Dispose();
                }).InMTA();

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(d[i][0], y[i][0], 0.001, "Сеть не обучается. Пример с ошибкой {0}", i);
            }
        }

        class BPNW3 : BackPropagationNetwork
        {
            public BPNW3()
                : base(0.5, 100, 2)
            {

                layers[0] = new NeuronLayer(2, 3, true, 0, "tanh", 1.7159, 2.0/3.0);
                layers[1] = new NeuronLayer(3, 3, false, 0, "tanh", 1.7159, 2.0 / 3.0);

                layers[1].CalcInvers(layers[0].WithThreshold);

                layers[0].neuros[0].weight[0] = 1.0 / 2.0;
                layers[0].neuros[0].weight[1] = 1.0 / 3.0;
                layers[0].neuros[0].weight[2] = 1.0 / 4.0;
                layers[0].neuros[1].weight[0] = 1.0 / 5.0;
                layers[0].neuros[1].weight[1] = 1.0 / 6.0;
                layers[0].neuros[1].weight[2] = 1.0 / 7.0;

                layers[1].neuros[0].weight[0] = 1.0 / 8.0;
                layers[1].neuros[0].weight[1] = 1.0 / 9.0;
                layers[1].neuros[0].weight[2] = 1.0 / 10.0;
                layers[1].neuros[1].weight[0] = 1.0 / 11.0;
                layers[1].neuros[1].weight[1] = 1.0 / 12.0;
                layers[1].neuros[1].weight[2] = 1.0 / 13.0;
                layers[1].neuros[2].weight[0] = 1.0 / 14.0;
                layers[1].neuros[2].weight[1] = 1.0 / 15.0;
                layers[1].neuros[2].weight[2] = 1.0 / 16.0;
            }

            public Vector[] LG(Vector e)
            {
                setLocalGrads(e);
                return LocalGrads;
            }

            public Vector[] Der()
            {
                Vector[] v = new Vector[layers.Length];
                for (int i = 0; i < v.Length; i++)
                    v[i] = layers[i].CalcDer();
                return v;
            }
        }

        [TestMethod]
        public void Test_BackPropagationWork()
        {
            BPNW3 nw = new BPNW3();
            Vector x = new Vector(3);
            x[0] = 1;
            x[1] = 0.5;
            x[2] = -1;
            Vector d = new Vector(3);
            d[0] = 0.5;
            d[1] = 0.4;
            d[2] = 0.3;

            Vector y = null;

            new Thread(() => y = nw.Calculation(x)).InMTA();

            Assert.AreEqual(0.1437,y[0], 0.0002);
            Assert.AreEqual(0.1075, y[1], 0.0002);
            Assert.AreEqual(0.08586, y[2], 0.0002);

            Vector[] der = nw.Der();

            Assert.AreEqual(2, der.Length);
            Assert.AreEqual(2, der[0].Length);
            Assert.AreEqual(3, der[1].Length);

            Assert.AreEqual(1.059997, der[0][0], 0.0001);
            Assert.AreEqual(1.1339, der[0][1], 0.0001);

            Assert.AreEqual(1.1358, der[1][0], 0.001);
            Assert.AreEqual(1.13937, der[1][1], 0.001);
            Assert.AreEqual(1.141, der[1][2], 0.001);

            Vector e = d - y;

            Vector[] lg = nw.LG(e);

            Assert.AreEqual(2, lg.Length);
            Assert.AreEqual(2, lg[0].Length);
            Assert.AreEqual(3, lg[1].Length);

            Assert.AreEqual(0.10423, lg[0][0], 0.001);
            Assert.AreEqual(0.10095, lg[0][1], 0.001);
            Assert.AreEqual(0.4047, lg[1][0], 0.001);
            Assert.AreEqual(0.3333, lg[1][1], 0.001);
            Assert.AreEqual(0.2443, lg[1][2], 0.001);   
        }
    }
}