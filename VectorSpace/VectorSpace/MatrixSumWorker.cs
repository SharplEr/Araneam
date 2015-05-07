using MyParallel;

namespace VectorSpace
{
    public class MatrixSumWorker: ParallelWorker<Vector>
    {
        Vector[] m;
        public MatrixSumWorker(int threadCount, Vector[] matrix) :
            base(threadCount, matrix, @"MatrixSumWorker№") { }

        public void Run(Vector[] mat)
        {
            m = mat;
            Run();
        }

        override protected void DoFromTo(int start, int finish)
        {
            for (int i = start; i < finish; i++)
            {
                vector[i].Addication(m[i]);
            }
        }
    }
}
