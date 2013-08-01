using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IODate;
using VectorSpace;
using System.IO;

namespace UnitTestIODate
{
    [TestClass]
    public class UnitTestDateAnalysis
    {
        [TestMethod]
        public void TestDateAnalysis()
        {
            const string name = @".\xyu1.csv";
            string[] some = new string[]{"mmm,sss,ddd,rrr",
                                                "A,BC,2, 1",
                                                "A,B,0, -1",
                                                "B,A,3, 1",
                                                "B,B,7, 1"};

            try
            {
                StreamWriter writer = new StreamWriter(name);
                for (int i = 0; i < some.Length; i++)
                    writer.WriteLine(some[i]);
                writer.Close();
            }
            catch
            {
                Assert.Fail("Не удается предварительно записать данные");
            }

            try
            {
                DateAnalysis analyser = new DateAnalysis(new string[] { name },
                    new string[] { "mmm", "sss", "ddd" }, new string[] { "rrr" }, new string[] { "ddd" }, (s) => Convert.ToDouble(s[0]), Convert.ToDouble);

                Assert.AreEqual(4, analyser.TestDate.Length, "Неверное число тестовых примеров");
                Assert.AreEqual(4, analyser.ResultDate.Length, "Неверное число примеров");
                for (int i = 0; i < analyser.TestDate.Length; i++)
                {
                    Assert.AreEqual(4, analyser.TestDate[i].Length, "Неверная размероность тестовых примеров");
                    Assert.AreEqual(1, analyser.ResultDate[i].Length, "Неверное размерность результатов");
                }

                Assert.AreEqual(1.0, analyser.ResultDate[0][0]);
                Assert.AreEqual(-1.0, analyser.ResultDate[1][0]);
                Assert.AreEqual(1.0, analyser.ResultDate[2][0]);
                Assert.AreEqual(1.0, analyser.ResultDate[3][0]);

                for (int j = 0; j < 3; j++)
                {
                    double max = Double.NegativeInfinity, min = Double.PositiveInfinity;
                    for (int i = 0; i < 4; i++)
                    {
                        double t = analyser.TestDate[i][j];
                        if (t < min) min = t;
                        if (t > max) max = t;
                    }
                    
                    Assert.AreEqual(1.0, max);
                    Assert.AreEqual(-1.0, min);
                }
                for (int i = 0; i < 4; i++)
                {
                    Assert.AreEqual(1.0, analyser.TestDate[i][3], "потеря порога");
                }
                
            }
            catch (SystemException e)
            {
                Assert.Fail("Сообщение: <" + e.Message + ">. Источник: <" + e.Source + ">. Стек вызовов: <" + e.StackTrace + ">");
            }
        }
    }
}
