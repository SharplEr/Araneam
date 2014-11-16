using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using IOData;

namespace Araneam
{
    public class ClassicNetwork : BackPropagationNetwork
    {
        double a = 1.7159, b = 2.0 / 3.0;

        public ClassicNetwork(double r, double t, int one, int two, int inputSize, int outputSize)
            : base(r, t, 3)
        {
            layers[0] = new NeuronLayer(one, inputSize, true, 1, "tanh", a / 2, b);
            layers[0].NormalInitialize(random);
            layers[1] = new NeuronLayer(two, one + 1, true, 1, "tanh", a, b);
            layers[1].NormalInitialize(random);
            layers[2] = new NeuronLayer(outputSize, two + 1, false, 1, "tanh", a, b);
            layers[2].NormalInitialize(random);

            layers[1].CalcInvers(layers[0].WithThreshold);
            layers[2].CalcInvers(layers[1].WithThreshold);
        }

        public override LearnLog EarlyStoppingLearn(bool flag)
        {
            return base.EarlyStoppingLearn(flag);
        }

        public override LearnLog NewLearn(bool flag, int max)
        {
            return base.NewLearn(flag, max);
        }

        public override LearnLog FullLearn()
        {
            return base.FullLearn();
        }

        public override LearnLog FullLearn(double minError)
        {
            return base.FullLearn(minError);
        }

        public NeuronLayer[] getLayers()
        {
            return layers;
        }
    }
}