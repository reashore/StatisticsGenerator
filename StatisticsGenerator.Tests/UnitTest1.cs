using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StatisticsGenerator.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const int expected = 1;
            const int actual = 1;
            
            Assert.AreEqual(expected, actual);
        }
    }
}
