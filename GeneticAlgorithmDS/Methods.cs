using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmDS
{
    public static class Methods
    {
        public static int[] ToBitsArray(this int value, int size)
        {
            var bits = new int[size];

            for (var i = size - 1; i >= 0; i--)
            {
                var bit = value - (int)Math.Pow(2, i);
                bits[Math.Abs(i + 1 - size)] = bit >= 0 ? 1 : 0;
                value = bit >= 0 ? bit : value;
            }
            return bits;
        }
    }
}
