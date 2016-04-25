﻿
using Xunit;

namespace StatisticsGenerator.Tests.XUnit
{
    public class XUnitTests
    {
        [Fact]
        public void XUnitTemplateTest()
        {
            //Arrange

            // Act

            // Assert
            Assert.Equal(true, true);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(4, 5, 9)]
        [InlineData(3, 5, 8)]
        [InlineData(2, 5, 7)]
        public void CalculatorTestCasesTest(int x, int y, int expectedSum)
        {
            // Arrange

            // Act
            int actualSum = Calculator.Add(x, y);

            // Assert
            Assert.Equal(expectedSum, actualSum);
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
