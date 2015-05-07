using System;
using ArrayHelper;
using VectorSpace;

namespace IOData
{
    /// <summary>
    /// Класс для представления смешанных данных
    /// </summary>
    [Serializable]
    public class MixData: ICloneable
    {
        public Vector continuous;
        public int[] discrete;

        public MixData(Vector c, int[] d)
        {
            continuous = c;
            discrete = d;
        }

        public Object Clone()
        {
            return new MixData(continuous.CloneOk(), discrete.CloneOk<int[]>());
        }

        public Boolean Mask(Tuple<int, int>[] mask)
        {
            Boolean ans = false;
            int i = 0;
            while ((i < mask.Length) && (ans = discrete[mask[i].Item1] == mask[i].Item2))
                i++;
            
            return ans;
        }
    }
}