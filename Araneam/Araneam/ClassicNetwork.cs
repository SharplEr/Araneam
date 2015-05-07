using System;

namespace Araneam
{
    public class ClassicNetwork : BackPropagationNetwork
    {
        readonly double a = 1.7159;
        readonly double b = 2.0 / 3.0;

        public ClassicNetwork(double r, double t, int one, int two, int inputSize, int outputSize)
            : base(r, t, 3)
        {
            if (one == int.MaxValue) throw new ArgumentException("ClassicNetwork: too big network");
            if (two == int.MaxValue) throw new ArgumentException("ClassicNetwork: too big network");

            layers[0] = new NeuronLayer(one, inputSize, true, 1, "tanh", a , b);
            layers[0].NormalInitialize(random);
            layers[1] = new NeuronLayer(two, one + 1, true, 1, "tanh", a, b);
            layers[1].NormalInitialize(random);
            layers[2] = new NeuronLayer(outputSize, two + 1, false, 1, "tanh", a, b);
            layers[2].NormalInitialize(random);

            layers[1].CalcInvers(layers[0].WithThreshold);
            layers[2].CalcInvers(layers[1].WithThreshold);
        }

        public ClassicNetwork(double r, double t, int[] layer, int inputSize, int outputSize): base(r, t, layer.Length+1)
        {
            layers[0] = new NeuronLayer(layer[0], inputSize, true, 1, "tanh", a , b);
            layers[0].NormalInitialize(random);

            for (int i = 1; i < layer.Length; i++)
            {
                layers[i] = new NeuronLayer(layer[i], layer[i - 1] + 1, true, 1, "tanh", a, b);
                layers[i].NormalInitialize(random);
            }

            layers[layer.Length] = new NeuronLayer(outputSize, layer[layer.Length - 1] + 1, false, 1, "tanh", a, b);
            layers[layer.Length].NormalInitialize(random);

            for (int i = 1; i < layers.Length; i++)
                layers[i].CalcInvers(layers[i - 1].WithThreshold);
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