using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IODate;
using System.IO;

namespace UnitTestIODate
{
    [TestClass]
    public class UnitTestDateInfo
    {
        [TestMethod]
        public void TestDateInfo()
        {
            const string name = @".\xyu.csv";
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

            CSVReader reader = null;
            try
            {
                reader = new CSVReader(name);
            }
            catch
            {
                Assert.Fail("Не удается считать данные");
            }
            try
            {
                DateInfo info = new DateInfo(reader, new string[] { "mmm", "sss" }, new string[] { "rrr" }, (s) => Convert.ToDouble(s[0]));

                Assert.AreEqual(0, info["mmm", "A"]);
                Assert.AreEqual(1, info["mmm", "B"]);
                Assert.AreEqual(0, info["sss", "B"]);
                Assert.IsTrue((info["sss", "A"] == 1 && info["sss", "BC"] == 2) || (info["sss", "A"] == 2 && info["sss", "BC"] == 1));
            }
            catch (SystemException e)
            {
                Assert.Fail("Сообщение: <" + e.Message + ">. Источник: <" + e.Source + ">. Стек вызовов: <" + e.StackTrace + ">");
            }
        }
    }
}
