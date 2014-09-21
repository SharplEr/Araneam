using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using IOData;

namespace Araneam
{
    public class Simples
    {
        public Result result;

        Simple[] items;

        public Simple this[int i]
        {
            get
            {
                return items[i];
            }
        }

        public int Length
        {
            get
            {
                return items.Length;
            }
        }

        public double avgError;
    }
}
