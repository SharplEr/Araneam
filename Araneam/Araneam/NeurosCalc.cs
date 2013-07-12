using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;
using MyParallel;
using System.Threading;

namespace Araneam
{
    /// <summary>
    /// Параллельный расчет активации нейронов
    /// </summary>
    class NeurosCalcWorker: ParallelWorker<Neuron>
    {
        Vector output;

        public NeurosCalcWorker(int threadCount, Neuron[] neur)
            : base(threadCount, neur, @"NeurosCalcWorker№"){}

        public void Run(Vector o)
        {
            output = o;
            Run();
        }

        override protected void DoFromTo(int start, int finish)
        {
            for (int i = start; i < finish; i++)
            {
                output[i] = vector[i].CalcS();
            }
        }
    }
}