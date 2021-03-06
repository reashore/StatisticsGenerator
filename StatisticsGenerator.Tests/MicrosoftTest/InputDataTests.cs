﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

using Operation = StatisticsGenerator.Domain.Operation;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class InputDataTests
    {
        private const double Delta = .001;

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void NullInputDataFileNameThrowsExceptionTest()
        {
            // Arrange
            const string inputFileName = null;
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
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
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
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
            const string inputDataFile = @"..\..\..\Data\InputData.txt";
            IConfiguration configuration = null;

            // Act
            // ReSharper disable once UnusedVariable
            // ReSharper disable once ExpressionIsAlwaysNull
            InputData inputData = new InputData(inputDataFile, configuration);

            // Assert
        }

        [TestMethod]
        public void StatisticalResultsHasLengthAsNonEmptyConfigurationFileLinesTest()
        {
            // Arrange
            const string inputDataFile = @"..\..\..\Data\InputData.txt";
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            int numberNonBlankLinesInConfigurationFile = File.ReadAllLines(configurationFile).Count(line => !string.IsNullOrWhiteSpace(line));

            // Act
            InputData inputData = new InputData(inputDataFile, configuration);
            Dictionary<Operation, double> statisticalResultsDictionary = inputData.CreateStatistics();
            int numberResults = statisticalResultsDictionary.Count;

            // Assert
            Assert.AreEqual(numberNonBlankLinesInConfigurationFile, numberResults);
        }

        [TestMethod]
        public void VerifySpecificStatisticalResultCase1Test()
        {
            // Arrange
            const string inputDataFile = @"..\..\..\Data\InputData.txt";
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            InputData inputData = new InputData(inputDataFile, configuration);
            const double expectedResult = 134444848.0633;
            Operation resultKey = new Operation
            {
                VariableName = "CashPrem",
                OuterAggregation = OuterAggregation.Average,
                PeriodAggregation = PeriodAggregation.Max
            };

            // Act
            Dictionary<Operation, double> statisticalResultsDictionary = inputData.CreateStatistics();
            double actualResult = statisticalResultsDictionary[resultKey];

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void VerifySpecificStatisticalResultCase2Test()
        {
            // Arrange
            const string inputDataFile = @"..\..\..\Data\InputData.txt";
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            InputData inputData = new InputData(inputDataFile, configuration);
            const double expectedResult = 0;
            Operation resultKey = new Operation
            {
                VariableName = "AvePolLoanYield",
                OuterAggregation = OuterAggregation.Min,
                PeriodAggregation = PeriodAggregation.First
            };

            // Act
            Dictionary<Operation, double> statisticalResultsDictionary = inputData.CreateStatistics();
            double actualResult = statisticalResultsDictionary[resultKey];

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void VerifySpecificStatisticalResultCase3Test()
        {
            // Arrange
            const string inputDataFile = @"..\..\..\Data\InputData.txt";
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            InputData inputData = new InputData(inputDataFile, configuration);
            const double expectedResult = -27923645.4400;
            Operation resultKey = new Operation
            {
                VariableName = "ResvAssumed",
                OuterAggregation = OuterAggregation.Min,
                PeriodAggregation = PeriodAggregation.First
            };

            // Act
            Dictionary<Operation, double> statisticalResultsDictionary = inputData.CreateStatistics();
            double actualResult = statisticalResultsDictionary[resultKey];

            // Assert
            Assert.AreEqual(expectedResult, actualResult, Delta);
        }

        [TestMethod]
        public void VerifyAlltatisticalResultCasesTest()
        {
            // Arrange
            const string inputDataFile = @"..\..\..\Data\InputData.txt";
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            InputData inputData = new InputData(inputDataFile, configuration);
            List<double> expectedResultsList = new List<double>
                {
                    0.0000,
                    0.0300,
                    0.0000,
                    0.0400,
                    0.0000,
                    0.0300,
                    0.0000,
                    0.0400,
                    0.0000,
                    0.0300,
                    0.0000,
                    0.0400,
                    0.0000,
                    0.0000,
                    0.0000,
                    0.0000,
                    0.0000,
                    84655914.8600,
                    0.0000,
                    107196660.0000,
                    0.0000,
                    84655947.1300,
                    0.0000,
                    165215335.3800,
                    0.0000,
                    84655936.3733,
                    0.0000,
                    134444848.0633,
                    0.0000,
                    15.2122,
                    0.0000,
                    23816613.4905,
                    -27923645.4400,
                    -36821010.2400,
                    -36821010.2400,
                    -27923645.4400,
                    -27923645.4400,
                    -36811494.2300,
                    -36811494.2300,
                    -27923645.4400,
                    -27923645.4400,
                    -36815937.1733,
                    -36815937.1733,
                    -27923645.4400,
                    0.0000,
                    3910.3626,
                    3910.3626,
                    0.0000
                };
            Dictionary<Operation, double> expectedResultsDictionary = new Dictionary<Operation, double>();
            for (int n = 0; n < configuration.Operations.Count; n++)
            {
                double expectedResult = expectedResultsList[n];
                Operation key = configuration.Operations[n];
                expectedResultsDictionary[key] = expectedResult;
            }

            // Act
            Dictionary<Operation, double> actualResultsDictionary = inputData.CreateStatistics();

            // Assert
            foreach (var key in configuration.Operations)
            {
                double expectedResult = expectedResultsDictionary[key];
                double actualResult = actualResultsDictionary[key];
                Assert.AreEqual(expectedResult, actualResult, Delta);
            }
        }
    }
}
