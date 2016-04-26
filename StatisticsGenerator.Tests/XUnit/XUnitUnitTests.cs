
using System;
using Xunit;
using Xunit.Abstractions;

namespace StatisticsGenerator.Tests.XUnit
{
    public sealed class XUnitTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XUnitTests(ITestOutputHelper testOutputHelper)
        {
            // constructor is run before each test
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        [Trait("Category", "Basic")]
        public void XUnitTemplateTest()
        {
            //Arrange
            _testOutputHelper.WriteLine("Arrange");
            const double expectedResult = 3.00001;

            // Act
            _testOutputHelper.WriteLine("Act");
            const double actualResult = 3.00000;

            // Assert
            _testOutputHelper.WriteLine("Assert");
            Assert.Equal(true, true);
            Assert.Equal(expectedResult, actualResult, 3);
            Assert.NotEqual(1, 2);
            Assert.Equal("FOO", "Foo", ignoreCase: true);
            Assert.Contains("foo", "foobar");
            Assert.StartsWith("foo", "foobar");
            Assert.False(false);
            //Assert.Null(null);
        }

        [Fact]
        [Trait("Category", "Exception")]
        public void CheckThatExceptionIsThrownTest()
        {
            Assert.Throws<Exception>(() => Calculator.ThrowException());
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

        public void Dispose()
        {
            // cleanup after each test
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Calculator
    {
        public static int Add(int x, int y)
        {
            return x + y;
        }

        public static void ThrowException()
        {
            throw new Exception("This is an exception");
        }
    }
}
