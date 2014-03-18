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
    }
}
