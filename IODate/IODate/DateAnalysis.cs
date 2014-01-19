using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using MyParallel;

namespace IODate
{
     /*
      * Общие пояснения для тех, кто попытается разобраться:
      * 0) Этот класс служит для выделения данных обучения из csv файловов
      *              Использует класс DateReader, DateInfo и CSVReader
      * 1) Для того что бы извлечь данные достаточно только вызвать конструктор и все нужные значения будут находится в свойствах TestDate и ResultDate
      * 2) Так же будет доступен делегат Convert - который производит нормировку значений. !Эта конструкция, вероятно, будет переработана!
     */
    /// <summary>
    /// Класс предназначенный для выделения данных из множества файлов
    /// </summary>
    public class DateAnalysis
    {
        public Vector[] TestDate
        {
            get;
            protected set;
        }
        public Vector[] ResultDate
        {
            get;
            protected set;
        }

        public readonly Action<Vector> Convert;

        readonly DateInfo info;

        public readonly DateReader Reader;

        public DateAnalysis(string[] fileNames, string[] TestTags, string[] ResultTags, string[] NumberTags, Func<string[], double> fEq, Func<string, double> ToDouble)
        {
            string[] AllTags = TestTags.Union(ResultTags).ToArray();
            CSVReader reader = new CSVReader(AllTags, fileNames);
            string[] DiscreteTags = TestTags.Remove(NumberTags);
            info = new DateInfo(reader, DiscreteTags, ResultTags, fEq);

            TestDate = new Vector[reader.countLine];
            ResultDate = new Vector[reader.countLine];

            for (int i = 0; i < reader.countLine; i++)
            {
                TestDate[i] = new Vector(TestTags.Length + 1);
                ResultDate[i] = new Vector(ResultTags.Length);

                for (int j = 0; j < ResultTags.Length; j++)
                    ResultDate[i][j] = ToDouble(reader[i, ResultTags[j]]);

                for (int j = 0; j < NumberTags.Length; j++)
                    TestDate[i][j] = ToDouble(reader[i, NumberTags[j]]);

                for (int j = NumberTags.Length; j < NumberTags.Length+DiscreteTags.Length; j++)
                    TestDate[i][j] = info[DiscreteTags[j - NumberTags.Length], (reader[i, DiscreteTags[j - NumberTags.Length]])];
                TestDate[i][TestTags.Length] = 1.0;
            }

            Convert = TestDate.Normalization(1.0);
            Reader = new DateReader(info, AllTags, ResultTags, TestTags, NumberTags, DiscreteTags, Convert, ToDouble);
        }
    }
}
