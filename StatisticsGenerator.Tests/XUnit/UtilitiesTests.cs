
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using StatisticsGenerator.Domain;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace StatisticsGenerator.Tests.XUnit
{
    public class UtilitiesTests
    {
        [Fact]
        public void StandardDeviationOfConstantSequenceIsZeroTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Repeat(1, 10).Select(n => (double)n);
            const double expectedStandardDeviation = 0;

            // Act
            double actualStandardDeviation = sequence.StandardDeviation();

            // Assert
            Assert.Equal(expectedStandardDeviation, actualStandardDeviation);
        }

        [Fact]
        public void StandardDeviationOfRangeTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Range(1, 10).Select(n => (double)n);
            const double expectedStandardDeviation = 2.8722813232690143;

            // Act
            double actualStandardDeviation = sequence.StandardDeviation();

            // Assert
            Assert.Equal(expectedStandardDeviation, actualStandardDeviation);
        }

        [Fact]
        public void ConcurrentAndNonconcurrentAggregationsAreEqualTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Range(1, 10000000).Select(n => (double)n);
            const int precision = 2;

            // Act
            // ReSharper disable PossibleMultipleEnumeration
            double sumWithoutConcurrency = sequence.Sum();
            double sumWithConcurrency = sequence.AsParallel().Sum();
            // ReSharper restore PossibleMultipleEnumeration

            // Assert
            Assert.Equal(sumWithoutConcurrency, sumWithConcurrency, precision);
        }

        [Fact]
        public void ConcurrentAggregationIsSlowerThanNonconcurrentAggregationTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Range(1, 10000000).Select(n => (double)n);

            // Act
            // ReSharper disable PossibleMultipleEnumeration
            // ReSharper disable UnusedVariable
            Stopwatch stopwatch = Stopwatch.StartNew();
            double sumWithoutConcurrency = sequence.Sum();
            TimeSpan elapsedWithoutConcurrency = stopwatch.Elapsed;

            stopwatch = Stopwatch.StartNew();
            double sumWithConcurrency = sequence.AsParallel().Sum();
            TimeSpan elapsedWithConcurrency = stopwatch.Elapsed;
            // ReSharper restore UnusedVariable
            // ReSharper restore PossibleMultipleEnumeration

            // Assert
            bool concurrentCalculationIsFaster = (elapsedWithConcurrency >= elapsedWithoutConcurrency);
            Assert.True(concurrentCalculationIsFaster);
        }
    }
}
