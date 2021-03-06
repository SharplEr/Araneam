﻿using System;
using System.Collections.Generic;
using System.IO;
using ArrayHelper;

namespace IOData
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Этот класс помогает считывать данные из csv файлов
     * 1) Данный класс работает по следующему принцыпу
     *      1.1) Считывает данные из файла, причем первую строку определяет как название столбцов
     *      1.2) Создает карту и позваляет обращаться к данным в виде [номер строки, имя столбца]
     * 2) Для получения данных требуется
     *      2.1) Вызвать однин из конструкторов
     *      2.2) Стоит выполнить метод Test() для того, что бы проверить, что все данные считались без видимых нарушений
     *      2.3) Затем можно обращаться по идексу в виде [номер строки, имя столбца]. Результат - строка.
    */
    public class CSVReader
    {
        readonly string[][] allElement;
        readonly Dictionary<string, int> map = new Dictionary<string, int>();
        readonly public int countLine;
        readonly public int lineLength;

        static readonly Action<string, SystemException> OnError = (x, y) => Console.WriteLine(x);

        public CSVReader(string name)
        {
            try
            {
                List<string> ans = new List<string>();
                string s;
                using (var reader = new StreamReader(name))
                {
                    do
                    {
                        s = reader.ReadLine();
                        if (s != null) ans.Add(s);
                    } while (s != null);
                }
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
                if (OnError!= null) OnError("Error in CSVReader at constructor(string name = '"+ name+"'.", e);
            }
        }

        public CSVReader(string[] tags, params string[] name)
        {
            List<string[]> temp = new List<string[]>();
            lineLength = tags.Length;
            for (int i = 0; i < name.Length; i++)
            {
                var reader = new CSVReader(name[i]);
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

        static string[] Separating(string s)
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
                if (!flag) 
                    break;
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

        public string[] this[string key]
        {
            get
            {
                string[] strs = new string[countLine];
                for(int i = 0; i < countLine; i++)
                    strs[i] = allElement[i][map[key.ToUpper()]];
                return strs;
            }
        }

        public string[] this[int index, string[] tags]
        {
            get
            {
                string[] strs = new string[lineLength];
                for (int i = 0; i < lineLength; i++)
                    strs[i] = allElement[index][map[tags[i].ToUpper()]];
                return strs;
            }
        }

        string[] descriptors = null;

        public string[] Descriptors
        {
            get
            {
                if (descriptors == null) descriptors = map.Keys.GetEnumerator().ToArray<string>(map.Keys.Count);
                return descriptors;
            }
        }
    }
}
