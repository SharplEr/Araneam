﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using MyParallel;

namespace IODate
{
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

        public readonly Action<Vector> convert;

        public DateAnalysis(string[] fileNames, string[] TestTags, string[] ResultTags, string[] NumberTags, Func<string[], double> fEq, Func<string, double> ToDouble)
        {
            CSVReader reader = new CSVReader(TestTags.Union(ResultTags).ToArray(), fileNames);
            string[] DiscreteTags = TestTags.Remove(NumberTags);
            DateInfo info = new DateInfo(reader, DiscreteTags, ResultTags, fEq);

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

            convert = TestDate.Normalization();
        }
    }
}
