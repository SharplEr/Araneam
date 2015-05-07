using System;
using MyParallel;
using VectorSpace;

namespace Araneam
{
    /// <summary>
    /// Параллельная коррекция весов по правилу w+=rate*input
    /// </summary>
    class CorrectionWorker: ParallelWorker<Neuron>
    {
        Double[] rate;
        public CorrectionWorker(int threadCount, Neuron[] neur) 
            : base(threadCount, neur, @"CorrectionWorker№") { }

        public void Run(Vector r)
        {
            if (r.Length != vector.Length) throw new ArgumentException("Bad correction spectrum");
            rate = r.elements;
            Run();
        }

        override unsafe protected void DoFromTo(int start, int finish)
        {
            double[] syn, wei;
            fixed (double* pr = &rate[start])
            {
                double* tr = pr;
                for (int i = start; i < finish; i++)
                {
                    syn = vector[i].synapse.elements;
                    wei = vector[i].weight.elements;
                    
                    fixed (double* psyn = syn, pwei = wei)
                    {
                        double* pend = psyn + syn.Length;
                        double* ts = psyn, tw = pwei;
                        while (ts < pend)
                        {
                            *tw += (*ts) * (*tr);
                            ts++;
                            tw++;
                        }
                    }
                    tr++;
                }
            }

        }
    }
}