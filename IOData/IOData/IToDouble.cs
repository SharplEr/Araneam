using System;

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
