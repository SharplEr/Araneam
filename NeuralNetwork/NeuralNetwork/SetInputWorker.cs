﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;
using MyParallel;
using System.Threading;

namespace Araneam
{
    /// <summary>
    /// Параллельная связывание синапсов с входным сигналом
    /// </summary>
    class SetInputWorker: ParallelWorker<Neuron>
    {
        double[] input;
        int[][] inputIndex;
        public SetInputWorker(int threadCount, Neuron[] neur) 
            : base(threadCount, neur, @"SetInputWorker№") { }

        public void Run(Vector inp, int[][] Index)
        {
            input = inp.element;
            inputIndex = Index;
            Run();
        }

        override unsafe protected void DoFromTo(int start, int finish)
        {
            int[] Index;

            int i;

            for (i = start; i < finish; i++)
            {
                Index = inputIndex[i];
                fixed(int* pi0 = Index)
                {
                    fixed (double* ps0 = vector[i].synapse.element)
                    {
                        int* pit = pi0;
                        double* pst = ps0;
                        int* pend = pi0 + Index.Length;

                        while (pit < pend)
                        {
                            *pst = input[*pit];
                            pst++;
                            pit++;
                        }
                    }
                }
            }
        }
    }
}