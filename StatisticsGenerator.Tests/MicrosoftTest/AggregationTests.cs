
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain.Aggregations;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class TemplateTests
    {
        [TestMethod]
        public void TestTemplate()
        {
            // Arrange
            const int expected = 42;

            // Act
            const int actual = 42;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class AggregationTests
    {
        private const bool UseConcurrency = false;
        private const int UpperLimit = 100;

        [TestMethod]
        public void MinAggregationTest()
        {
            // Arrange
            MinAggregation minAggregation = new MinAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1;

            // Act
            double actualResult = minAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void MaxAggregationTest()
        {
            // Arrange
            MaxAggregation maxAggregation = new MaxAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 100;

            // Act
            double actualResult = maxAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void FirstAggregationTest()
        {
            // Arrange
            FirstAggregation firstAggregation = new FirstAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1;

            // Act
            double actualResult = firstAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void LastAggregationTest()
        {
            // Arrange
            LastAggregation lastAggregation = new LastAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 100;

            // Act
            double actualResult = lastAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void AverageAggregationTest()
        {
            // Arrange
            AverageAggregation averageAggregation = new AverageAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 50.5;

            // Act
            double actualResult = averageAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void StandardDeviationAggregationTest()
        {
            // Arrange
            StandardDeviationAggregation standardDeviationAggregation = new StandardDeviationAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 28.866070047722118;

            // Act
            double actualResult = standardDeviationAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }

    [TestClass]
    public class ConcurrentAggregationTests
    {
        private const bool UseConcurrency = true;
        private const int UpperLimit = 1000000;

        [TestMethod]
        public void MinAggregationTest()
        {
            // Arrange
            MinAggregation minAggregation = new MinAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1;

            // Act
            double actualResult = minAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void MaxAggregationTest()
        {
            // Arrange
            MaxAggregation maxAggregation = new MaxAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1000000;

            // Act
            double actualResult = maxAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void FirstAggregationTest()
        {
            // Arrange
            FirstAggregation firstAggregation = new FirstAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1;

            // Act
            double actualResult = firstAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void LastAggregationTest()
        {
            // Arrange
            LastAggregation lastAggregation = new LastAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 1000000;

            // Act
            double actualResult = lastAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void AverageAggregationTest()
        {
            // Arrange
            AverageAggregation averageAggregation = new AverageAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 500000.5;

            // Act
            double actualResult = averageAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void StandardDeviationAggregationTest()
        {
            // Arrange
            StandardDeviationAggregation standardDeviationAggregation = new StandardDeviationAggregation(UseConcurrency);
            IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
            const double expectedResult = 288675.1345958226;

            // Act
            double actualResult = standardDeviationAggregation.Aggregate(sequence);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }

    //[TestClass]
    //public class ConcurrentAggregationIsFasterThanRegularAggregationTest
    //{
    //    private const int UpperLimit = 1000000;

    //    [TestMethod]
    //    public void AverageAggregationTest()
    //    {
    //        // Arrange
    //        AverageAggregation averageAggregationWithoutConcurrency = new AverageAggregation(false);
    //        AverageAggregation averageAggregationWithConcurrency = new AverageAggregation(true);
    //        IEnumerable<double> sequence = Enumerable.Range(1, UpperLimit).Select(n => (double)n);
    //        const double expectedResult = 500000.5;

    //        // Act
    //        Stopwatch stopwatch1 = Stopwatch.StartNew();
    //        double actualResultWithoutConcurrency = averageAggregationWithoutConcurrency.Aggregate(sequence);
    //        TimeSpan elapsedWithoutConcurrency = stopwatch1.Elapsed;

    //        Stopwatch stopwatch2 = Stopwatch.StartNew();
    //        double actualResultWithConcurrency = averageAggregationWithConcurrency.Aggregate(sequence);
    //        TimeSpan elapsedWithConcurrency = stopwatch2.Elapsed;

    //        // Assert
    //        Assert.AreEqual(expectedResult, actualResultWithoutConcurrency);
    //        Assert.AreEqual(expectedResult, actualResultWithConcurrency);

    //        // concurrent aggregation is slower than regular aggregation!!!
    //        Assert.IsTrue(elapsedWithConcurrency < elapsedWithoutConcurrency);
    //    }
    //}
}
