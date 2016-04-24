
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain
{
    public static class EnumerableExtensions
    {
        public static double StandardDeviation(this IEnumerable<double> sequence)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            double average = sequence.Average();

            // ReSharper disable once PossibleMultipleEnumeration
            int count = sequence.Count();

            double sum = 0;

            // ReSharper disable once PossibleMultipleEnumeration
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (double element in sequence)
            {
                double temp = element - average;
                sum += temp * temp;
            }

            double standardDeviation = Math.Sqrt(sum / count);

            return standardDeviation;
        }
    }
}