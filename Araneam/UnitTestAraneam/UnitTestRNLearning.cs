using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;

namespace UnitTestAraneam
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

            Assert.AreEqual(3, c.Length, "неверная длина");
            Assert.AreEqual(1, c[0], "неверная длина");
            Assert.AreEqual(6, c[1], "неверная длина");
            Assert.AreEqual(10, c[2], "неверная длина");
        }
    }
}
