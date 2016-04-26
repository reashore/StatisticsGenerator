
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void StandardDeviationOfConstantSequenceIsZeroTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Repeat(1, 10).Select(n => (double)n);
            const double expectedStandardDeviation = 0;

            // Act
            double actualStandardDeviation = sequence.StandardDeviation();

            // Assert
            Assert.AreEqual(expectedStandardDeviation, actualStandardDeviation);
        }

        [TestMethod]
        public void StandardDeviationOfRangeTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Range(1, 10).Select(n => (double)n);
            const double expectedStandardDeviation = 2.8722813232690143;

            // Act
            double actualStandardDeviation = sequence.StandardDeviation();

            // Assert
            Assert.AreEqual(expectedStandardDeviation, actualStandardDeviation);
        }

        [TestMethod]
        public void ConcurrentAndNonconcurrentAggregationsAreEqualTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Range(1, 10000000).Select(n => (double)n);
            const double delta = .0001;

            // Act
            // ReSharper disable PossibleMultipleEnumeration
            double sumWithoutConcurrency = sequence.Sum();
            double sumWithConcurrency = sequence.AsParallel().Sum();
            // ReSharper restore PossibleMultipleEnumeration

            // Assert
            Assert.AreEqual(sumWithoutConcurrency, sumWithConcurrency, delta);
        }

        [TestMethod]
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
            Assert.IsTrue(concurrentCalculationIsFaster);
        }
    }
}
