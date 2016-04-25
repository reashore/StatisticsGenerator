
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain.Aggregations;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class AggregationTests
    {
        private const double Delta = .001;
        private const int UpperLimit = 100;

        [TestMethod]
        public void MinAggregationTest()
        {
            // Arrange
            MinAggregation minAggregation = new MinAggregation();
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1;

            // Act
            double actualResult = minAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void MaxAggregationTest()
        {
            // Arrange
            MaxAggregation maxAggregation = new MaxAggregation();
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 100;

            // Act
            double actualResult = maxAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void FirstAggregationTest()
        {
            // Arrange
            FirstAggregation firstAggregation = new FirstAggregation();
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1;

            // Act
            double actualResult = firstAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void LastAggregationTest()
        {
            // Arrange
            LastAggregation lastAggregation = new LastAggregation();
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 100;

            // Act
            double actualResult = lastAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void AverageAggregationTest()
        {
            // Arrange
            AverageAggregation averageAggregation = new AverageAggregation();
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 50.5;

            // Act
            double actualResult = averageAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void StandardDeviationAggregationTest()
        {
            // Arrange
            StandardDeviationAggregation standardDeviationAggregation = new StandardDeviationAggregation();
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 28.866070047722118;

            // Act
            double actualResult = standardDeviationAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }
    }
}
