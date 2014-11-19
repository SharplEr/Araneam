using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using VectorSpace;

namespace IOData
{
    public static class DataFile
    {
        public static Tuple<string[], string[], string, string[]> LoadDataInfo(string file)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(file);
                var ans = LoadDataInfo(reader);
                reader.Close();
                reader.Dispose();
                return ans;
            }
            catch
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                throw new IOException("Не удается считать файл!");
            }

        }
        public static Tuple<string[], string[], string, string[]> LoadDataInfo(StreamReader reader)
        {
            string s = reader.ReadLine();

            List<string> fileNames = new List<string>();

            while (!String.IsNullOrWhiteSpace(s))
            {
                fileNames.Add(s);
                s = reader.ReadLine();
            }

            if (fileNames.Count == 0) throw new IOException("Неверный формат файла! Нет списка файлов.");

            s = reader.ReadLine();

            List<string> inputTags = new List<string>();

            while (!String.IsNullOrWhiteSpace(s))
            {
                inputTags.Add(s);
                s = reader.ReadLine();
            }

            if (inputTags.Count == 0) throw new IOException("Неверный формат файла! Нет списка входных значений.");

            s = reader.ReadLine();

            List<string> continuousTags = new List<string>();

            while (!String.IsNullOrWhiteSpace(s))
            {
                continuousTags.Add(s);
                s = reader.ReadLine();
            }

            //Все значения могут быть дискретными

            string outputTag = reader.ReadLine();

            if (String.IsNullOrWhiteSpace(outputTag)) throw new IOException("Неверный формат файла! Должно быть выходное значение!");

            return new Tuple<string[], string[], string, string[]>(fileNames.ToArray(), inputTags.ToArray(), outputTag, continuousTags.ToArray());
        }

        public static Tuple<Vector[], Vector[]> getСontinuous(string[] fileNames, string[] inputTags, string[] outputTags, string[] continuousTags, Func<string, double> ToDouble, Func<double[], double> fEq)
        {
            string[] AllTags = inputTags.Union(outputTags).ToArray();
            CSVReader reader = new CSVReader(AllTags, fileNames);
            if (!reader.Test()) throw new IOException("Bad files.");
            string[] DiscreteTags = inputTags.Remove(continuousTags);

            Vector[] OutputData = new Vector[reader.countLine];
            double[] y = new double[OutputData.Length];
            for (int i = 0; i < reader.countLine; i++)
            {
                OutputData[i] = new Vector(outputTags.Length, (j)=>ToDouble(reader[i, outputTags[j]]));
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
                InputData[i] = new Vector(inputTags.Length + 1);
                for (int j = 0; j < DiscreteTags.Length; j++)
                    InputData[i][j] = fs[j](reader[i, DiscreteTags[j]]);

                for (int j = 0; j < continuousTags.Length; j++)
                    InputData[i][j + DiscreteTags.Length] = ToDouble(reader[i, continuousTags[j]]);
                InputData[i][inputTags.Length] = 1.0;
            }

            InputData.Normalization(1.0);
            return new Tuple<Vector[], Vector[]>(InputData, OutputData);
        }

        public static Tuple<int[][], int[][]> getDiscrete(string[] fileNames, string[] InputTags, string[] OutputTags, string[] СontinuousTags, Func<string, double> ToDouble, Func<string, int> ToInt, int min)
        {
            string[] AllTags = InputTags.Union(OutputTags).ToArray();
            CSVReader reader = new CSVReader(AllTags, fileNames);
            if (!reader.Test()) throw new IOException("Bad files.");
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
            if (!reader.Test()) throw new IOException("Bad files.");

            Vector[] InputData = new Vector[reader.countLine];

            for (int i = 0; i < reader.countLine; i++)
                    InputData[i] = new Vector(Tags.Length, (j)=>ToDouble(reader[i, Tags[j]]), 1.0);

            InputData.Normalization(1.0);

            return InputData;
        }

        public static Tuple<int[][], int[]> getOnlyDiscrete(string[] fileNames, string[] Tags)
        {
            CSVReader reader = new CSVReader(Tags, fileNames);
            if (!reader.Test()) throw new IOException("Bad files.");

            int[][] InputData = new int[reader.countLine][];

            Func<string, int>[] ToInts = new Func<string,int>[Tags.Length];

            int[] maxs = new int[Tags.Length];
            for (int i = 0; i < Tags.Length; i++)
                ToInts[i] = DataConverter.NumericOfString(reader[Tags[i]], out maxs[i]);

            for (int i = 0; i < reader.countLine; i++)
            {
                InputData[i] = new int[Tags.Length];

                for (int j = 0; j < Tags.Length; j++)
                    InputData[i][j] = ToInts[j](reader[i, Tags[j]]);
            }

            return new Tuple<int[][],int[]>(InputData, maxs);
        }

        public static Results getOnlyResult(string[] fileNames, string Tag)
        {
            CSVReader reader = new CSVReader(new string[]{Tag}, fileNames);
            if (!reader.Test()) throw new IOException("Bad files.");
        
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
    }
}