using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IODate
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Этот класс присваивает порядковые номера для дискретных значений содержащихся в csv файле использует класс CSVReader
     * 1!) Вероятно концепция будет переработана
    */
    /// <summary>
    /// Класс преднозначенный для нумерования дискретных именных значений
    /// </summary>
    public class DateInfo
    {
        Dictionary<string, List<string>> sortDic = new Dictionary<string,List<string>>();

        public DateInfo(CSVReader reader, string[] ValueTags, string[] ResultTags, Func<string[], double> f)
        {
            string[] t = new string[ResultTags.Length];
            double d;
            Dictionary<string, Dictionary<string, double>> dic = new Dictionary<string,Dictionary<string,double>>();

            for (int i = 0; i < ValueTags.Length; i++)
            {
                dic.Add(ValueTags[i], new Dictionary<string, double>());
            }

            for (int i = 0; i < reader.countLine; i++)
            {
                
                for (int j = 0; j < ResultTags.Length; j++)
                {
                    t[j] = reader[i, ResultTags[j]];
                }

                d = f(t);

                for (int j = 0; j < ValueTags.Length; j++)
                {
                    string name = ValueTags[j];
                    string val = reader[i, name];
                    if (dic[name].ContainsKey(val))
                        dic[name][val] += d;
                    else
                        dic[name].Add(val, d);
                }
            }

            for (int i = 0; i < ValueTags.Length; i++)
            {
                var sorted =
                    (from kv in dic[ValueTags[i]]
                    orderby kv.Value
                    select kv.Key).ToList();
                sortDic.Add(ValueTags[i], sorted);
            }
        }

        public int this[string tag, string value]
        {
            get
            {
                return sortDic[tag].IndexOf(value);
            }
        }

        public string[] this[string tag]
        {
            get
            {
                return sortDic[tag].ToArray();
            }
        }
    }
}