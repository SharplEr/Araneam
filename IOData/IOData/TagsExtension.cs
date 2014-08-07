using System;
using System.Collections.Generic;
using System.Linq;

namespace IOData
{
    public static class TagsExtension
    {        
        public static string[] Remove(this string[] t1, string[] t2)
        {
            List<string> ans = t1.ToList();

            for (int i = 0; i < t2.Length; i++)
                ans.Remove(t2[i]);

            return ans.ToArray();
        }

        public static Boolean Mask(this int[] discrete, Tuple<int, int>[] mask)
        {
            Boolean ans = false;
            int i = 0;
            while ((i < mask.Length) && (ans = discrete[mask[i].Item1] == mask[i].Item2))
                i++;

            return ans;
        }
    }
}
