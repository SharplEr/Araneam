using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IOData;
using System.IO;

namespace UnitTestIODate
{
    [TestClass]
    public class UnitTestCSV
    {
        [TestMethod]
        public void Test_CSVRead()
        {
            const string name = @".\xyu.csv";
            const string some1 = @"LSD,Кавай,MDMA";
            const string some2 = @"A,B,AB";
            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(some1);
            writer.WriteLine(some2);
            writer.Close();

            CSVReader reader = null;

            try
            {
                reader = new CSVReader(name);
            }
            catch (NullReferenceException e)
            {
                Assert.Fail("Сообщение: <" + e.Message + ">. Источник: <" + e.Source + ">. Стек вызовов: <" + e.StackTrace + ">");
            }

            try
            {
                Assert.IsTrue((reader[0, "LSD"] == "A") && (reader[0, "Кавай"] == "B") && (reader[0, "MDMA"] == "AB"), "Неверно считало строку");
            }
            catch (IndexOutOfRangeException e)
            {
                Assert.Fail("Сообщение: <" + e.Message + ">. Источник: <" + e.Source + ">. Стек вызовов: <" + e.StackTrace + ">");
            }
        }

        [TestMethod]
        public void Test_CSVRead_TestCorrect()
        {
            const string name = @".\xyu.csv";
            const string some1 = @"LSD,Кавай,MDMA";
            const string some2 = @"A,""2,5"",AB";
            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(some1);
            writer.WriteLine(some2);
            writer.Close();

            CSVReader reader = new CSVReader(name);
            Assert.IsTrue(reader.Test(), "Определяет как некорректный корректный файл");
        }


        [TestMethod]
        public void Test_CSVRead_TestWrong()
        {
            const string name = @".\xyu.csv";
            const string some1 = @"LSD,Кавай,MDMA";
            const string some2 = @"A,B,AB, Ad";
            const string some3 = @"A,""2,5""";
            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(some1);
            writer.WriteLine(some2);
            writer.WriteLine(some3);
            writer.Close();

            CSVReader reader = new CSVReader(name);
            Assert.IsFalse(reader.Test(), "Определяет как корректный некорректный файл");
        }
    }
}
