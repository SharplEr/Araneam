using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOData
{
    public class Results
    {
        Result[] items;

        int maxNumber;

        public Results(Func<int, Result> f, int n)
        {
            items = new Result[n];

            if (n < 1) throw new ArgumentNullException();

            for (int i = 0; i < n; i++)
                items[i] = f(i);

            maxNumber = items[0].MaxNumber;
        }

        public Result this[int i]
        {
            get { return items[i]; }
        }

        public int Length
        {
            get { return items.Length; }
        }
    }
}
