
using NUnit.Framework;

namespace StatisticsGenerator.Tests.NUnit
{
    [TestFixture]
    public class NUnitTests
    {
        [Test]
        public void TestNUnitTest1()
        {
            //Arrange

            // Act

            // Assert
            Assert.IsTrue(true);
        }

        [Test]
        [TestCase(1, 2, 3)]
        [TestCase(4, 5, 9)]
        [TestCase(3, 5, 8)]
        [TestCase(2, 5, 7)]
        public void TestCalculatorTestCases(int x, int y, int expectedSum)
        {
            // Arrange
            Calculator calculator = new Calculator();

            // Act
            int actualSum = calculator.Add(x, y);

            // Assert
            Assert.AreEqual(expectedSum, actualSum);
        }
    }

    // Domain

    public class Calculator
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
