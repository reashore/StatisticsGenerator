﻿
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

    public static class DoubleExtensions
    {
        private const double Digits3 = 0.001;
        private const double Digits4 = 0.0001;
        private const double Digits5 = 0.00001;
        private const double Digits6 = 0.000001;
        private const double Digits7 = 0.0000001;
        private const double Digits8 = 0.00000001;

        public static bool Equals3DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < Digits3;
        }

        public static bool Equals4DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < Digits4;
        }

        public static bool Equals5DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < Digits5;
        }

        public static bool Equals6DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < Digits6;
        }

        public static bool Equals7DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < Digits7;
        }

        public static bool Equals8DigitPrecision(this double left, double right)
        {
            return Math.Abs(left - right) < Digits8;
        }
    }


}
