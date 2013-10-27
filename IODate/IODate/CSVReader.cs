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

        static Action<string, SystemException> OnError;

        public CSVReader(string name)
        {
            try
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
            catch(SystemException e)
            {
                OnError("Error in CSVReader at constructor(string name = '"+ name+"'.", e);
            }
        }

        public CSVReader(string[] tags, params string[] name)
        {
            List<string[]> temp = new List<string[]>();
            lineLength = tags.Length;
            for (int i = 0; i < name.Length; i++)
            {
                CSVReader reader = new CSVReader(name[i]);
                for (int j = 0; j < reader.countLine; j++)
                {
                    string[] strs = new string[lineLength];
                    for (int k = 0; k < lineLength; k++)
                    {
                        strs[k] = reader[j, tags[k]];
                    }
                    temp.Add(strs);
                }
            }
            allElement = temp.ToArray();
            countLine = allElement.Length;
            for (int i = 0; i<tags.Length; i++)
            {
                map.Add(tags[i].ToUpper(), i);
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
