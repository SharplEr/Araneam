using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;
using MyParallel;

namespace Araneam
{
    class DerivativeWorker : ParallelWorker<NeuronLayer>
    {
        Vector[] output;
        public DerivativeWorker(int threadCount, NeuronLayer[] layers)
            : base(threadCount, layers, @"DerivativeWorker№") { }

        public void Run(Vector[] o)
        {
            output = o;
            Run();
        }

        override protected void DoFromTo(int start, int finish)
        {
            for (int i = start; i < finish; i++)
            {
                output[i] = vector[i].CalcDer();
            }
        }
    }
}
