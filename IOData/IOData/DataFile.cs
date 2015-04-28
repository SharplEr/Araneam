using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using VectorSpace;

namespace IOData
{
    /// <summary>
    /// Набор статических методов для извлечения файлов данных, а так же извлечения инфрмации о файлах данных
    /// </summary>
    public static class DataFile
    {
        public static Tuple<string[], string[], string, string[], Func<string, double>, string[]> LoadDataInfo(string file)
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
        
        public static Tuple<string[], string[], string, string[], Func<string, double>, string[]> LoadDataInfo(StreamReader reader)
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

            //if (inputTags.Count == 0) throw new IOException("Неверный формат файла! Нет списка входных значений.");
            //Может не быть числовых входных значений

            s = reader.ReadLine();

            List<string> continuousTags = new List<string>();

            while (!String.IsNullOrWhiteSpace(s))
            {
                continuousTags.Add(s);
                s = reader.ReadLine();
            }

            //Все значения могут быть дискретными

            s = reader.ReadLine();

            List<string> stringTags = new List<string>();

            while (!String.IsNullOrWhiteSpace(s))
            {
                stringTags.Add(s);
                s = reader.ReadLine();
            }

            //Может не быть строковых значений

            string outputTag = reader.ReadLine();

            if (String.IsNullOrWhiteSpace(outputTag)) throw new IOException("Неверный формат файла! Должно быть выходное значение!");

            reader.ReadLine();
            s = reader.ReadLine();

            ProblemMod mod = ProblemMod.none;

            switch (s)
            {
                case ("classification"): mod = ProblemMod.classification; break;
                case ("regression"): mod = ProblemMod.regression; break;
                default: break;
            }

            if (reader.EndOfStream)
            {
                return new Tuple<string[], string[], string, string[], Func<string, double>, string[]>(fileNames.ToArray(), inputTags.ToArray(), outputTag, continuousTags.ToArray(), Convert.ToDouble, stringTags.ToArray());
            }

            if ((s == "standard") || (String.IsNullOrWhiteSpace(s)))
                return new Tuple<string[], string[], string, string[], Func<string, double>, string[]>(fileNames.ToArray(), inputTags.ToArray(), outputTag, continuousTags.ToArray(), Convert.ToDouble, stringTags.ToArray());
            else
            {
                Assembly a = Assembly.LoadFrom(@s);
                s = reader.ReadLine();//Имя класса преобразования
                Func<string, double> f = null;

                if (String.IsNullOrWhiteSpace(s))
                    foreach (Type t in a.GetExportedTypes())
                    {
                        if (typeof(IToDouble).IsAssignableFrom(t))
                        {
                            f = (Func<string, double>)t.GetMethod("ToDouble").CreateDelegate(typeof(Func<string, double>));
                            break;
                        }
                    }
                else
                    foreach (Type t in a.GetExportedTypes())
                    {
                        if (typeof(IToDouble).IsAssignableFrom(t) && (s == t.Name))
                        {
                            f = (Func<string, double>)t.GetMethod("ToDouble").CreateDelegate(typeof(Func<string, double>));
                            break;
                        }
                    }
                if (f == null) throw new IOException("В искомом файле не найден класс наследующий IToDouble");
                return new Tuple<string[], string[], string, string[], Func<string, double>, string[]>(fileNames.ToArray(), inputTags.ToArray(), outputTag, continuousTags.ToArray(), f, stringTags.ToArray());
            }
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

        public static Vector[] getOnlyRegressionResult(string[] fileNames, string[] Tags, Func<string, double> ToDouble)
        {
            CSVReader reader = new CSVReader(Tags, fileNames);
            if (!reader.Test()) throw new IOException("Bad files.");

            Vector[] ans = new Vector[reader.countLine];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = new Vector(reader.lineLength, (j) => ToDouble(reader[i, Tags[j]]));

            return ans;
        }

        public static String[][] getOnlyString(string[] fileNames, string[] Tags)
        {
            if (Tags == null) return null;
            CSVReader reader = new CSVReader(Tags, fileNames);
            if (!reader.Test()) throw new IOException("Bad files.");

            String[][] ans = new String[reader.countLine][];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = reader[i, Tags];

            return ans;
        }
    }
}