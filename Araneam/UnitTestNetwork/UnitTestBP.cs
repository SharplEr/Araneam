﻿using System;
using System.Threading;
using Araneam;
using ArrayHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyParallel;
using VectorSpace;

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

            Vector e = default(Vector);
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
                : base(0.5, 1000, 2)
            {
                layers[0] = new NeuronLayer(2, 3, true, 0, "no");
                layers[1] = new NeuronLayer(1, 3, false, 0, "no");
                
                layers[0].NormalInitialize(random);

                layers[1].CalcInvers(layers[0].WithThreshold);
                layers[1].NormalInitialize(random);
            }
        }

        [TestMethod]
        public void Test_BackPropagationEaseLearn()
        {
            BPNW2 nw = new BPNW2();

            const int m = 2;

            Vector[] x = new Vector[m];
            Vector[] d = new Vector[m];

            x[0] = new Vector(3);
            x[0][0] = -1;
            x[0][1] = -1;
            x[0][2] = 1;
            x[1] = new Vector(3);
            x[1][0] = 1;
            x[1][1] = 1;
            x[1][2] = 1;

            d[0] = new Vector(1);
            d[0][0] = -1;
            d[1] = new Vector(1);
            d[1][0] = 1;

            Vector[] y = new Vector[m];

            new Thread(() =>
            {
                for (int i = 0; i < m * 35; i++)
                {
                    nw.Learn(x[i % m], d[i % m]);
                }

                for (int i = 0; i < m; i++)
                {
                    y[i] = nw.Calculation(x[i]).CloneOk();
                }

                nw.Dispose();
            }).InMTA();

            for (int i = 0; i < m; i++)
            {
                Assert.AreEqual(d[i][0], y[i][0], 0.02, "Сеть не обучается. Пример с ошибкой {0}", i);
            }
        }

        [TestMethod]
        public void Test_BackPropagationEaseLearn2()
        {
            BPNW2 nw = new BPNW2();

            const int m = 3;

            Vector[] x = new Vector[m];
            Vector[] d = new Vector[m];

            x[0] = new Vector(3);
            x[0][0] = 0;
            x[0][1] = 0;
            x[0][2] = 1;
            x[1] = new Vector(3);
            x[1][0] = 1;
            x[1][1] = 1;
            x[1][2] = 1;
            x[2] = new Vector(3);
            x[2][0] = 1;
            x[2][1] = 0;
            x[2][2] = 1;

            d[0] = new Vector(1);
            d[0][0] = -1;
            d[1] = new Vector(1);
            d[1][0] = 1;
            d[2] = new Vector(1);
            d[2][0] = 0;

            Vector[] y = new Vector[m];

            new Thread(() =>
            {
                for (int i = 0; i < m * 25; i++)
                {
                    nw.Learn(x[i % m], d[i % m]);
                }

                for (int i = 0; i < m; i++)
                {
                    y[i] = nw.Calculation(x[i]).CloneOk();
                }

                nw.Dispose();
            }).InMTA();

            for (int i = 0; i < m; i++)
            {
                Assert.AreEqual(d[i][0], y[i][0], 0.01, "Сеть не обучается. Пример с ошибкой {0}", i);
            }
        }

        [TestMethod]
        public void Test_BackPropagationLinearLearn()
        {
            Random r = new Random();

            BPNW2 nw = new BPNW2();

            const int m = 10;

            Vector[] y = new Vector[m];

            new Thread(() =>
            {
                for (int i = 0; i < m * 100; i++)
                {
                    Vector x = new Vector(3, (j)=>r.NextDouble(), 1);
                    Vector d = new Vector(1);
                    d[0] = x[0] + x[1];
                    nw.Learn(x, d);
                }

                for (int i = 0; i < m; i++)
                {
                    Vector x = new Vector(3, (j) => r.NextDouble(), 1);
                    Vector d = new Vector(1);
                    d[0] = x[0] + x[1];
                    y[i] = d-nw.Calculation(x);
                }

                nw.Dispose();
            }).InMTA();

            for (int i = 0; i < m; i++)
            {
                Assert.AreEqual(0, y[i][0], 0.01, "Сеть не обучается. Пример с ошибкой {0}", i);
            }
        }

        [TestMethod]
        public void Test_BackPropagationLinearLearn2()
        {
            Random r = new Random();

            BPNW2 nw = new BPNW2();

            const int m = 10;

            Vector[] y = new Vector[m];

            new Thread(() =>
            {
                for (int i = 0; i < m * 100; i++)
                {
                    Vector x = new Vector(3, (j) => r.NextDouble()*2-1, 1);
                    Vector d = new Vector(1);
                    d[0] = (x[0] * Math.PI + x[1]*Math.E) / 3.0;
                    nw.Learn(x, d);
                }

                for (int i = 0; i < m; i++)
                {
                    Vector x = new Vector(3, (j) => r.NextDouble()*2-1, 1);
                    Vector d = new Vector(1);
                    d[0] = (x[0] * Math.PI + x[1] * Math.E) / 3.0;
                    y[i] = d - nw.Calculation(x);
                }

                nw.Dispose();
            }).InMTA();

            for (int i = 0; i < m; i++)
            {
                Assert.AreEqual(0, y[i][0], 0.01, "Сеть не обучается. Пример с ошибкой {0}", i);
            }
        }

        class BPNW4 : BackPropagationNetwork
        {
            public BPNW4()
                : base(0.5, 5000, 4)
            {
                layers[0] = new NeuronLayer(10, 3, true, 0, "tanh", 1.7159, 2.0 / 3.0);
                layers[1] = new NeuronLayer(5, 11, true, 0, "tanh", 1.7159, 2.0 / 3.0);
                layers[2] = new NeuronLayer(3, 6, true, 0, "tanh", 1.7159, 2.0 / 3.0);
                layers[3] = new NeuronLayer(1, 4, false, 0, "tanh", 1.7159, 2.0 / 3.0);

                layers[0].NormalInitialize(random);
                layers[1].NormalInitialize(random);
                layers[2].NormalInitialize(random);
                layers[3].NormalInitialize(random);

                layers[1].CalcInvers(layers[0].WithThreshold);
                layers[2].CalcInvers(layers[1].WithThreshold);
                layers[3].CalcInvers(layers[2].WithThreshold);
            }
        }

        [TestMethod]
        public void Test_BackPropagationNonLinearLearn()
        {
            Random r = new Random();

            BPNW4 nw = new BPNW4();

            const int m = 10;

            Vector[] y = new Vector[m];


            double avgError = 0.0;
            new Thread(() =>
            {
                for (int i = 0; i < m * 10000; i++)
                {
                    Vector x = new Vector(3, (j) => r.NextDouble() * 2 - 1, 1);
                    Vector d = new Vector(1);
                    d[0] = (Math.Sin(x[0]) + Math.Cos(x[1])) / Math.Sqrt(2);
                    nw.Learn(x, d, true);
                }

                for (int i = 0; i < m; i++)
                {
                    Vector x = new Vector(3, (j) => r.NextDouble() * 2 - 1, 1);
                    Vector d = new Vector(1);
                    d[0] = (Math.Sin(x[0]) + Math.Cos(x[1]))/Math.Sqrt(2);
                    y[i] = d - nw.Calculation(x);
                    avgError += Math.Abs(y[i][0]);
                }

                avgError /= m;

                nw.Dispose();
            }).InMTA();

                Assert.AreEqual(0, avgError, 0.12, "Сеть не обучается. Cредняя ошибка: {0}", avgError);
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

            Vector y = default(Vector);

            new Thread(() => y = nw.Calculation(x)).InMTA();

            Assert.AreEqual(0.1247,y[0], 0.0002);
            Assert.AreEqual(0.0928, y[1], 0.0002);
            Assert.AreEqual(0.0739, y[2], 0.0002);

            Vector[] der = nw.Der();

            Assert.AreEqual(2, der.Length);
            Assert.AreEqual(2, der[0].Length);
            Assert.AreEqual(3, der[1].Length);

            Assert.AreEqual(1.06, der[0][0], 0.0001);
            Assert.AreEqual(1.1339, der[0][1], 0.0001);

            Assert.AreEqual(1.1379, der[1][0], 0.001);
            Assert.AreEqual(1.1406, der[1][1], 0.001);
            Assert.AreEqual(1.1418, der[1][2], 0.001);

            Vector e = d - y;

            Vector[] lg = nw.LG(e);

            Assert.AreEqual(2, lg.Length);
            Assert.AreEqual(2, lg[0].Length);
            Assert.AreEqual(3, lg[1].Length);

            Assert.AreEqual(0.10988, lg[0][0], 0.001);
            Assert.AreEqual(0.10641, lg[0][1], 0.001);
            Assert.AreEqual(0.4270, lg[1][0], 0.001);
            Assert.AreEqual(0.3503, lg[1][1], 0.001);
            Assert.AreEqual(0.2581, lg[1][2], 0.001);   
        }
    }
}