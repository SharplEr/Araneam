using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IODate;
using VectorSpace;

namespace UnitTestIODate
{
    [TestClass]
    public class UnitTestStatist
    {
        [TestMethod]
        public void TestStatistGetRandomIndex()
        {
            Random rnd = new Random();
            const int n = 13;
            int[] indexes = Statist.getRandomIndex(n, rnd);

            Assert.IsTrue(indexes.Length == n, "Длина массива" + ((indexes.Length > n) ? "больше" : "меньше") + "чем надо");

            bool[] count = new bool[n];

            for (int i = 0; i < n; i++)
                count[i] = false;

            try
            {
                for (int i = 0; i < n; i++)
                    count[indexes[i]] = !count[indexes[i]];
            }
            catch (IndexOutOfRangeException)
            {
                Assert.Fail("Выход за пределы индексации");
            }

            bool flag = true;

            for (int i = 0; i < n; i++)
                flag = flag && count[i];

            Assert.IsTrue(flag, "Повторы номеров");
        }

        [TestMethod]
        public void TestStatistNormalization()
        {

            Vector[] matrix = new Vector[2];
            matrix[0] = new Vector(2);
            matrix[0][0] = 1;
            matrix[0][1] = 2;
            matrix[1] = new Vector(2);
            matrix[1][0] = -1;
            matrix[1][1] = 4;
            Action<Vector> f = matrix.Normalization(1.0);

            Assert.AreEqual(matrix[0][0], 1.0, "Не нормализует массив");
            Assert.AreEqual(matrix[0][1], -1.0, "Не нормализует массив");
            Assert.AreEqual(matrix[1][0], -1.0, "Не нормализует массив");
            Assert.AreEqual(matrix[1][1], 1.0, "Не нормализует массив");

            Vector some = new Vector(2);
            some[0] = 0.5;
            some[1] = 2.5;
            f(some);
            Assert.IsTrue((some[0] == 0.5) && (some[1] == -0.5), "Возвращает функцию которая, не нормализует");
        }
    }
}
