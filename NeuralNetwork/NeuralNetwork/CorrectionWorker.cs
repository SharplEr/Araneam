using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;
using MyParallel;
using System.Threading;

namespace NeuralNetwork
{
    /// <summary>
    /// Паралельная коррекция весов по правилу w+=rate*input
    /// </summary>
    class CorrectionWorker: ParallelWorker<Neuron>
    {
        Double[] rate;
        public CorrectionWorker(int threadCount, Neuron[] neur) 
            : base(threadCount, neur, @"CorrectionWorker№") { }

        public void Run(Vector r)
        {
            if (r.Length != vector.Length) throw new ArgumentException("плохой вектор коррекции");
            rate = r.element;
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
                    syn = vector[i].synapse.element;
                    wei = vector[i].weight.element;
                    
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