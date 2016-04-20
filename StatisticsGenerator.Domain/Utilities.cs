
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain
{
    public static class Utilities
    {
        public static double ComputeStandardDeviation(IEnumerable<double> sequence)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            double average = sequence.Average();

            // ReSharper disable once PossibleMultipleEnumeration
            int count = sequence.Count();

            double sum = 0;

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (double element in sequence)
            {
                double temp = element - average;
                sum += temp*temp;
            }

            double standardDeviation = Math.Sqrt(sum/count);

            return standardDeviation;
        }

        // todo compute concurrently
        public static double ComputeStandardDeviationConcurrently(IEnumerable<double> sequence)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            double average = sequence.Average();
            // ReSharper disable once PossibleMultipleEnumeration
            int count = sequence.Count();

            double sum = 0;

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (double element in sequence)
            {
                double deviation = element - average;
                sum += deviation*deviation;
            }

            double standardDeviation = Math.Sqrt(sum/count);

            return standardDeviation;
        }
    }
}
