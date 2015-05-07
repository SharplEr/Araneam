using System;
using ArrayHelper;
using VectorSpace;

namespace IOData
{
    [Serializable]
    public class FullData
    {
        //Почекать потокобезопасность!
        readonly Object blokerContinuous = new Object();
        readonly Object blokerDiscrete = new Object();
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
                            discreteInput[i] = new int[mixInput[0].discrete.Length + fs.Length];

                            for (int j = 0; j < fs.Length; j++)
                                discreteInput[i][j] = fs[j](mixInput[i].continuous[j]);

                            for (int j = fs.Length; j < discreteInput[i].Length; j++)
                                discreteInput[i][j] = mixInput[i].discrete[j - fs.Length];
                        }
                    }
                }
                return discreteInput;
            }
        }

        String[][] stringInput;

        public String[][] StringInput
        {
            get { return stringInput; }
        }

        Results output;

        public Results Output
        {
            get
            {
                return output;
            }
        }

        Double[] regression;

        public Double[] Regression
        {
            get
            { return regression; }
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

        public int[] maxdiscretePart {get; protected set; }

        public FullData(string settingFile)
        {
            var x = DataFile.LoadDataInfo(settingFile);

            Set(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7);
        }

        protected void Set(string[] fileNames, string[] InputTags, string OutputTag, string[] СontinuousTags, Func<string, double> ToDouble, String[] stringTags, ProblemMod mod)
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

            switch (mod)
            {
                case ProblemMod.classification:
                    output = DataFile.getOnlyResult(fileNames, OutputTag);
                    break;
                case ProblemMod.regression:
                    regression = DataFile.getOnlyRegressionResult(fileNames, OutputTag, ToDouble);
                    break;
                default:
                    throw new ArgumentException("bad mod");
            }
            

            dimension = mixInput[0].continuous.Length + mixInput[0].discrete.Length;

            stringInput = DataFile.getOnlyString(fileNames, stringTags);
        }

        public FullData(FullData data, int[] indexer)
        {
            mixInput = data.mixInput.CloneShuffle(indexer);
            output = data.output.CloneShuffle(indexer);
            continuousInput = data.СontinuousInput.CloneShuffle(indexer);
            discreteInput = data.DiscreteInput.CloneShuffle(indexer);
            dimension = data.dimension;
            maxdiscretePart = data.maxdiscretePart;
            stringInput = data.stringInput.CloneShuffle(indexer);
            regression = data.regression.CloneShuffleStruct(indexer);
        }
    }
}