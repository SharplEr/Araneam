using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace
{
    public class ZMatrix
    {
        int[][] items;

        public ZMatrix(int[][] its)
        {
            items = new int[its.Length][];
            for (int i = 0; i < its.Length; i++)
                items[i] = (int[])its[i].Clone();
        }

        public int[] this[int i]
        {
            get { return items[i]; }
        }

        public int this[int i, int j]
        {
            get { return items[i][j]; }
        }

        /*
        public int TranspositionItem
        {
            return 
        }*/
    }
}
