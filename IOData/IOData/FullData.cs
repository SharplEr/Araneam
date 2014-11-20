using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using ArrayHelper;

namespace IOData
{
    [Serializable]
    public class FullData
    {
        //Почекать потокобезопасность!
        Object blokerContinuous = new Object();
        Object blokerDiscrete = new Object();
        MixData[] mixInput;

        public MixData[] MixInput
        {
            get
            {
                return mixInput;
            }
        }

        Vector[] continuousInput;

        public Vector[] СontinuousInput
        {
            get
            {
                lock (blokerContinuous)
                {
                    if (continuousInput == null)
                    {
                        continuousInput = new Vector[mixInput.Length];

                        Func<int, double>[] fs = new Func<int, double>[MixInput[0].discrete.Length];

                        for (int i = 0; i < fs.Length; i++)
                            fs[i] = DataConverter.DiscreteToСontinuous(new ArrayShall<int>((j) => MixInput[j].discrete[i], MixInput.Length), output);

                        for (int i = 0; i < continuousInput.Length; i++)
                        {
                            continuousInput[i] = new Vector(mixInput[0].continuous.Length + fs.Length);

                            for (int j = 0; j < mixInput[0].continuous.Length; j++)
                                continuousInput[i][j] = mixInput[i].continuous[j];

                            for (int j = mixInput[0].continuous.Length; j < continuousInput[i].Length; j++)
                                continuousInput[i][j] = fs[j - mixInput[0].continuous.Length](mixInput[i].discrete[j - mixInput[0].continuous.Length]);
                        }
                        //continuousInput.Normalization(1.0);
                    }
                }
                return continuousInput;
            }
        }

        int[][] discreteInput;

        public int[][] DiscreteInput
        {
            get
            {
                lock (blokerDiscrete)
                {
                    if (discreteInput == null)
                    {
                        discreteInput = new int[mixInput.Length][];

                        Func<double, int>[] fs = new Func<double, int>[MixInput[0].continuous.Length];

                        for (int i = 0; i < fs.Length; i++)
                            fs[i] = DataConverter.СontinuousToDiscrete(new ArrayShall<double>((j) => mixInput[j].continuous[i], mixInput.Length), 3);

                        for (int i = 0; i < discreteInput.Length; i++)
                        {
                            discreteInput[i] = new int[mixInput[0].continuous.Length + fs.Length];

                            for (int j = 0; j < mixInput[0].continuous.Length; j++)
                                discreteInput[i][j] = fs[j](mixInput[i].continuous[j]);

                            for (int j = mixInput[0].continuous.Length; j < discreteInput[i].Length; j++)
                                discreteInput[i][j] = mixInput[i].discrete[j - mixInput[0].continuous.Length];
                        }
                    }
                }
                return discreteInput;
            }
        }

        Results output;

        public Results Output
        {
            get
            {
                return output;
            }
        }

        int dimension;

        public int Dimension
        {
            get
            { return dimension; }
        }

        public int Length
        {
            get
            { return mixInput.Length; }
        }

        public int[] maxdiscretePart;

        public FullData(string settingFile)
        {
            var x = DataFile.LoadDataInfo(settingFile);

            Set(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5);
        }

        public FullData(string[] fileNames, string[] InputTags, string OutputTag, string[] СontinuousTags, Func<string, double> ToDouble)
        {
            Set(fileNames, InputTags, OutputTag, СontinuousTags, ToDouble);
        }

        protected void Set(string[] fileNames, string[] InputTags, string OutputTag, string[] СontinuousTags, Func<string, double> ToDouble)
        {
            string[] DiscreteTags = InputTags.Remove(СontinuousTags);

            Vector[] cd = DataFile.getOnlyСontinuous(fileNames, СontinuousTags, ToDouble);

            var tempdd = DataFile.getOnlyDiscrete(fileNames, DiscreteTags);

            int[][] dd = tempdd.Item1;
            maxdiscretePart = tempdd.Item2;

            if (cd.Length != dd.Length) throw new ArgumentOutOfRangeException();

            mixInput = new MixData[cd.Length];

            for (int i = 0; i < mixInput.Length; i++)
                mixInput[i] = new MixData(cd[i], dd[i]);

            output = DataFile.getOnlyResult(fileNames, OutputTag);

            dimension = mixInput[0].continuous.Length + mixInput[0].discrete.Length;           
        }

        FullData(MixData[] mi, Vector[] ci, int[][] di, Results r)
        {
            mixInput = mi.CloneOk<MixData>();
            if (ci != null) continuousInput = ci.CloneOk<Vector>();
            if (di != null) discreteInput = di.CloneOk<int[]>();
            output = r.CloneOk();
            dimension = mixInput[0].continuous.Length + mixInput[0].discrete.Length;
        }

        public FullData(FullData data, int[] indexer)
        {
            mixInput = data.mixInput.CloneShuffle(indexer);
            output = data.output.CloneShuffle(indexer);
            dimension = data.dimension;
            maxdiscretePart = data.maxdiscretePart;
        }
    }
}