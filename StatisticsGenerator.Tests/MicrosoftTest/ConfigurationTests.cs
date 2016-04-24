
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void ConfigurationFileReturnsExpectedNumberOfOperationsTest()
        {
            // Arrange
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);
            int numberNonBlankLinesInConfigurationFile = File.ReadAllLines(configurationFile).Count(line => !string.IsNullOrWhiteSpace(line));

            // Act
            int actualNumberOperations = configuration.Operations.Count;

            // Assert
            Assert.AreEqual(numberNonBlankLinesInConfigurationFile, actualNumberOperations);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MissingConfigurationFileNameThrowsExceptionTest()
        {
            // Arrange
            const string configurationFile = null;
            // ReSharper disable once UnusedVariable
            Configuration configuration = new Configuration(configurationFile);

            // Act

            // Assert
        }

        [TestMethod]
        public void GetPeriodAggregationsForVariableNameDoesNotContainDuplicatesTest()
        {
            // Arrange
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);
            List<string> variableNameList = configuration.GetVariableNames();

            // Act and assert
            foreach (string variableName in variableNameList)
            {
                List<PeriodAggregation> periodAggregationList = configuration.GetPeriodAggregationsForVariable(variableName);
                int numberPeriodAggregations = periodAggregationList.Count;
                int numberDistinctPeriodAggregations = periodAggregationList.Distinct().Count();

                Assert.AreEqual(numberPeriodAggregations, numberDistinctPeriodAggregations);
            }
        }

        [TestMethod]
        public void GetOuterAggregationsForVariableNameDoesNotContainDuplicatesTest()
        {
            // Arrange
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);
            List<string> variableNameList = configuration.GetVariableNames();

            // Act and assert
            foreach (string variableName in variableNameList)
            {
                List<OuterAggregation> outerAggregationList = configuration.GetOuterAggregationsForVariable(variableName);
                int numberOuterAggregations = outerAggregationList.Count;
                int numberDistinctOuterAggregations = outerAggregationList.Distinct().Count();

                Assert.AreEqual(numberOuterAggregations, numberDistinctOuterAggregations);
            }
        }

        [TestMethod]
        public void GetVariableNamesListDoesNotContainDuplicatesTest()
        {
            // Arrange
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);

            // Act
            List<string> variableNameList = configuration.GetVariableNames();
            int numberVariableNames = variableNameList.Count;
            int numberDistinctVariableNames = variableNameList.Count;

            // Assert
            Assert.AreEqual(numberVariableNames, numberDistinctVariableNames);
        }

        [TestMethod]
        public void VariableNamesInConfigurationAreProcessedTest()
        {
            // Arrange
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);
            string variableName1 = "AvePolLoanYield";
            string variableName2 = "CashPrem";
            string variableName3 = "ResvAssumed";
            const bool expectedIsVariable1Processed = true;
            const bool expectedIsVariable2Processed = true;
            const bool expectedIsVariable3Processed = true;

            // Act
            bool actualIsVariable1Processed = configuration.IsVariableProcessed(variableName1);
            bool actualIsVariable2Processed = configuration.IsVariableProcessed(variableName2);
            bool actualIsVariable3Processed = configuration.IsVariableProcessed(variableName3);

            // Assert
            Assert.AreEqual(expectedIsVariable1Processed, actualIsVariable1Processed);
            Assert.AreEqual(expectedIsVariable2Processed, actualIsVariable2Processed);
            Assert.AreEqual(expectedIsVariable3Processed, actualIsVariable3Processed);
        }
    }
}
