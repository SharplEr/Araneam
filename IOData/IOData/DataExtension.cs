using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayHelper;

namespace IOData
{
    public class DataExtension
    {
        public static void ExtentionToMis(CSVReader reader, string[] allTags, int[] extIndexs, string mark, string fileName)
        {
            List<string[]> ans = new List<string[]>(reader.countLine * extIndexs.Length);

            for (int i = 0; i < reader.countLine; i++)
            {
                string[] bas = reader[i, allTags];
                ans.Add(bas.CloneOk());
                for (int j = 0; j < extIndexs.Length; j++)
                {
                    int k = extIndexs[j];
                    string t = bas[k];
                    if (t != mark)
                    {
                        bas[k] = mark;
                        ans.Add(bas.CloneOk());
                        bas[k] = t;
                    }
                }
            }

            CSVWriter.Write(fileName, ans.ToArray(), allTags);
        }

        public static void ExtentionToMis(string From, int[] extIndexs, string mark, string To)
        {
            CSVReader reader = new CSVReader(From);
            ExtentionToMis(reader, reader.Tags, extIndexs, mark, To);
        }

        public static void ExtentionToDoubleMis(CSVReader reader, string[] allTags, int[] extIndexs, string mark, string fileName)
        {
            List<string[]> ans = new List<string[]>(reader.countLine * extIndexs.Length);

            for (int i = 0; i < reader.countLine; i++)
            {
                string[] bas = reader[i, allTags];
                ans.Add(bas.CloneOk());
                for (int j = 0; j < extIndexs.Length; j++)
                {
                    int indj = extIndexs[j];
                    string t = bas[indj];
                    if (t != mark)
                    {
                        bas[indj] = mark;
                        ans.Add(bas.CloneOk());
                    }

                    for (int k = j+1; k < extIndexs.Length; k++)
                    {
                        int indk = extIndexs[k];
                        string tt = bas[indk];
                        if (tt != mark)
                        {
                            bas[indk] = mark;
                            ans.Add(bas.CloneOk());
                            bas[indk] = tt;
                        }
                    }
                    bas[indj] = t;
                }
            }

            CSVWriter.Write(fileName, ans.ToArray(), allTags);
        }

        public static void ExtentionToDoubleMis(string From, int[] extIndexs, string mark, string To)
        {
            CSVReader reader = new CSVReader(From);
            ExtentionToDoubleMis(reader, reader.Tags, extIndexs, mark, To);
        }

        public static string[][] ExtentionToDoubleMis(CSVReader reader, string[] allTags, int[] extIndexs, string mark)
        {
            List<string[]> ans = new List<string[]>(reader.countLine * extIndexs.Length);

            for (int i = 0; i < reader.countLine; i++)
            {
                string[] bas = reader[i, allTags];
                ans.Add(bas.CloneOk());
                for (int j = 0; j < extIndexs.Length; j++)
                {
                    int indj = extIndexs[j];
                    string t = bas[indj];
                    if (t != mark)
                    {
                        bas[indj] = mark;
                        ans.Add(bas.CloneOk());
                    }

                    for (int k = j + 1; k < extIndexs.Length; k++)
                    {
                        int indk = extIndexs[k];
                        string tt = bas[indk];
                        if (tt != mark)
                        {
                            bas[indk] = mark;
                            ans.Add(bas.CloneOk());
                            bas[indk] = tt;
                        }
                    }
                    bas[indj] = t;
                }
            }

            return ans.ToArray();
        }

        public static string[][] ExtentionToDoubleMis(string From, int[] extIndexs, string mark)
        {
            CSVReader reader = new CSVReader(From);
            return ExtentionToDoubleMis(reader, reader.Tags, extIndexs, mark);
        }
    }
}
