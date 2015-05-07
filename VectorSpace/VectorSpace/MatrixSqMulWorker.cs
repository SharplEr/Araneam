using MyParallel;

namespace VectorSpace
{
    public class MatrixSqMulWorker:ParallelWorker<Vector>
    {
        
        Vector[] m;
        readonly int n;

        readonly Vector[] temp;

        public MatrixSqMulWorker(int threadCount, Vector[] matrix) :
            base(threadCount, matrix, @"MatrixSqMulWorker№") 
        {
            n = matrix.Length;
            temp = new Vector[n];
            for (int i = 0; i < n; i++)
                temp[i] = new Vector(n);
        }

        public Vector[] Run(Vector[] mat)
        {
            m = mat;
            Run();
            return temp;
        }

        override protected void DoFromTo(int start, int finish)
        {
            int i, j, k;

            for (i = start; i < finish; i++)
            {
                for (j = 0; j < n; j++)
                {
                    temp[i][j] = 0.0;
                    for (k = 0; k < n; k++)
                        temp[i][j] += vector[i][k] * m[k][j];
                }
            }
            
        }
    }
}
