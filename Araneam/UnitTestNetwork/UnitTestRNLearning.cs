using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;
using IOData;
using VectorSpace;

namespace UnitTestNetwork
{
    [TestClass]
    public class UnitTestRNLearning
    {
        [TestMethod]
        public void Test_RNLearningNode_Intersection()
        {
            int[] a = new int[] { 1, 6, 8, 10 };
            int[] b = new int[] { 1, 2, 3, 6, 7, 10 };

            var c = RNLearningNode.Intersection(a, b);

            Assert.AreEqual(3, c.Length, "неверная длина 1");
            Assert.AreEqual(1, c[0], "неверное значение 1");
            Assert.AreEqual(6, c[1], "неверное значение 1");
            Assert.AreEqual(10, c[2], "неверное значение 1");

            a = new int[] { 4, 5, 8, 9 };
            b = new int[] { 1, 2, 3, 6, 7, 10 };

            c = RNLearningNode.Intersection(a, b);

            Assert.AreEqual(0, c.Length, "неверная длина 2");

            a = new int[] { 0, 1, 2 };
            b = new int[] { 0, 1, 2 };

            c = RNLearningNode.Intersection(a, b);

            Assert.AreEqual(3, c.Length, "неверная длина 3");
            Assert.AreEqual(0, c[0], "неверное значение 3");
            Assert.AreEqual(1, c[1], "неверное значение 3");
            Assert.AreEqual(2, c[2], "неверное значение 3");
        }

        [TestMethod]
        public void Test_RNLearningNode_Union()
        {
            Tuple<int, int>[] a = new Tuple<int, int>[] {new Tuple<int, int>(0,1),
            new Tuple<int, int>(1,2), new Tuple<int, int>(2,4)};

            Tuple<int, int>[] b = new Tuple<int, int>[] {new Tuple<int, int>(1,2),
            new Tuple<int, int>(2,4), new Tuple<int, int>(5,1)};

            var c = RNLearningNode.Union(a, b);

            Assert.AreNotEqual(null, c, "Неверное определение совместимости");

            Assert.AreEqual(4, c.Length, "Неверная длина");

            Assert.AreEqual(0, c[0].Item1);
            Assert.AreEqual(1, c[1].Item1);
            Assert.AreEqual(2, c[2].Item1);
            Assert.AreEqual(5, c[3].Item1);

            Assert.AreEqual(1, c[0].Item2);
            Assert.AreEqual(2, c[1].Item2);
            Assert.AreEqual(4, c[2].Item2);
            Assert.AreEqual(1, c[3].Item2);

            a = new Tuple<int, int>[] {new Tuple<int, int>(0,3)};

            b = new Tuple<int, int>[] {new Tuple<int, int>(1,2)};

            c = RNLearningNode.Union(a, b);

            Assert.AreNotEqual(null, c, "Неверное определение совместимости");

            Assert.AreEqual(2, c.Length, "Неверная длина");

            Assert.AreEqual(0, c[0].Item1);
            Assert.AreEqual(1, c[1].Item1);

            Assert.AreEqual(3, c[0].Item2);
            Assert.AreEqual(2, c[1].Item2);

            a = new Tuple<int, int>[] { new Tuple<int, int>(0, 3) };

            b = new Tuple<int, int>[] { new Tuple<int, int>(0, 2) };

            c = RNLearningNode.Union(a, b);

            Assert.AreEqual(null, c, "Неверное определение совместимости");


            a = new Tuple<int, int>[] {new Tuple<int, int>(0,1),
            new Tuple<int, int>(1,3), new Tuple<int, int>(2,4)};

            b = new Tuple<int, int>[] {new Tuple<int, int>(1,2),
            new Tuple<int, int>(2,4), new Tuple<int, int>(5,1)};

            c = RNLearningNode.Union(a, b);

            Assert.AreEqual(null, c, "Неверное определение совместимости");
        }

        [TestMethod]
        public void Test_RNLearningNode_Learn()
        {
            RNNetwork network = new RNNetwork();

            //double x = 0.7;
            //int s = 3;
            int[] maxs = new int[] { 2, 2, 2, 2 };

            MixData[] data = new MixData[10];

            data[0] = new MixData(null, new int[] { 1, 0, 0, 0 });
            data[1] = new MixData(null, new int[] { 0, 1, 0, 0 });
            data[2] = new MixData(null, new int[] { 0, 0, 1, 0 });
            data[3] = new MixData(null, new int[] { 0, 0, 0, 1 });
            data[4] = new MixData(null, new int[] { 1, 1, 0, 0 });
            data[5] = new MixData(null, new int[] { 0, 1, 1, 0 });
            data[6] = new MixData(null, new int[] { 0, 0, 1, 1 });
            data[7] = new MixData(null, new int[] { 0, 1, 0, 1 });
            data[8] = new MixData(null, new int[] { 1, 0, 1, 1 });
            data[9] = new MixData(null, new int[] { 1, 1, 1, 1 });

            int[] res = new int[] {1, 0, 0, 1, 1, 0, 1, 1, 1, 1 };

            Results results = new Results((i)=> new Result(res[i], 2), res.Length);

            //network.Learning(data, results, maxs, s, x);
        }
    }
}
