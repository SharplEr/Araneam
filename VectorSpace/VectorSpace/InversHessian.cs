using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VectorSpace
{
    public class InversHessian: IDisposable
    {
        HessianHelper helper;
        MatrixSumWorker sumHelper;
        MatrixDivisionWorker divHelper;
        public Vector[] H;

        public InversHessian(int n)
        {
            const double m = 100000.0;
            H = new Vector[n];

            for (int i = 0; i < n; i++)
                (H[i] = new Vector(n))[i] = m;

            int threadCount = Environment.ProcessorCount;
            if (threadCount > n) threadCount = n;

            helper = new HessianHelper(n, H);
            sumHelper = new MatrixSumWorker(n, H);
            divHelper = new MatrixDivisionWorker(n, helper.Numerator);
        }

        public void Next(Vector v)
        {
            helper.Run(v);
            //helper.Numerator * H матрично и поделить на (helper.Denominator+1)
            divHelper.Run(helper.Denominator + 1.0);
            sumHelper.Run(helper.Numerator);
        }

        public void Dispose()
        {
            helper.Dispose();
            sumHelper.Dispose();
            divHelper.Dispose();
        }
    }
}
