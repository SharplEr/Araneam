using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Araneam
{
    public class LearnLog
    {
        /// <summary>
        /// Число итераций метода обучения
        /// </summary>
        public readonly int LearnIteration;
        /// <summary>
        /// Число эпох обучения
        /// </summary>
        public readonly int LearnEpoch;
        /// <summary>
        /// Средняя норма ошибки
        /// </summary>
        public readonly double Error;

        public LearnLog(int i, int ep)
        {
            LearnIteration = i;
            LearnEpoch = ep;
        }

        public LearnLog(int i, int ep, double er)
        {
            LearnIteration = i;
            LearnEpoch = ep;
            Error = er;
        }
    }
}
