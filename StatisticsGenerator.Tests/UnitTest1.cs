
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests
{
    // todo add tests that use NUnit (get code from GitHub)
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
            const string basePath = @"..\..\Data";
            string configurationFile = Path.Combine(basePath, "Configuration.txt");
            Configuration configuration = new Configuration(configurationFile);
            // todo count lines in configuration file
            const int expectedNumberOperations = 36;
            // todo mock IConfiguration?

            // Act
            int actualNumberOperations1 = configuration.Operations.Count;

            // Assert
            int actualNumberOperations2 = configuration.Operations.Count;

            Assert.AreEqual(expectedNumberOperations, actualNumberOperations1);
            Assert.AreEqual(actualNumberOperations1, actualNumberOperations2);
        }

        // test that correct statistics are genearted when the columns are in a different order
        // test standard deviation calculation
        // test that exceptions are throw
        // create DataLine class and create tests for this class
        // create exception if column name is missing: ScenarioId, VariableName

        // parse header and verify results
        // parse data lines and verify result
    }
}
