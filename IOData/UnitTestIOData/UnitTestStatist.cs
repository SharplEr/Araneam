using System;
using IOData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorSpace;

namespace UnitTestIODate
{
    [TestClass]
    public class UnitTestStatist
    {
        [TestMethod]
        public void Test_Statist_GetRandomIndex()
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
        public void Test_Statist_Normalization()
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

        [TestMethod]
        public void Test_Statist_Correlation()
        {
            double[] x = new double[] {1, 2, 3, 4, 5 };
            double[] y = new double[] { 2, 4, 6, 8, 10 };

            Assert.AreEqual(1, Statist.Correlation(x, y));
        }

        [TestMethod]
        public void Test_Statist_ExactDifference()
        {
            double x = Statist.ExactDifference(1, 1, 2, 1.5);
            Assert.AreEqual(0.0, x, "Не находит нулевое расстояние");
            x = Statist.ExactDifference(2, 1.5, 1, 1);
            Assert.AreEqual(0.0, x, "Не работает нулевое расстояние в обратную сторону");

            x = Statist.ExactDifference(1, Math.Sqrt(7), 10, Math.Sqrt(9));
            Assert.AreEqual(-5.0, x, 0.0001,"Неверно находит расстояние");

            x = Statist.ExactDifference(10, Math.Sqrt(7), 1, Math.Sqrt(9));
            Assert.AreEqual(5.0, x, 0.0001, "Неверно находит расстояние в обратную сторону");

            x = Statist.ExactDifference(1, Math.Sqrt(7), 100, Double.NaN);
            Assert.IsTrue(x>0, "Неверно определяет предпочтения-1");

            x = Statist.ExactDifference(100, Double.NaN, 1, 2);
            Assert.IsTrue(x < 0, "Неверно определяет предпочтения-2");

            x = Statist.ExactDifference(Double.PositiveInfinity, Double.NaN, Double.NegativeInfinity, 2);
            Assert.IsTrue(x == 0.0, "Не сравнивает два худших результата");
        }
    }
}
