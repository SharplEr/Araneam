using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;

namespace IODate
{
    public class DateReader
    {
        DateInfo info;
        string[] allTags;
        string[] resultTags;
        string[] testTags;
        string[] numberTags;
        string[] discreteTags;
        public Vector[] TestDate
        { get; protected set; }
        public Vector[] ResultDate
        { get; protected set; }

        Action<Vector> convert;
        Func<string, double> ToDouble;

        public DateReader(
            DateInfo inf,
            string[] aTags,
            string[] rTags,
            string[] tTags,
            string[] nTags,
            string[] dTags,
            Action<Vector> conv,
            Func<string, double> td
            )
        {
            allTags = aTags;
            resultTags = rTags;
            testTags = tTags;
            numberTags = nTags;
            discreteTags = dTags;
            info = inf;
            convert = conv;
            ToDouble = td;
        }

        public void Read(string name)
        {
            CSVReader reader = new CSVReader(name);
            TestDate = new Vector[reader.countLine];
            ResultDate = new Vector[reader.countLine];

            for (int i = 0; i < reader.countLine; i++)
            {
                TestDate[i] = new Vector(testTags.Length + 1);
                ResultDate[i] = new Vector(resultTags.Length);

                for (int j = 0; j < resultTags.Length; j++)
                    ResultDate[i][j] = ToDouble(reader[i, resultTags[j]]);

                for (int j = 0; j < numberTags.Length; j++)
                    TestDate[i][j] = ToDouble(reader[i, numberTags[j]]);

                for (int j = numberTags.Length; j < numberTags.Length + discreteTags.Length; j++)
                    TestDate[i][j] = info[discreteTags[j - numberTags.Length], (reader[i, discreteTags[j - numberTags.Length]])];
                TestDate[i][testTags.Length] = 1.0;

                convert(TestDate[i]);
            }
        }
    }
}
