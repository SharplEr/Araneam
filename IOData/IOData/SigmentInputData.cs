using System;
using IOData;
using VectorSpace;
using ArrayHelper;

namespace IOData
{
    public class SigmentInputData
    {
        protected FullData data;
        protected int[] indexer;

        public int Length
        {
            get
            {
                return indexer.Length;
            }
        }

        public int Dimension
        {
            get
            {
                return data.Dimension;
            }
        }

        public SigmentInputData(FullData d, int[] ind)
        {
            data = d;
            indexer = ind;
        }

        int[][] discreteArray = null;
        Object discretelock = new Object();

        public int[][] GetDiscreteArray()
        {
            lock (discretelock)
            {
                if (discreteArray == null)
                {
                    discreteArray = new int[Length][];

                    for (int i = 0; i < Length; i++)
                        discreteArray[i] = data.DiscreteInput[indexer[i]].CloneOk<int[]>();
                }
            }
            return discreteArray;
        }

        Vector[] continuousArray = null;
        object continuouslock = new object();

        public Vector[] GetСontinuousArray()
        {
            lock (continuouslock)
            {
                if (continuousArray == null)
                {
                    continuousArray = new Vector[Length];

                    for (int i = 0; i < Length; i++)
                        continuousArray[i] = data.СontinuousInput[indexer[i]].CloneOk();
                }
            }
            return continuousArray;
        }

        MixData[] mixArray = null;
        object mixlock = new object();

        public MixData[] GetMixArray()
        {
            lock (mixlock)
            {
                if (mixArray == null)
                {
                    mixArray = new MixData[Length];

                    for (int i = 0; i < Length; i++)
                        mixArray[i] = data.MixInput[indexer[i]].CloneOk();
                }
            }
            return mixArray;
        }
    }
}