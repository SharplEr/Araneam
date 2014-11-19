using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
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

        [TestMethod]
        public void Test_DataFile_getOnlyDiscrete()
        {
            const string name = @".\xyu8.csv";
            string[] some = new string[]{"mmm,sss,ddd,rrr",
                                                "A, B, B, 1",
                                                "A, B, A, -1",
                                                "B, A, B, 1",
                                                "B, B, C, -1"};

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

            var data = DataFile.getOnlyDiscrete(new string[] { name }, new string[] { "mmm", "sss", "ddd", "rrr" }).Item1;

            Assert.AreEqual(4, data.Length);

            Assert.AreEqual(0, data[0][0]);
            Assert.AreEqual(0, data[0][1]);
            Assert.AreEqual(0, data[0][2]);
            Assert.AreEqual(0, data[0][3]);

            Assert.AreEqual(0, data[1][0]);
            Assert.AreEqual(0, data[1][1]);
            Assert.AreEqual(1, data[1][2]);
            Assert.AreEqual(1, data[1][3]);

            Assert.AreEqual(1, data[2][0]);
            Assert.AreEqual(1, data[2][1]);
            Assert.AreEqual(0, data[2][2]);
            Assert.AreEqual(0, data[2][3]);

            Assert.AreEqual(1, data[3][0]);
            Assert.AreEqual(0, data[3][1]);
            Assert.AreEqual(2, data[3][2]);
            Assert.AreEqual(1, data[3][3]);
        }

        [TestMethod]
        public void Test_DataFile_getOnlyСontinuous()
        {
            const string name = @".\xyu8.csv";
            string[] some = new string[]{"mmm,sss,ddd",
                                                "4, 8, 0",
                                                "2, 4, 4.4",
                                                "3, 2, 3.3",
                                                "0, 0, 2.2"};
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

            var data = DataFile.getOnlyСontinuous(new string[] { name }, new string[] { "mmm", "sss", "ddd" }, (x) => { return Convert.ToDouble(x, CultureInfo.GetCultureInfo("en-US")); });

            Assert.AreEqual(4, data.Length);

            Assert.AreEqual(1, data[0][0], 0.0001);
            Assert.AreEqual(1, data[0][1], 0.0001);
            Assert.AreEqual(-1, data[0][2], 0.0001);

            Assert.AreEqual(0, data[1][0], 0.0001);
            Assert.AreEqual(0, data[1][1], 0.0001);
            Assert.AreEqual(1, data[1][2], 0.0001);

            Assert.AreEqual(0.5, data[2][0], 0.0001);
            Assert.AreEqual(-0.5, data[2][1], 0.0001);
            Assert.AreEqual(0.5, data[2][2], 0.0001);

            Assert.AreEqual(-1, data[3][0], 0.0001);
            Assert.AreEqual(-1, data[3][1], 0.0001);
            Assert.AreEqual(0, data[3][2], 0.0001);

            Assert.AreEqual(1, data[0][3], 0.0001);
            Assert.AreEqual(1, data[1][3], 0.0001);
            Assert.AreEqual(1, data[2][3], 0.0001);
        }

        [TestMethod]
        public void Test_DataFile_getOnlyResult()
        {
            const string name = @".\xyu9.csv";
            string[] some = new string[]{"mmm,sss,ddd,rrr",
                                                "4, 8, 0, A",
                                                "2, 4, 4.4, B",
                                                "3, 2, 3.3, B",
                                                "0, 0, 2.2, A"};
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

            var data = DataFile.getOnlyResult(new string[] { name }, "rrr");

            Assert.AreEqual(4, data.Length);

            Assert.AreEqual(0, data[0].Number);
            Assert.AreEqual(1, data[1].Number);
            Assert.AreEqual(1, data[2].Number);
            Assert.AreEqual(0, data[3].Number);
        }

        [TestMethod]
        public void Test_DataFile_LoadDataInfo()
        {
            const string name = @".\xyu10.csv";

            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(@"C:\Users\sharp_000\Downloads.txt");
            writer.WriteLine(@"C:\Users\sharp_000\Dropbox.txt");
            writer.WriteLine("");
            writer.WriteLine(@"MDMA");
            writer.WriteLine(@"LSD");
            writer.WriteLine(@"2C-B");
            writer.WriteLine("");
            writer.WriteLine(@"2C-B");
            writer.WriteLine("");
            writer.WriteLine(@"Old");
            writer.Close();
            writer.Dispose();

            StreamReader reader = new StreamReader(name);

            var ans = DataFile.LoadDataInfo(reader);
            reader.Close();
            reader.Dispose();

            Assert.AreEqual(ans.Item1.Length, 2);
            Assert.AreEqual(ans.Item1[0], @"C:\Users\sharp_000\Downloads.txt");
            Assert.AreEqual(ans.Item1[1], @"C:\Users\sharp_000\Dropbox.txt");

            Assert.AreEqual(ans.Item2.Length, 3);
            Assert.AreEqual(ans.Item2[0], @"MDMA");
            Assert.AreEqual(ans.Item2[1], @"LSD");
            Assert.AreEqual(ans.Item2[2], @"2C-B");

            Assert.AreEqual(ans.Item3, @"Old");

            Assert.AreEqual(ans.Item4.Length, 1);
            Assert.AreEqual(ans.Item4[0], @"2C-B");
        }

        [TestMethod]
        public void Test_DataFile_LoadDataInfo2()
        {
            const string name = @".\xyu10.csv";

            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(@"C:\Users\sharp_000\Downloads.txt");
            writer.WriteLine(@"C:\Users\sharp_000\Dropbox.txt");
            writer.WriteLine("");
            writer.WriteLine(@"MDMA");
            writer.WriteLine(@"LSD");
            writer.WriteLine(@"2C-B");
            writer.WriteLine("");
            writer.WriteLine("");
            writer.WriteLine(@"Old");
            writer.Close();
            writer.Dispose();

            StreamReader reader = new StreamReader(name);

            var ans = DataFile.LoadDataInfo(reader);
            reader.Close();
            reader.Dispose();

            Assert.AreEqual(ans.Item1.Length, 2);
            Assert.AreEqual(ans.Item1[0], @"C:\Users\sharp_000\Downloads.txt");
            Assert.AreEqual(ans.Item1[1], @"C:\Users\sharp_000\Dropbox.txt");

            Assert.AreEqual(ans.Item2.Length, 3);
            Assert.AreEqual(ans.Item2[0], @"MDMA");
            Assert.AreEqual(ans.Item2[1], @"LSD");
            Assert.AreEqual(ans.Item2[2], @"2C-B");

            Assert.AreEqual(ans.Item3, @"Old");

            Assert.AreEqual(ans.Item4.Length, 0);
        }

        [TestMethod]
        public void Test_DataFile_LoadDataInfo3()
        {
            const string name = @".\xyu10.csv";

            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(@"C:\Users\sharp_000\Downloads.txt");
            writer.WriteLine(@"C:\Users\sharp_000\Dropbox.txt");
            writer.WriteLine("");
            writer.WriteLine(@"MDMA");
            writer.WriteLine(@"LSD");
            writer.WriteLine(@"2C-B");
            writer.WriteLine("");
            writer.WriteLine("");
            writer.WriteLine(@"Old");
            writer.Close();
            writer.Dispose();

            var ans = DataFile.LoadDataInfo(name);

            Assert.AreEqual(ans.Item1.Length, 2);
            Assert.AreEqual(ans.Item1[0], @"C:\Users\sharp_000\Downloads.txt");
            Assert.AreEqual(ans.Item1[1], @"C:\Users\sharp_000\Dropbox.txt");

            Assert.AreEqual(ans.Item2.Length, 3);
            Assert.AreEqual(ans.Item2[0], @"MDMA");
            Assert.AreEqual(ans.Item2[1], @"LSD");
            Assert.AreEqual(ans.Item2[2], @"2C-B");

            Assert.AreEqual(ans.Item3, @"Old");

            Assert.AreEqual(ans.Item4.Length, 0);
        }
    }
}