using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.XUnit
{
    public class ConfigurationTests
    {
        [Fact]
        public void ConfigurationFileReturnsExpectedNumberOfOperationsTest()
        {
            // Arrange
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            Configuration configuration = new Configuration(configurationFile);
            int numberNonBlankLinesInConfigurationFile = File.ReadAllLines(configurationFile).Count(line => !string.IsNullOrWhiteSpace(line));

            // Act
            int actualNumberOperations = configuration.Operations.Count;

            // Assert
            Assert.Equal(numberNonBlankLinesInConfigurationFile, actualNumberOperations);
        }

        [Fact]
        public void MissingConfigurationFileNameThrowsExceptionTest()
        {
            // Arrange

            // Act

            // Assert
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => new Configuration(configurationFile: null));
            Assert.Equal(argumentNullException.ParamName, "configurationFile");
        }

        [Fact]
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

                Assert.Equal(numberPeriodAggregations, numberDistinctPeriodAggregations);
            }
        }

        [Fact]
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

                Assert.Equal(numberOuterAggregations, numberDistinctOuterAggregations);
            }
        }

        [Fact]
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
            Assert.Equal(numberVariableNames, numberDistinctVariableNames);
        }

        [Fact]
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
            Assert.Equal(expectedIsVariable1Processed, actualIsVariable1Processed);
            Assert.Equal(expectedIsVariable2Processed, actualIsVariable2Processed);
            Assert.Equal(expectedIsVariable3Processed, actualIsVariable3Processed);
        }
    }
}
