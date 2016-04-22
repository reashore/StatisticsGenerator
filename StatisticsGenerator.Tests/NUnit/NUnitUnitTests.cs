using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace StatisticsGenerator.Tests.NUnit
{
    [SetUpFixture]
    public class TestSetUpClass
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            Debug.WriteLine("Test run starting.");
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            Debug.WriteLine("Test run ending.");
        }
    }

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
        //[NUnit.Framework.Ignore]
        [Category("Ignored")]
        public void TestIgnore()
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

        //[Test]
        //[Category("Timing")]
        //[MaxTime(2000)]
        //public void TestMaxTimeFails()
        //{
        //    // act
        //    Thread.Sleep(3000);

        //    // assert
        //    NUnit.Framework.Assert.Fail();
        //}

        [Test]
        [Category("Timing")]
        [Timeout(2000)]
        //[Microsoft.VisualStudio.TestTools.UnitTesting.Timeout(2000)]
        public void TestTimeoutPasses()
        {
            // act
            Thread.Sleep(1000);

            // assert
            Assert.IsTrue(true);
        }

        //[Test]
        //[Category("Timing")]
        //[NUnit.Framework.Timeout(2000)]
        //public void TestTimeoutFails()
        //{
        //    // act
        //    Thread.Sleep(3000);

        //    // assert
        //    NUnit.Framework.Assert.Fail();
        //}
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
