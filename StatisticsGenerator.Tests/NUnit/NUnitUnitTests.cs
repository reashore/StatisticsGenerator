
using NUnit.Framework;

namespace StatisticsGenerator.Tests.NUnit
{
    [TestFixture]
    public class NUnitTests
    {
        [Test]
        public void NUnitTemplateTest()
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
        public void CalculatorTestCasesTest(int x, int y, int expectedSum)
        {
            // Arrange

            // Act
            int actualSum = Calculator.Add(x, y);

            // Assert
            Assert.AreEqual(expectedSum, actualSum);
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Calculator
    {
        public static int Add(int x, int y)
        {
            return x + y;
        }
    }
}
