﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmDS
{
    public class Individual
    {
        public int[] _binary;
        public int _size;

        public Individual(int[] binary)
        {
            _size = binary.Length;
            _binary = binary;
        }

        public Individual(int size, int value)
        {
            _size = size;
            _binary = value.ToBitsArray(size);
        }

        public int[] GetPartOfParent(int start, int end)
        {
            var result = new List<int>();
            for (var i = start; i < end; i++)
            {
                result.Add(_binary[i]);
            }
            return result.ToArray();
        }

        public void SwitchPosition(int position)
        {
            _binary[position] = _binary[position] == 0 ? 1 : 0;
        }

        public int ToBinary()
        {
            return _binary.ToBinary();
        }

        public override string ToString()
        {
            return _binary.Aggregate("", (currentstring, index) => currentstring + index.ToString());
        }
    }
}
