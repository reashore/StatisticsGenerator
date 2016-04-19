
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            const int expected = 1;

            // Act
            const int actual = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Arrange
            Configuration configuration = new Configuration();
            const string basePath = @"..\..\Data";
            string configurationFile = Path.Combine(basePath, "Configuration.txt");
            const int expectedNumberOperations = 36;
            // todo mock IConfiguration?

            // Act
            configuration.ReadConfigurationFile(configurationFile);

            // Assert
            int actualNumberOperations = configuration.Operations.Count;

            Assert.AreEqual(expectedNumberOperations, actualNumberOperations);
        }
    }
}
