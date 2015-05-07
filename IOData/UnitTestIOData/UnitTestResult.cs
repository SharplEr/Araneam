using IOData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorSpace;

namespace UnitTestIOData
{
    [TestClass]
    public class UnitTestResult
    {
        [TestMethod]
        public void Test_Result_Properties_Spectrum()
        {
            Result r = new Result(0, 2);

            Assert.AreEqual(2, r.Spectrum.Length, "Неверая длина");
            Assert.AreEqual(1.0, r.Spectrum[0], "Неверное значение");
            Assert.AreEqual(-1.0, r.Spectrum[1], "Неверное значение");

            r = new Result(1, 2);

            Assert.AreEqual(2, r.Spectrum.Length, "Неверая длина");
            Assert.AreEqual(-1.0, r.Spectrum[0], "Неверное значение");
            Assert.AreEqual(1.0, r.Spectrum[1], "Неверное значение");

            r = new Result(7, 13);

            Assert.AreEqual(13, r.Spectrum.Length, "Неверая длина");

            for(int i = 0; i<7; i++)
                Assert.AreEqual(-1.0, r.Spectrum[i], "Неверное значение");

            Assert.AreEqual(1.0, r.Spectrum[7], "Неверное значение");

            for (int i = 8; i < 13; i++)
                Assert.AreEqual(-1.0, r.Spectrum[i], "Неверное значение");
        }

        [TestMethod]
        public void Test_Result_Properties_Number()
        {
            Result r = new Result(new Vector(2, (i) => (i != 0) ? -1.0 : 1.0));

            Assert.AreEqual(0, r.Number, "Неверное значение");

            r = new Result(new Vector(2, (i) => (i != 1) ? -1.0 : 1.0));

            Assert.AreEqual(1, r.Number, "Неверное значение");

            r = new Result(new Vector(11, (i) => (i != 10) ? -1.0 : 1.0));

            Assert.AreEqual(10, r.Number, "Неверное значение");
        }

        [TestMethod]
        public void Test_Result_Operation_Equals()
        {
            Result r1 = new Result(new Vector(2, (i) => (i != 0) ? -1.0 : 1.0));
            Result r2 = new Result(0, 2);

            Assert.AreEqual(true, r1==r2, "Неверно выполнена операция равенства");
            Assert.AreEqual(false, r1 != r2, "Неверно выполнена операция неравенства");

            r1 = new Result(new Vector(3, (i) => (i != 1) ? -1.0 : 1.0));
            r2 = new Result(new Vector(3, (i) => (i != 2) ? -1.0 : 1.0));

            Assert.AreEqual(true, r1 != r2, "Неверно выполнена операция неравенства, когда оба векторы");
            Assert.AreEqual(false, r1 == r2, "Неверно выполнена операция равенства, когда оба векторы");

            r1 = new Result(2, 3);
            r2 = new Result(7, 14);

            Assert.AreEqual(true, r1 != r2, "Неверно выполнена операция неравенства, когда оба числа");
            Assert.AreEqual(false, r1 == r2, "Неверно выполнена операция равенства, когда оба числа");

            r1 = new Result(new Vector(5, (i) => (i != 3) ? -1.0 : 1.0));
            r2 = new Result(new Vector(5, (i) => (i != 3) ? -1.0 : 1.0));

            Assert.AreEqual(true, r1 == r2, "Неверно выполнена операция равенства, когда оба векторы 2");
            Assert.AreEqual(false, r1 != r2, "Неверно выполнена операция неравенства, когда оба векторы 2");
        }
    }
}