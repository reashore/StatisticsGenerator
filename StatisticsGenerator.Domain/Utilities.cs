
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
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (double element in sequence)
            {
                double temp = element - average;
                sum += temp*temp;
            }

            double standardDeviation = Math.Sqrt(sum/count);

            return standardDeviation;
        }

        public static double ComputeStandardDeviationConcurrently(IEnumerable<double> sequence)
        {
            // todo for now compute concurrently just calls non-concurrent version
            return ComputeStandardDeviation(sequence);

            //double average = sequence.Average();
            //int count = sequence.Count();

            //double sum = 0;

            //foreach (double element in sequence)
            //{
            //    double deviation = element - average;
            //    sum += deviation*deviation;
            //}

            //double standardDeviation = Math.Sqrt(sum/count);

            //return standardDeviation;
        }
    }
}
