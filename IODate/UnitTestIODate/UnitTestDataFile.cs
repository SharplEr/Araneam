using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using IOData;
using VectorSpace;

namespace UnitTestIODate
{
    [TestClass]
    public class UnitTestDataFile
    {
        [TestMethod]
        public void Test_DataFile_getСontinuous()
        {
            const string name = @".\xyu7.csv";
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

            Tuple<Vector[], Vector[]> ans = DataFile.getСontinuous(
                new string[]{name}, new string[] { "mmm", "sss", "ddd" }, new string[] { "rrr" },
                new string[] { "ddd" }, Convert.ToDouble, (s) => s[0]
                );
            Vector[] InputData = ans.Item1;
            Vector[] OutputData = ans.Item2;

            Assert.AreEqual(4, InputData.Length, "Неверное число тестовых примеров");
            Assert.AreEqual(4, OutputData.Length, "Неверное число примеров");

            for (int i = 0; i < InputData.Length; i++)
            {
                Assert.AreEqual(4, InputData[i].Length, "Неверная размероность тестовых примеров");
                Assert.AreEqual(1, OutputData[i].Length, "Неверное размерность результатов");
            }

            Assert.AreEqual(1.0, OutputData[0][0]);
            Assert.AreEqual(-1.0, OutputData[1][0]);
            Assert.AreEqual(1.0, OutputData[2][0]);
            Assert.AreEqual(1.0, OutputData[3][0]);

            for (int j = 0; j < 3; j++)
            {
                double max = Double.NegativeInfinity, min = Double.PositiveInfinity;
                for (int i = 0; i < 4; i++)
                {
                    double t = InputData[i][j];
                    if (t < min) min = t;
                    if (t > max) max = t;
                }

                Assert.AreEqual(1.0, max);
                Assert.AreEqual(-1.0, min);
            }
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(1.0, InputData[i][3], "потеря порога");
            }
        }

        [TestMethod]
        public void Test_DataFile_getDiscrete()
        {
            const string name = @".\xyu7.csv";
            string[] some = new string[]{"mmm,sss,ddd,rrr",
                                                "A,B,3, B",
                                                "A,B,0, A",
                                                "B,A,2, B",
                                                "B,B,7, B"};

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

            Tuple<int[][], int[][]> ans = DataFile.getDiscrete(
                new string[] { name }, new string[] { "mmm", "sss", "ddd" }, new string[] { "rrr" },
                new string[] { "ddd" }, Convert.ToDouble,
                (s)=> 
                    (s=="A")?0:1,
                2
                );
            int[][] InputData = ans.Item1;
            int[][] OutputData = ans.Item2;

            Assert.AreEqual(4, InputData.Length, "Неверное число тестовых примеров");
            Assert.AreEqual(4, OutputData.Length, "Неверное число примеров");

            for (int i = 0; i < InputData.Length; i++)
            {
                Assert.AreEqual(3, InputData[i].Length, "Неверная размероность тестовых примеров");
                Assert.AreEqual(1, OutputData[i].Length, "Неверное размерность результатов");
            }

            Assert.AreEqual(1, OutputData[0][0]);
            Assert.AreEqual(0, OutputData[1][0]);
            Assert.AreEqual(1, OutputData[2][0]);
            Assert.AreEqual(1, OutputData[3][0]);

            Assert.AreEqual(0, InputData[0][0]);
            Assert.AreEqual(0, InputData[1][0]);
            Assert.AreEqual(1, InputData[2][0]);
            Assert.AreEqual(1, InputData[3][0]);

            Assert.AreEqual(1, InputData[0][1]);
            Assert.AreEqual(1, InputData[1][1]);
            Assert.AreEqual(0, InputData[2][1]);
            Assert.AreEqual(1, InputData[3][1]);

            Assert.AreEqual(1, InputData[0][2]);
            Assert.AreEqual(0, InputData[1][2]);
            Assert.AreEqual(0, InputData[2][2]);
            Assert.AreEqual(1, InputData[3][2]);
        }
    }
}