using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorSpace;
using Araneam;
using IOData;

namespace UnitTestNetwork
{
    [TestClass]
    public class UnitTestNetworkWithPlugins
    {
        public class plg : Plug<Tuple<int, double>>
        {
            public plg()
            {
                dimension = 1;
            }

            public override Vector Calc(Tuple<int, double> input)
            {
                return new Vector(1, (i) => 0.5/*Math.Pow(Math.Sin(input1.Item2), input1.Item1)*/);
            }
        }

        [TestMethod]
        public void TestMethod_LearnBPNWP()
        {
            
            plg p = new plg();
            double a = 1.7159, b = 2.0 / 3.0;

            Vector[] input = new Vector[10];

            input[0] = new Vector(2);
            input[1] = new Vector(2);
            input[2] = new Vector(2);
            input[3] = new Vector(2);
            input[4] = new Vector(2);
            input[5] = new Vector(2);
            input[6] = new Vector(2);
            input[7] = new Vector(2);
            input[8] = new Vector(2);
            input[9] = new Vector(2);

            input[0][0] = 1;
            input[1][0] = 1;
            input[2][0] = 1;
            input[3][0] = 0;
            input[4][0] = 0;
            input[5][0] = 0;
            input[6][0] = 0;
            input[7][0] = -1;
            input[8][0] = -1;
            input[9][0] = -1;

            input[0][1] = 0.2;
            input[1][1] = 0.2;
            input[2][1] = 0.3;
            input[3][1] = 0.4;
            input[4][1] = 0.5;
            input[5][1] = 0.7;
            input[6][1] = 0.6;
            input[7][1] = 0.1;
            input[8][1] = 0.7;
            input[9][1] = 0.5;
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
            Tuple<int, double>[] yetInput = new Tuple<int,double>[10];

            yetInput[0] = new Tuple<int, double>(1, 0.1257);
            yetInput[1] = new Tuple<int, double>(2, 0.1857);
            yetInput[2] = new Tuple<int, double>(2, 0.1297);
            yetInput[3] = new Tuple<int, double>(2, 0.4257);
            yetInput[4] = new Tuple<int, double>(3, 0.127);
            yetInput[5] = new Tuple<int, double>(3, 0.125);
            yetInput[6] = new Tuple<int, double>(3, 0.157);
            yetInput[7] = new Tuple<int, double>(4, 0.1257);
            yetInput[8] = new Tuple<int, double>(3, 0.1);
            yetInput[9] = new Tuple<int, double>(1, 0.157);

            Vector[] ans = new Vector[10];

            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = new Vector(1);
                ans[i][0] = input[i][0] * Math.Cos(input[i][1]);
                //ans[i][1] = Math.Pow(Math.Sin(yetInput[i].Item2), yetInput[i].Item1);
            }

            BackPropagationNetworkWithPlugs<Tuple<int, double>> network = new BackPropagationNetworkWithPlugs<Tuple<int, double>>(new Plug<Tuple<int, double>>[] { p }, 0.1, 5500,
                new int[] { 4, 1}, 2, "tanh", a, b);

            network.AddTestDate(input, yetInput, ans, new int[] { 5, 5 });

            for (int i = 0; i < 20000; i++)
                network.Learn(input[i % 10], yetInput[i % 10], ans[i % 10]);

            Vector[] aaa = network.Calculation(input, yetInput);

            for (int i = 0; i < ans.Length; i++)
            {
                Assert.AreEqual(ans[i][0], aaa[i][0], 0.001, "жопа {0}", i);
                Assert.AreEqual(ans[i][1], aaa[i][1], 0.0000001, "супер хрень {0}", i);
            }
        }

        [TestMethod]
        public void TestMethod_Learn2()
        {
            double a = 1.7159, b = 2.0 / 3.0;

            Vector[] input = new Vector[10];

            input[0] = new Vector(3);
            input[1] = new Vector(3);
            input[2] = new Vector(3);
            input[3] = new Vector(3);
            input[4] = new Vector(3);
            input[5] = new Vector(3);
            input[6] = new Vector(3);
            input[7] = new Vector(3);
            input[8] = new Vector(3);
            input[9] = new Vector(3);

            input[0][0] = 1;
            input[1][0] = 1;
            input[2][0] = 1;
            input[3][0] = 0;
            input[4][0] = 0;
            input[5][0] = 0;
            input[6][0] = 0;
            input[7][0] = -1;
            input[8][0] = -1;
            input[9][0] = -1;

            input[0][1] = 0.2;
            input[1][1] = 0.2;
            input[2][1] = 0.3;
            input[3][1] = 0.4;
            input[4][1] = 0.5;
            input[5][1] = 0.7;
            input[6][1] = 0.6;
            input[7][1] = 0.1;
            input[8][1] = 0.7;
            input[9][1] = 0.5;

            input[0][2] = 0.5;
            input[1][2] = 0.5;
            input[2][2] = 0.5;
            input[3][2] = 0.5;
            input[4][2] = 0.5;
            input[5][2] = 0.5;
            input[6][2] = 0.5;
            input[7][2] = 0.5;
            input[8][2] = 0.5;
            input[9][2] = 0.5;

            Vector[] ans = new Vector[10];

            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = new Vector(1);
                ans[i][0] = input[i][0] * Math.Cos(input[i][1]);
            }

            BackPropagationNetwork network = new BackPropagationNetwork(0.1, 5500,
                new int[] { 4, 1 }, 3, "tanh", a, b);

            network.AddTestDate(input, ans, new int[] { 5, 5 });
            //network.NewLearn(false, 100);
            for (int i = 0; i < 20000; i++)
                network.Learn(input[i % 10], ans[i % 10]);

            Vector[] aaa = network.Calculation(input);

            for (int i = 0; i < ans.Length; i++)
            {
                Assert.AreEqual(ans[i][0], aaa[i][0], 0.05, "жопа {0}", i);
                //Assert.AreEqual(ans[i][1], aaa[i][1], 0.0000001, "супер хрень {0}", i);
            }
        }
    }
}
