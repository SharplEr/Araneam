﻿using IOData;

namespace Araneam
{
    public class Simples
    {
        public Result result;

        Simple[] items;

        public Simple this[int i]
        {
            get
            {
                return items[i];
            }
        }

        public int Length
        {
            get
            {
                return items.Length;
            }
        }

        public double avgError;
    }
}
