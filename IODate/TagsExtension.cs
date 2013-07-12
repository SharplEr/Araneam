using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IODate
{
    public static class TagsExtension
    {
        /*
        public static string[] Union(this string[] t1, string[] t2)
        {
            List<string> ans = t1.ToList();
            ans.AddRange(t2);
            ans.Sort();
            int i = 1;
            while(i<ans.Count)
                if (ans[i - 1] == ans[i]) ans.RemoveAt(i);

            return ans.ToArray();
        }*/
        
        public static string[] Remove(this string[] t1, string[] t2)
        {
            List<string> ans = t1.ToList();

            for (int i = 0; i < t2.Length; i++)
                ans.Remove(t2[i]);

            return ans.ToArray();
        }
    }
}
