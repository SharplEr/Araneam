using ArrayHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorSpace;

namespace UnitTestVector
{
    [TestClass]
    public class UnitTestV
    {
        [TestMethod]
        public void Test_Vector_SumOne()
        {
            Vector v1 = new Vector(1);
            Vector v2 = new Vector(1);
            v1[0] = 5.0;
            v2[0] = 3.1;
            v1.Addication(v2);
            Assert.AreEqual(v1[0], 5.0 + 3.1, "Не может увеличить вектор длины один");
        }

        [TestMethod]
        public void Test_Vector_Modul()
        {
            Vector v = new Vector(11);
            v[0] = 123456789.0;
            for (int i = 1; i < v.Length; i++)
            {
                v[i] = 1.0;
            }

            double x = (double)v;

            double d = 0.0;

            for (int i = 1; i < v.Length; i++)
            {
                d+=v[i]*v[i];
            }

            d += v[0] * v[0];
            Assert.AreEqual(d, x, 10.0,"Не может вычислять норму. Ошибка: " + (d-x).ToString());
        }

        [TestMethod]
        public void Test_VectorSum()
        {
            const int n = 5;
            Vector v1 = new Vector(n);
            Vector v2 = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                v1[i] = i;
                v2[i] = -i;
            }
            v1.Addication(v2);

            bool flag = true;

            for (int i = 0; i < n; i++)
                flag = flag && (v1[i] == 0.0);

            Assert.IsTrue(flag, "Не может увеличить вектор");
        }

        [TestMethod]
        public void Test_Vector_Initialization()
        {
            const int n = 5;
            Vector v = new Vector(n, (j) => j);
            
            bool flag = true;

            for (int i = 0; i < n; i++)
                flag = flag && (v[i] == i);

            Assert.IsTrue(flag, "Не верная инициализация");
        }

        [TestMethod]
        public void Test_Vector_Clone()
        {
            const int n = 5;
            Vector v1 = new Vector(n, (j) => j);
            Vector v2 = v1.CloneOk();

            bool flag = !(v1.Equals(v2));

            if (!flag) Assert.IsTrue(flag, "Не создает нового объекта");

            for (int i = 0; i < n; i++)
                flag = flag && (v1[i] == v2[i]);

            Assert.IsTrue(flag, "Не верно клонирует");
        }

        [TestMethod]
        public void Test_Vector_MulOne()
        {
            Vector v1 = new Vector(1);

            v1[0] = 5.0;
            v1.Multiplication(1.1);
            Assert.AreEqual(v1[0], 5.0 * 1.1, "Не может умножить вектор длины один");
        }

        [TestMethod]
        public void Test_Vector_MulScal()
        {
            const int n = 5;
            Vector v1 = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                v1[i] = i;
            }
            v1.Multiplication(1.1);

            bool flag = true;

            for (int i = 0; i < n; i++)
                flag = flag && (v1[i] == i*1.1);
            
            Assert.IsTrue(flag, "Не может умножить вектор на скаляр");
        }

        [TestMethod]
        public void Test_Vector_Mul()
        {
            const int n = 5;
            Vector v1 = new Vector(n, (i)=>i);
            Vector v2 = new Vector(n, (i)=>i/2.0);

            v1.Multiplication(v2);

            Assert.AreEqual(v1.Length, n, "Длина умножаемого вектора изменилась");
            Assert.AreEqual(v2.Length, n, "Длина вектора-множителя изменилась");

            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(i / 2.0, v2[i], 0.00000001, "Вектор множитель изменился на {0} шаге", i);
            }

            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(i * i / 2.0, v1[i], 0.00000001, "Поломалось умножение на {0} шаге", i);
            }
        }

        [TestMethod]
        public void Test_Vector_MulMatrix()
        {
            Vector v = new Vector(3);
            v[0] = 29;
            v[1] = 23;
            v[2] = 31;

            Vector[] m = new Vector[3];
            m[0] = new Vector(3);
            m[0][0] = 19;
            m[0][1] = 3;
            m[0][2] = 5;
            m[1] = new Vector(3);
            m[1][0] = 7;
            m[1][1] = 11;
            m[1][2] = 13;
            m[2] = new Vector(3);
            m[2][0] = 15;
            m[2][1] = 17;
            m[2][2] = 2;
            Vector n = m * v;
            Assert.AreEqual(n.Length, 3, "Нарушена размерность");
            Assert.AreEqual(n[0], 775, 0.001, "ошибка в 1 элементе");
            Assert.AreEqual(n[1], 859, 0.001, "ошибка в 2 элементе");
            Assert.AreEqual(n[2], 888, 0.001, "ошибка в 3 элементе");
        }
    }
}
