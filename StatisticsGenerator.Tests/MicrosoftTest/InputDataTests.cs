
using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class InputDataTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void NullInputDataFileNameThrowsExceptionTest()
        {
            // Arrange
            const string inputFileName = null;
            const string configurationFile = @"..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);

            // Act
            // ReSharper disable once UnusedVariable
            InputData inputData = new InputData(inputFileName, configuration);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void WhitespaceInputDataFileNameThrowsExceptionTest()
        {
            // Arrange
            const string inputFileName = "  ";
            const string configurationFile = @"..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);

            // Act
            // ReSharper disable once UnusedVariable
            InputData inputData = new InputData(inputFileName, configuration);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConfigurationThrowsExceptionTest()
        {
            // Arrange
            const string inputDataFile = @"..\..\Data\InputData.txt";
            IConfiguration configuration = null;

            // Act
            // ReSharper disable once UnusedVariable
            // ReSharper disable once ExpressionIsAlwaysNull
            InputData inputData = new InputData(inputDataFile, configuration);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatisticalResultsHasSameNumberNonEmptyLinesAsConfigurationFileTest()
        {
            // Arrange
            const string inputDataFile = @"..\..\Data\InputData.txt";
            const string configurationFile = @"..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            int numberNonBlankLinesInConfigurationFile = File.ReadAllLines(configurationFile).Count(line => !string.IsNullOrWhiteSpace(line));

            // Act
            // ReSharper disable once UnusedVariable
            // ReSharper disable once ExpressionIsAlwaysNull
            InputData inputData = new InputData(inputDataFile, configuration);
            string statisticalResults = inputData.CreateStatistics();

            // Assert
            string[] linesArray = statisticalResults.Split('\n');
            int numberLines = linesArray.Length;
            Assert.AreEqual(numberNonBlankLinesInConfigurationFile, numberLines);
        }



    }
}
