using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOData
{
    public class IToDouble
    {
        //!!В случае изменения имени требуется внести изменение в DataFile.cs
        public static double ToDouble(string s)
        {
            return Convert.ToDouble(s);
        }
    }
}
