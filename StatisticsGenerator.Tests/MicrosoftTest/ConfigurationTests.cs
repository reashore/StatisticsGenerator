
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class TemplateTests2
    {
        [TestMethod]
        public void TestTemplate()
        {
            // Arrange
            const int expected = 42;

            // Act
            const int actual = 42;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void TestMethod2()
        {
            // Arrange
            const string configurationFile = @"..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);
            // todo count lines in configuration file
            const int expectedNumberOperations = 36;

            // Act
            int actualNumberOperations = configuration.Operations.Count;

            // Assert
            Assert.AreEqual(expectedNumberOperations, actualNumberOperations);
        }

        // throws exception if filename is null


    }
}
