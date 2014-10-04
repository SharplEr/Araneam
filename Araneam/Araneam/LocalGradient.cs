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
        Vector[] temp;
        Vector[] localGrads;

        public LocalGradient(NeuronLayer[] layers)
        {
            temp = new Vector[layers.Length-1];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = new Vector(layers[i].neuros.Length);

            localGrads = new Vector[layers.Length];
            for (int i = 0; i < localGrads.Length; i++)
                localGrads[i] = new Vector(layers[i].neuros.Length);
        }

        public Vector[] Put(Vector error, NeuronLayer[] layers)
        {
            int l, i, j, k;
            Vector t;
            TwoArray inv;

            layers[layers.Length - 1].CalcDer(localGrads[layers.Length - 1]).Multiplication(error);

            for (l = layers.Length - 2; l >= 0; l--)
            {
                layers[l].CalcDer(localGrads[l]);

                t = temp[l];

                for (i = 0; i < t.Length; i++)
                {
                    inv = layers[l + 1].InversIndex[i];
                    t[i] = 0.0;
                    for (j = 0; j < inv.index.Length; j++)
                    {
                        k = inv.index[j];
                        t[i] += layers[l + 1].neuros[k].weight[inv.subIndex[j]] * localGrads[l + 1][k];
                    }
                }
                localGrads[l].Multiplication(t);
            }
            return localGrads;
        }
    }
}
