using VectorSpace;

namespace Araneam
{
    public interface ILocalGradient
    {
        Vector[] Put(Vector error, NeuronLayer[] nl);
    }
}
