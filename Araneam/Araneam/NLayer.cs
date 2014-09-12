using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using IOData;

namespace Araneam
{
    /// <summary>
    /// N-слой RN-сети.
    /// </summary>
    public class NLayer: Plug<int[]>
    {
        //Фактически N-нейроны. Первый элемент Tuple равен номеру коориданты, второй -- значению.
        public Tuple<int, int>[][] NNeurons;
        //Вес каждого N-нейрона, зависящий от соблюдения x-критерия.
        public double[] weights;

        public NLayer(Func<int, Tuple<int,int>[]> f, double[] w)
        {
            int n = w.Length;

            NNeurons = new Tuple<int,int>[n][];

            for (int i = 0; i < n; i++)
                NNeurons[i] = f(i);
            weights = w;

            dimension = weights.Length;
        }

        public override Vector Calc(int[] input)
        {
            Vector ans = new Vector(NNeurons.Length, (i)=> (input.Mask(NNeurons[i]))?(2*weights[i]-1):-1);
            //Vector ans = new Vector(NNeurons.Length, (i) => (input.Mask(NNeurons[i])) ? weights[i] : -weights[i]);

            return ans;
        }


    }
}
