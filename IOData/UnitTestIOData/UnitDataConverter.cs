using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IOData;

namespace UnitTestIODate
{
    [TestClass]
    public class UnitDateConverter
    {
        [TestMethod]
        public void Test_DataConverter_DiscreteToСontinuous()
        {
            string[] input = new string[] { "A", "B", "A", "A", "B", "C", "C", "C", "A", "C" };
            double[] output = new double[] { 1,   1,   1,  -1,  -1,  -1,  -1,   1,   1,  -1};
            double[] right = new double[] { 0.5, 0, 0.5, 0.5, 0, -0.5, -0.5, -0.5, 0.5, -0.5 };
            Func<string, double> f = DataConverter.DiscreteToСontinuous(input, output);

            double[] result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = f(input[i]);

            double corr = Statist.Correlation(result, output);

            if (corr < 0) Assert.Fail("Даже направление неверно");
            if (corr<0.4472135954999579) Assert.Fail("Корреляция меньше, чем при нумерации");

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(right[i], result[i], "Найдено различие в преобразовании на шаге {0}", i);
        }
        [TestMethod]
        public void Test_DataConverter_СontinuousToDiscrete()
        {
            double[] input = new double[] { 0.25, 0.6, 0.5, 0.7, 0.1, 0.9 };
            
            int[] right = new int[] { 0,    1,   0,   1,   0,   1 };
            Func<double, int> f = DataConverter.СontinuousToDiscrete(input, 3);

            double[] result = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = f(input[i]);

            for (int i = 0; i < input.Length; i++)
                Assert.AreEqual(right[i], result[i], "Найдено различие в преобразовании на шаге {0}", i);
        }
    }

    
}
