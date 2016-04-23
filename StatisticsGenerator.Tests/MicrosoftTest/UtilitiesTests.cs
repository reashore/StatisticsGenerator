
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

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
            double expectedStandardDeviation = 0;

            // Act
            double actualStandardDeviation = Utilities.ComputeStandardDeviation(sequence);

            // Assert
            Assert.AreEqual(expectedStandardDeviation, actualStandardDeviation);
        }

        [TestMethod]
        public void StandardDeviationOfRangeTest()
        {
            // Arrange
            IEnumerable<double> sequence = Enumerable.Range(1, 10).Select(n => (double)n);
            double expectedStandardDeviation = 2.8722813232690143;

            // Act
            double actualStandardDeviation = Utilities.ComputeStandardDeviation(sequence);

            // Assert
            Assert.AreEqual(expectedStandardDeviation, actualStandardDeviation);
        }
    }
}
