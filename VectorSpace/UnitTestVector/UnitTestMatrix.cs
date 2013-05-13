using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorSpace;
using MyParallel;
using System.Threading;

namespace UnitTestVector
{
    [TestClass]
    public class UnitTestMatrix
    {
        [TestMethod]
        public void TestMatrixSum()
        {
            const int n = 10;
            Vector[] matrix1 = new Vector[n];
            Vector[] matrix2 = new Vector[n];
            for(int i = 0; i<n; i++)
            {
                matrix1[i]= new Vector(n, (j)=>i*n + j);
                matrix2[i]= new Vector(n, (j)=>-i*n-j);
            }
            bool flag = true;
            
            new Thread(()=>
                {
                    MatrixSumWorker msw = new MatrixSumWorker(2, matrix1);
                    msw.Run(matrix2);
                    for (int i = 0; i < n; i++)
                        for (int j = 0; j < n; j++)
                            flag = flag && (matrix1[i][j] == 0.0);
                    msw.Dispose();
                }
                ).InMTA();
            
            Assert.IsTrue(flag, "Не сумирует матрицы на двух потоках");
        }

        [TestMethod]
        public void TestMatrixMul()
        {
            const int n = 10;
            Vector[] matrix1 = new Vector[n];
            Vector[] matrix2 = new Vector[n];
            for (int i = 0; i < n; i++)
            {
                matrix1[i] = new Vector(n);
                matrix1[i][i] = 1.0;
                matrix2[i] = new Vector(n).SetRandom();
            }
            bool flag = true;

            new Thread(() =>
            {
                MatrixSqMulWorker mmw = new MatrixSqMulWorker(2, matrix1);
                Vector[] ans = mmw.Run(matrix2);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        flag = flag && (ans[i][j] == matrix2[i][j]);
                mmw.Dispose();
            }
                ).InMTA();

            Assert.IsTrue(flag, "Не множатся матрицы на двух потоках");
        }

        [TestMethod]
        public void TestMatrixDiv()
        {
            const int n = 10;
            Vector[] matrix = new Vector[n];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = new Vector(n, (j) => i*n+j);
            }
            bool flag = true;

            new Thread(() =>
            {
                MatrixDivisionWorker mmw = new MatrixDivisionWorker(2, matrix);
                mmw.Run(2.0);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        flag = flag && (matrix[i][j] == (i*n+j)/2.0);
                mmw.Dispose();
            }
                ).InMTA();

            Assert.IsTrue(flag, "Не делит матрицу на скаляр на двух потоках");
        }

        [TestMethod]
        public void TestMatrixDivOne()
        {
            const int n = 10;
            Vector[] matrix = new Vector[n];
            for (int i = 0; i < n; i++)
            {
                matrix[i] = new Vector(n, (j) => i * n + j);
            }
            bool flag = true;

            new Thread(() =>
            {
                MatrixDivisionWorker mmw = new MatrixDivisionWorker(1, matrix);
                mmw.Run(2.0);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        flag = flag && (matrix[i][j] == (i * n + j) / 2.0);
                mmw.Dispose();
            }
                ).InMTA();

            Assert.IsTrue(flag, "Не делит матрицу на скаляр на одном потоке");
        }

        [TestMethod]
        public void TestMatrixMulOne()
        {
            const int n = 10;
            Vector[] matrix1 = new Vector[n];
            Vector[] matrix2 = new Vector[n];
            for (int i = 0; i < n; i++)
            {
                matrix1[i] = new Vector(n);
                matrix1[i][i] = 1.0;
                matrix2[i] = new Vector(n, (j)=>j);
            }
            bool flag = true;

            new Thread(() =>
            {
                MatrixSqMulWorker mmw = new MatrixSqMulWorker(1, matrix1);
                Vector[] ans = mmw.Run(matrix2);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        flag = flag && (ans[i][j] == matrix2[i][j]);
                mmw.Dispose();
            }
                ).InMTA();

            Assert.IsTrue(flag, "Не множатся матрицы на одном потоке");
        }

        [TestMethod]
        public void TestMatrixMulOneOne()
        {
            const int n = 1;
            Vector[] matrix1 = new Vector[n];
            Vector[] matrix2 = new Vector[n];
            for (int i = 0; i < n; i++)
            {
                matrix1[i] = new Vector(n);
                matrix1[i][i] = 1.0;
                matrix2[i] = new Vector(n).SetRandom();
            }
            bool flag = true;

            new Thread(() =>
            {
                MatrixSqMulWorker mmw = new MatrixSqMulWorker(1, matrix1);
                Vector[] ans = mmw.Run(matrix2);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        flag = flag && (ans[i][j] == matrix2[i][j]);
                mmw.Dispose();
            }
                ).InMTA();

            Assert.IsTrue(flag, "Не множатся матрицы 1x1 на одном потоке.");
        }

        [TestMethod]
        public void TestMatrixSumOne()
        {
            const int n = 2;
            Vector[] matrix1 = new Vector[n];
            Vector[] matrix2 = new Vector[n];
            for (int i = 0; i < n; i++)
            {
                matrix1[i] = new Vector(n, (j) => i * n + j);
                matrix2[i] = new Vector(n, (j) => -i * n - j);
            }
            bool flag = true;

            new Thread(() =>
            {
                MatrixSumWorker msw = new MatrixSumWorker(1, matrix1);
                msw.Run(matrix2);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        flag = flag && (matrix1[i][j] == 0.0);
                msw.Dispose();
            }
                ).InMTA();

            Assert.IsTrue(flag, "Не сумирует матрицы на одном потоке");
        }
    }
}