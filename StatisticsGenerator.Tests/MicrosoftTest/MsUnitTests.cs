
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    // test that correct statistics are genearted when the columns are in a different order
    // test standard deviation calculation
    // test that exceptions are throw
    // create DataLine class and create tests for this class
    // create exception if column name is missing: ScenarioId, VariableName

    // parse header and verify results
    // parse data lines and verify result
    // if variable name is not in configuration then variable is not aggregated
    // test individual aggregation strategies

    // test Configuration class
    // test HeaderLine class
    // test DataLine class
    // test InputData class
    // test Aggregation classes - Done

    [TestClass]
    public class MsUnitTests
    {
        // test template
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
            const string configurationFile = @"..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);
            // todo count lines in configuration file
            const int expectedNumberOperations = 36;

            // Act
            int actualNumberOperations = configuration.Operations.Count;

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expectedNumberOperations, actualNumberOperations);
        }
    }
}
