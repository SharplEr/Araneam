using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IODate
{
    public class CSVReader
    {
        string[][] allElement;
        Dictionary<string, int> map = new Dictionary<string, int>();
        readonly public int countLine;
        readonly public int lineLength;

        public CSVReader(string name)
        {
            StreamReader reader = new StreamReader(name);
            List<string> ans = new List<string>();
            string s;
            do
            {
                s = reader.ReadLine();
                if (s != null) ans.Add(s);
            } while (s != null);
            reader.Close();
            allElement = new string[ans.Count - 1][];

            countLine = allElement.Length;

            for (int i = 1; i < ans.Count; i++)
            {
                allElement[i - 1] = Separating(ans[i]);
            }

            string[] strs = Separating(ans[0]);

            lineLength = strs.Length;

            for (int i = 0; i < strs.Length; i++)
            {
                map.Add(strs[i].ToUpper(), i);
            }
        }

        string[] Separating(string s)
        {
            List<string> ans = new List<string>();
            string add = "";
            bool inQuotes = false;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '"') inQuotes = !inQuotes;
                else if ((inQuotes) && (s[i] == ','))
                {
                    add += ',';
                }
                else if (s[i] == ',')
                {
                    ans.Add(add);
                    add = "";
                }
                else if (s[i] != ' ') add += s[i];
            }
            ans.Add(add);

            return ans.ToArray();
        }

        public bool Test()
        {
            bool flag = true;

            for (int i = 0; i < allElement.Length; i++)
            {
                flag = (lineLength == allElement[i].Length);
                if (!flag) break;
            }
            return flag;
        }

        public string this[int i, string key]
        {
            get
            {
                string s = allElement[i][map[key.ToUpper()]];
                return s;
            }
        }
    }
}
