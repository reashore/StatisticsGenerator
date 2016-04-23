
using System.Threading;
using NUnit.Framework;

namespace StatisticsGenerator.Tests.NUnit
{
    [TestFixture]
    public class NUnitTests
    {
        [Test]
        [Category("Basic")]
        public void TestNUnitTest1()
        {
            // assert
            Assert.IsTrue(true);
        }

        [Test]
        [TestCase(1, 2, 3)]
        [TestCase(4, 5, 9)]
        [TestCase(3, 5, 8)]
        [TestCase(2, 5, 7)]
        [Category("Composite")]
        public void TestNUnitTestCases(int x, int y, int expectedSum)
        {
            // arrange
            Calculator calculator = new Calculator();

            //act
            int actualSum = calculator.Add(x, y);

            // assert
            Assert.AreEqual(expectedSum, actualSum);
        }

        [Test]
        [Category("Timing")]
        [MaxTime(2000)]
        public void TestMaxTimePasses()
        {
            // act
            Thread.Sleep(1000);

            // assert
            Assert.IsTrue(true);
        }

        [Test]
        [Category("Timing")]
        [Timeout(2000)]
        public void TestTimeoutPasses()
        {
            // act
            Thread.Sleep(1000);

            // assert
            Assert.IsTrue(true);
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
