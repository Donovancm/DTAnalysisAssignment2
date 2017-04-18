using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmDS
{
    public class Individual 
    {
        private readonly int[] _binary;
        public int _size;

        public Individual(int[] binary)
        {
            _size = binary.Length;
            _binary = binary;
        }
        public Individual(int size, int value)
        {
            _size = size;
            _binary = value.(size);
        }
        public int[] GetPart(int start, int end)
        {
            //return;
        }
    }
}
