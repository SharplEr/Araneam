using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IOData
{
    public class CSVWriter
    {
        /// <summary>
        /// Записывает в csv файл плоскую таблицу
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elements"></param>
        public static void Write(string name, string[,] elements)
        {
            StreamWriter writer = new StreamWriter(name, false);

            int n = elements.GetLength(0);
            int m = elements.GetLength(1)-1;    //В цикле понятно, почему отнимается 1.

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    writer.Write(elements[i, j] + ",");

                writer.WriteLine(elements[i, m]);
            }

            writer.Close();
            writer.Dispose();
        }

        /// <summary>
        /// Записывает в csv файл плоскую таблицу
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elements"></param>
        public static void Write(string name, string[][] elements)
        {
            StreamWriter writer = new StreamWriter(name, false);

            int n = elements.Length;
            int m = elements[0].Length - 1;    //В цикле понятно, почему отнимается 1.

            for (int i = 0; i < n; i++)
            {
                string[] t = elements[i];

                for (int j = 0; j < m; j++)
                    writer.Write(t[j] + ",");

                writer.WriteLine(t[m]);
            }

            writer.Close();
            writer.Dispose();
        }

        /// <summary>
        /// Записывает в csv файл плоскую таблицу
        /// </summary>
        /// <param name="name">Имя файла</param>
        /// <param name="elements">Элементы</param>
        /// <param name="tags">Имена колонок</param>
        public static void Write(string name, string[][] elements, string[] tags)
        {
            StreamWriter writer = new StreamWriter(name, false);

            int n = elements.Length;
            int m = tags.Length-1;    //В цикле понятно, почему отнимается 1.

            if (m != elements[0].Length-1) throw new ArgumentException();

            for (int j = 0; j < m; j++)
                writer.Write(tags[j] + ",");

            writer.WriteLine(tags[m]);

            for (int i = 0; i < n; i++)
            {
                string[] t = elements[i];

                for (int j = 0; j < m; j++)
                    writer.Write(t[j] + ",");

                writer.WriteLine(t[m]);
            }

            writer.Close();
            writer.Dispose();
        }

    }
}
