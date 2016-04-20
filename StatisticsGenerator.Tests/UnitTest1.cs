
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests
{
    // todo add tests that use NUnit
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

        // Call ReadConfiguration implicitly
        [TestMethod]
        public void TestMethod2()
        {
            // Arrange
            Configuration configuration = new Configuration();
            const string basePath = @"..\..\Data";
            string configurationFile = Path.Combine(basePath, "Configuration.txt");
            // todo count lines in configuration file
            const int expectedNumberOperations = 36;
            // todo mock IConfiguration?

            // Act
            int actualNumberOperations1 = configuration.ReadConfigurationFile(configurationFile);

            // Assert
            int actualNumberOperations2 = configuration.Operations.Count;

            Assert.AreEqual(expectedNumberOperations, actualNumberOperations1);
            Assert.AreEqual(actualNumberOperations1, actualNumberOperations2);
        }

        // Call ReadConfiguration explicitly
        [TestMethod]
        public void TestMethod3()
        {
            // Arrange
            Configuration configuration = new Configuration();
            const string basePath = @"..\..\Data";
            string configurationFile = Path.Combine(basePath, "Configuration.txt");
            // todo count lines in configuration file
            const int expectedNumberOperations = 36;
            // todo mock IConfiguration?

            // Act
            int actualNumberOperations1 = configuration.ReadConfigurationFile(configurationFile);

            // Assert
            int actualNumberOperations2 = configuration.Operations.Count;

            Assert.AreEqual(expectedNumberOperations, actualNumberOperations1);
            Assert.AreEqual(actualNumberOperations1, actualNumberOperations2);
        }

        // test that correct statistics are genearted when the columns are in a different order
        // test standard deviation calculation
        // test that exceptions are throw
    }
}
