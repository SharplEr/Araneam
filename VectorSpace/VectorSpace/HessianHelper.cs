using System;
using System.Collections.Generic;
using System.Text;
using MyParallel;

namespace VectorSpace
{
    class HessianHelper:ParallelWorker<Vector>
    {
        Vector v;
        Double denominator;
        Vector[] numerator;
        object calc = new object();

        public HessianHelper(int threadCount, Vector[] matrix) :
            base(threadCount, matrix, @"HessianHelper№")
        {
            int n = matrix.Length;
            numerator = new Vector[n];
            for (int i = 0; i < n; i++)
                numerator[i] = new Vector(n);
        }

        public Double Denominator
        {
            get { return denominator; }
        }

        public Vector[] Numerator
        {
            get { return numerator; }
        }

        public void Run(Vector vec)
        {
            v = vec;
            Run();
        }

        override protected void DoFromTo(int start, int finish)
        {
            double prer = 0.0;
            double t;
            for (int i = start; i < finish; i++)
            {
                t = vector[i] * v;
                numerator[i].Set(v).Multiplication(t);
                prer += t * v[i];
            }
            lock (calc)
                denominator += prer;
        }
    }
}
