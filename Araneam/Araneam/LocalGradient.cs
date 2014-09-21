using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;

namespace Araneam
{
    public class LocalGradient:ILocalGradient
    {
        
        public Vector[] Put(Vector error, NeuronLayer[] layers)
        {
            Vector[] lg = new Vector[layers.Length];
            int l, i, j, k;
            Vector t;
            TwoArray inv;

            lg[layers.Length - 1] = error.Multiplication(layers[layers.Length - 1].CalcDer());

            for (l = layers.Length - 2; l >= 0; l--)
            {
                lg[l] = layers[l].CalcDer();
                t = new Vector(lg[l].Length);

                for (i = 0; i < t.Length; i++)
                {
                    inv = layers[l + 1].InversIndex[i];
                    t[i] = 0.0;
                    for (j = 0; j < inv.index.Length; j++)
                    {
                        k = inv.index[j];
                        t[i] += layers[l + 1].neuros[k].weight[inv.subIndex[j]] * lg[l + 1][k];
                    }
                }
                lg[l].Multiplication(t);
            }
            return lg;
        }
    }
}
