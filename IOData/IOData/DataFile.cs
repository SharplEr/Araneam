using System;
using System.Collections.Generic;
using System.Linq;
using VectorSpace;

namespace IOData
{
    public static class DataFile
    {
        public static Tuple<Vector[], Vector[]> getСontinuous(string[] fileNames, string[] InputTags, string[] OutputTags, string[] СontinuousTags, Func<string, double> ToDouble, Func<double[], double> fEq)
        {
            string[] AllTags = InputTags.Union(OutputTags).ToArray();
            CSVReader reader = new CSVReader(AllTags, fileNames);
            if(!reader.Test()) throw new ArgumentException("Bad files.");
            string[] DiscreteTags = InputTags.Remove(СontinuousTags);

            Vector[] OutputData = new Vector[reader.countLine];
            double[] y = new double[OutputData.Length];
            for (int i = 0; i < reader.countLine; i++)
            {
                OutputData[i] = new Vector(OutputTags.Length, (j)=>ToDouble(reader[i, OutputTags[j]]));
                y[i] = fEq(OutputData[i].element);
            }

            Func<string, double>[] fs = new Func<string, double>[DiscreteTags.Length];

            for (int i = 0; i < DiscreteTags.Length; i++)
            {
                string[] inp = new string[reader.countLine];
                for (int j = 0; j < inp.Length; j++)
                {
                    inp[j] = reader[j, DiscreteTags[i]];
                }
                fs[i] = DataConverter.DiscreteToСontinuous(inp, y);
            }
            
            Vector[] InputData = new Vector[reader.countLine];
            for (int i = 0; i < reader.countLine; i++)
            {
                InputData[i] = new Vector(InputTags.Length + 1);
                for (int j = 0; j < DiscreteTags.Length; j++)
                    InputData[i][j] = fs[j](reader[i, DiscreteTags[j]]);

                for (int j = 0; j < СontinuousTags.Length; j++)
                    InputData[i][j + DiscreteTags.Length] = ToDouble(reader[i, СontinuousTags[j]]);
                InputData[i][InputTags.Length] = 1.0;
            }

            InputData.Normalization(1.0);
            return new Tuple<Vector[], Vector[]>(InputData, OutputData);
        }

        /*!!!!1
         * 
         * Что за string to int? нумерация должна быть автоматическая!111 
         * 
         * 
         *!111111!1 
        */
        public static Tuple<int[][], int[][]> getDiscrete(string[] fileNames, string[] InputTags, string[] OutputTags, string[] СontinuousTags, Func<string, double> ToDouble, Func<string, int> ToInt, int min)
        {
            string[] AllTags = InputTags.Union(OutputTags).ToArray();
            CSVReader reader = new CSVReader(AllTags, fileNames);
            if (!reader.Test()) throw new ArgumentException("Bad files.");
            string[] DiscreteTags = InputTags.Remove(СontinuousTags);

            int[][] OutputData = new int[reader.countLine][];

            for (int i = 0; i < reader.countLine; i++)
            {
                OutputData[i] = new int[OutputTags.Length];
                for (int j = 0; j < OutputTags.Length; j++)
                    OutputData[i][j] = ToInt(reader[i, OutputTags[j]]);
            }

            Func<double, int>[] fs = new Func<double, int>[СontinuousTags.Length];

            for (int i = 0; i < СontinuousTags.Length; i++)
            {
                double[] inp = new double[reader.countLine];
                for (int j = 0; j < inp.Length; j++)
                {
                    inp[j] = ToDouble(reader[j, СontinuousTags[i]]);
                }
                fs[i] = DataConverter.СontinuousToDiscrete(inp, min);
            }

            int[][] InputData = new int[reader.countLine][];
            for (int i = 0; i < reader.countLine; i++)
            {
                InputData[i] = new int[InputTags.Length];

                for (int j = 0; j < DiscreteTags.Length; j++)
                    InputData[i][j] = ToInt(reader[i, DiscreteTags[j]]);

                for (int j = 0; j < СontinuousTags.Length; j++) 
                    InputData[i][j + DiscreteTags.Length] = fs[j](ToDouble(reader[i, СontinuousTags[j]]));
            }

            return new Tuple<int[][], int[][]>(InputData, OutputData);
        }

        public static Vector[] getOnlyСontinuous(string[] fileNames, string[] Tags, Func<string, double> ToDouble)
        {
            CSVReader reader = new CSVReader(Tags, fileNames);
            if (!reader.Test()) throw new ArgumentException("Bad files.");

            Vector[] InputData = new Vector[reader.countLine];

            for (int i = 0; i < reader.countLine; i++)
                    InputData[i] = new Vector(Tags.Length, (j)=>ToDouble(reader[i, Tags[j]]), 1.0);

            InputData.Normalization(1.0);

            return InputData;
        }

        public static int[][] getOnlyDiscrete(string[] fileNames, string[] Tags)
        {
            CSVReader reader = new CSVReader(Tags, fileNames);
            if (!reader.Test()) throw new ArgumentException("Bad files.");

            int[][] InputData = new int[reader.countLine][];

            Func<string, int>[] ToInts = new Func<string,int>[Tags.Length];

            for (int i = 0; i < Tags.Length; i++)
                ToInts[i] = DataConverter.NumericOfString(reader[Tags[i]]);

            for (int i = 0; i < reader.countLine; i++)
            {
                InputData[i] = new int[Tags.Length];

                for (int j = 0; j < Tags.Length; j++)
                    InputData[i][j] = ToInts[j](reader[i, Tags[j]]);
            }

            return InputData;
        }

        public static Results getOnlyResult(string[] fileNames, string Tag)
        {
            CSVReader reader = new CSVReader(new string[]{Tag}, fileNames);
            if (!reader.Test()) throw new ArgumentException("Bad files.");
        
            int[] r = new int[reader.countLine];
            int max = 0;
            string[] strs = reader[Tag];
            List<string> num = new List<string>();

            for (int i = 0; i < strs.Length; i++)
            {
                if (!num.Contains(strs[i]))
                    num.Add(strs[i]);
                r[i] = num.IndexOf(strs[i]);
            }

            max = num.Count;

            Results results = new Results((i) => new Result(r[i], max), reader.countLine);

            return results;
        }

        //list equivalents
    }
}