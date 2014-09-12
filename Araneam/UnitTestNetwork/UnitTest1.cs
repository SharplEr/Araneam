using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;
using VectorSpace;
using IOData;

namespace UnitTestNetwork
{
    [TestClass]
    public class UnitTest1
    {
        public class plgConst : Plug<object>
        {
            public plgConst()
            {
                dimension = 1;
            }

            public override Vector Calc(object input)
            {
                return new Vector(1, (i) => 0.5);
            }
        }

        public class OpenNetWork : BackPropagationNetwork
        {
            public OpenNetWork(double r, double t, int[] LayerCounts, int inputDem, string name, params double[] k) :
                base(r, t, LayerCounts, inputDem, name, k)
            { }

            public NeuronLayer[] getLayer()
            {
                return layers;
            }
        }

        public class NetworkP : BackPropagationNetworkWithPlugs<object>
        {
            public NetworkP(Plug<object>[] plugs, double r, double t, int[] LayerCounts, int inputDem, string name, params double[] k) :
                base(plugs, r, t, LayerCounts, inputDem, name, k)
            { }

            public bool Check1(OpenNetWork network)
            {
                NeuronLayer[] l = network.getLayer();

                if (l.Length != layers.Length)
                    return false;

                for (int i = 0; i < l.Length; i++)
                {
                    if (l[i].InputIndex.Length != layers[i].InputIndex.Length)
                        return false;
                    for (int j = 0; j < l[i].InputIndex.Length; j++)
                    {
                        if (l[i].InputIndex[j].Length != layers[i].InputIndex[j].Length)
                            return false;
                        for (int k = 0; k < l[i].InputIndex[j].Length; k++ )
                            if (l[i].InputIndex[j][k] != layers[i].InputIndex[j][k])
                                return false;
                    }
                }
                for (int i = 1; i < l.Length; i++)
                {
                    if (l[i].InversIndex.Length != layers[i].InversIndex.Length)
                        return false;

                    for (int j = 0; j < l[i].InversIndex.Length; j++)
                    {
                        if (l[i].InversIndex[j].index.Length != layers[i].InversIndex[j].index.Length)
                            return false;

                        if (l[i].InversIndex[j].subIndex.Length != layers[i].InversIndex[j].subIndex.Length)
                            return false;

                        if (l[i].InversIndex[j].subIndex.Length != l[i].InversIndex[j].index.Length)
                            return false;

                        for (int k = 0; k < l[i].InversIndex[j].subIndex.Length; k++)
                        {
                            if (l[i].InversIndex[j].index[k] != layers[i].InversIndex[j].index[k])
                                return false;

                            if (l[i].InversIndex[j].subIndex[k] != layers[i].InversIndex[j].subIndex[k])
                                return false;
                        }
                    }

                }

                return true;
            }
            public bool Check2(OpenNetWork network)
            {
                NeuronLayer[] l = network.getLayer();

                if (l.Length != layers.Length)
                    return false;

                for (int i = 0; i < l.Length; i++)
                {
                    if (l[i].neuros.Length != layers[i].neuros.Length)
                        return false;
                    for (int j = 0; j < l[i].neuros.Length; j++)
                    {
                        if (l[i].neuros[j].Length != layers[i].neuros[j].Length)
                            return false;
                        for (int k = 0; k < l[i].neuros[j].Length; k++)
                            if (l[i].neuros[j].weight[k] != layers[i].neuros[j].weight[k])
                                return false;
                    }
                }

                return true;
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            NetworkP n = new NetworkP(new plgConst[] { new plgConst(), new plgConst() }, 0.1, 5500, new int[] { 4, 2 }, 3, "tanh", 1.7, 2.0 / 3.0);
            OpenNetWork on = new OpenNetWork(0.1, 5500, new int[] { 4, 2 }, 4, "tanh", 1.7, 2.0 / 3.0);

            Assert.IsTrue(n.Check1(on), "Не верные индексы");

            n.Learn(new Vector(3, (i) => 0.5), new object(), new Vector(2, (i) => 0.5));

            on.Learn(new Vector(4, (i) => 0.5), new Vector(2, (i) => 0.5));

            n.Learn(new Vector(3, (i) => i/6.0), new object(), new Vector(2, (i) => 0.1));

            on.Learn(new Vector(4, (i) => i/6.0), new Vector(2, (i) => 0.1));

            Assert.IsTrue(n.Check2(on), "Не верное обучение");
        }

        [TestMethod]
        public void TestMethod_LearnBPNWP222()
        {
            double a = 1.7159, b = 2.0 / 3.0;

            Vector[] input1 = new Vector[10];

            input1[0] = new Vector(2);
            input1[1] = new Vector(2);
            input1[2] = new Vector(2);
            input1[3] = new Vector(2);
            input1[4] = new Vector(2);
            input1[5] = new Vector(2);
            input1[6] = new Vector(2);
            input1[7] = new Vector(2);
            input1[8] = new Vector(2);
            input1[9] = new Vector(2);

            input1[0][0] = 1;
            input1[1][0] = 1;
            input1[2][0] = 1;
            input1[3][0] = 0;
            input1[4][0] = 0;
            input1[5][0] = 0;
            input1[6][0] = 0;
            input1[7][0] = -1;
            input1[8][0] = -1;
            input1[9][0] = -1;

            input1[0][1] = 0.2;
            input1[1][1] = 0.2;
            input1[2][1] = 0.3;
            input1[3][1] = 0.4;
            input1[4][1] = 0.5;
            input1[5][1] = 0.7;
            input1[6][1] = 0.6;
            input1[7][1] = 0.1;
            input1[8][1] = 0.7;
            input1[9][1] = 0.5;
            /*
            input1[0][2] = 0.5;
            input1[1][2] = 0.5;
            input1[2][2] = 0.5;
            input1[3][2] = 0.5;
            input1[4][2] = 0.5;
            input1[5][2] = 0.5;
            input1[6][2] = 0.5;
            input1[7][2] = 0.5;
            input1[8][2] = 0.5;
            input1[9][2] = 0.5;
            */

            Vector[] input2 = new Vector[10];

            input2[0] = new Vector(3);
            input2[1] = new Vector(3);
            input2[2] = new Vector(3);
            input2[3] = new Vector(3);
            input2[4] = new Vector(3);
            input2[5] = new Vector(3);
            input2[6] = new Vector(3);
            input2[7] = new Vector(3);
            input2[8] = new Vector(3);
            input2[9] = new Vector(3);

            input2[0][0] = 1;
            input2[1][0] = 1;
            input2[2][0] = 1;
            input2[3][0] = 0;
            input2[4][0] = 0;
            input2[5][0] = 0;
            input2[6][0] = 0;
            input2[7][0] = -1;
            input2[8][0] = -1;
            input2[9][0] = -1;

            input2[0][1] = 0.2;
            input2[1][1] = 0.2;
            input2[2][1] = 0.3;
            input2[3][1] = 0.4;
            input2[4][1] = 0.5;
            input2[5][1] = 0.7;
            input2[6][1] = 0.6;
            input2[7][1] = 0.1;
            input2[8][1] = 0.7;
            input2[9][1] = 0.5;
            
            input2[0][2] = 0.5;
            input2[1][2] = 0.5;
            input2[2][2] = 0.5;
            input2[3][2] = 0.5;
            input2[4][2] = 0.5;
            input2[5][2] = 0.5;
            input2[6][2] = 0.5;
            input2[7][2] = 0.5;
            input2[8][2] = 0.5;
            input2[9][2] = 0.5;
            
            Vector[] ans = new Vector[10];

            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = new Vector(1);
                ans[i][0] = input1[i][0] * Math.Cos(input1[i][1]);
                //ans[i][1] = Math.Pow(Math.Sin(yetInput[i].Item2), yetInput[i].Item1);
            }

            NetworkP network = new NetworkP(new plgConst[] { new plgConst(), new plgConst() }, 0.1, 5500,
                new int[] { 4, 1 }, 2, "tanh", a, b);

            OpenNetWork opn = new OpenNetWork(0.1, 5500,
               new int[] { 4, 1 }, 3, "tanh", a, b);
            //network.AddTestDate(input1, yetInput, ans, new int[] { 5, 5 });

            Assert.IsTrue(network.Check1(opn), "говно");

            object[] o = new object[10];
            for (int i = 0; i < o.Length; i++)
                o[i] = new object();

            for (int i = 0; i < 20000; i++)
            {
                network.Learn(input1[i % 10], o[i % 10], ans[i % 10]);
                opn.Learn(input2[i % 10], ans[i % 10]);
                Assert.IsTrue(network.Check2(opn), "расхождение на итерации {0}", i);
                Vector t1 = network.Calculation(input1[i % 10], o[i % 10]);
                Vector t2 = opn.Calculation(input2[i % 10]);
                Assert.AreEqual(t1[0], t2[0], "удивительное расхождение на итерации {0}", i);
            }

            Vector[] aaa = network.Calculation(input1, o);

            Vector[] bbb = opn.Calculation(input2);

            for (int i = 0; i < ans.Length; i++)
            {
                //Assert.AreEqual(ans[i][0], aaa[i][0], 0.1, "жопа {0}", i);
                //Assert.AreEqual(ans[i][0], bbb[i][0], 0.1, "жопа другая {0}", i);
                Assert.AreEqual(aaa[i][0], bbb[i][0], 0.1, "жопа другая {0}", i);
                //Assert.AreEqual(ans[i][1], aaa[i][1], 0.0000001, "супер хрень {0}", i);
            }
        }
    }
}
