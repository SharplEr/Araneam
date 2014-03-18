using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;
using VectorSpace;
using System.Threading;
using System.IO;
using MyParallel;

namespace UnitTestNetwork
{
    [TestClass]
    public class UnitTestLMS
    {
        [TestMethod]
        public void Test_LMSLearn()
        {
            LMSNetwork nw = new LMSNetwork(0.9, 100, 1, 2, "no");
            Vector x = new Vector(2, (j) => 1.0);
            Vector d = new Vector(1, (j) => 1.0);

            double y = 0.0;

            new Thread(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        nw.Learn(x, d);
                    }

                    y = nw.Calculation(x)[0];
                    
                    nw.Dispose();
                }).InMTA();

            Assert.AreEqual(1.0, y, 0.0001, "Сеть не обучается");
        }

        [TestMethod]
        public void Test_LMSFix()
        {
            double yPred = 0.0, yPost = 0.0;
            new Thread(() =>
            {
                LMSNetwork nw = new LMSNetwork(0.9, 100, 1, 2, "no");
                Vector x = new Vector(2, (j) => 1.0);
                Vector d = new Vector(1, (j) => 1.0);

                for (int i = 0; i < 1000; i++)
                {
                    nw.Learn(x, d);
                }
                
                nw.Fix();
                
                d = new Vector(1, (j) => 2.0);

                for (int i = 0; i < 1000; i++)
                {
                    nw.Learn(x, d);
                }

                yPred = nw.Calculation(x)[0];
                
                nw.ReFix();
                
                yPost = nw.Calculation(x)[0];
                
                nw.Dispose();
                
            }).InMTA();

            Assert.AreEqual(2.0, yPred, 0.0001, "Обучение поломалось после фиксации");
            Assert.AreEqual(1.0, yPost, 0.0001, "Восстановление не работает");
        }

        [TestMethod]
        public void Test_LMSSaveLoad()
        {
            bool save, load;
            save = load = false;
            double y = 0.0;
            new Thread(() =>
            {
                LMSNetwork nw = new LMSNetwork(0.9, 100, 1, 2, "no");
                Vector x = new Vector(2, (j) => 1.0);
                Vector d = new Vector(1, (j) => 1.0);

                for (int i = 0; i < 1000; i++)
                {
                    nw.Learn(x, d);
                }
                Stream s = File.OpenWrite(@".\saakcus.csv");
                save = nw.Save(s);
                nw.Dispose();
                s.Close();
                nw = new LMSNetwork(0.9, 100, 1, 2, "no");
                s = File.OpenRead(@".\saakcus.csv");

                load = nw.Load(s);
                
                s.Close();
                y = nw.Calculation(x)[0];
                
            }).InMTA();

            Assert.IsTrue(save, "Сохранение не удалось.");

            Assert.IsTrue(load, "Загрузка не удалась.");

            Assert.AreEqual(1.0, y, 0.0001, "Загрузка совершена с ошибкой");
        }
    }
}