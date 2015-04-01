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
    }
}
