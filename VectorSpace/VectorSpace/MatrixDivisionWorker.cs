using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyParallel;

namespace VectorSpace
{
    public class MatrixDivisionWorker : ParallelWorker<Vector>
    {
        Double d;
        public MatrixDivisionWorker(int threadCount, Vector[] matrix) :
            base(threadCount, matrix, @"MatrixDivisionWorker№") { }

        public void Run(Double x)
        {
            d = 1.0 / x;
            Run();
        }

        override protected void DoFromTo(int start, int finish)
        {
            for (int i = start; i < finish; i++)
            {
                vector[i].Multiplication(d);
            }
        }
    }
}
