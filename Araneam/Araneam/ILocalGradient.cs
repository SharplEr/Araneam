using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;

namespace Araneam
{
    public interface ILocalGradient
    {
        Vector[] Put(Vector error, NeuronLayer[] nl);
    }
}
