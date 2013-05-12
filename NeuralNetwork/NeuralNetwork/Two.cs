using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork
{
    public class TwoArray
    {
        public int[] index, subIndex;
        public TwoArray(int[] a, int[] b)
        {
            index = a;
            subIndex = b;
        }
    }

    public class TwoList
    {
        public List<int> index, subIndex;

        public TwoList()
        {
            index = new List<int>();
            subIndex = new List<int>();
        }
    }
}
