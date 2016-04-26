
using System;
using System.Collections.Generic;
using Xunit;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.XUnit
{
    public class DataLineTests
    {
        private const int Precision = 3;

        [Fact]
        public void NullDataLineThrowsExceptionTest()
        {
            // Arrange
            // An empty column mapping dictionary is sufficient for this test
            Dictionary<string, int> columnMappingDictionary = new Dictionary<string, int>();
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);

            // Act

            // Assert
            Assert.Throws<Exception>(() => new DataLine(null, columnMappingDictionary, configuration));
        }

        [Fact]
        public void WhitespaceDataLineThrowsExceptionTest()
        {
            // Arrange
            // An empty column mapping dictionary is sufficient for this test
            Dictionary<string, int> columnMappingDictionary = new Dictionary<string, int>();
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);

            // Act

            // Assert
            Assert.Throws<Exception>(() => new DataLine("  ", columnMappingDictionary, configuration));
        }

        [Fact]
        public void DataLineIsParsedCorrectly1Test()
        {
            // Arrange
            const string headerLine =  "ScenId	VarName	Value000	Value001	Value002	Value003	Value004	Value005";
            DataHeader dataheader = new DataHeader(headerLine);
            Dictionary<string, int> columnMappingsDictionary = dataheader.ColumnMappings;
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            const string line = "1	AvePolLoanYield	0.00	0.04	0.04	0.04	0.04	0.03";
            const int expectedScenarioId = 1;
            const string expectedVariableName = "AvePolLoanYield";
            double[] expectedPeriodValueArray = {0.00, 0.04, 0.04, 0.04, 0.04, 0.03};

            // Act
            DataLine dataLine = new DataLine(line, columnMappingsDictionary, configuration);
            int actualScenarioId = dataLine.ScenarioId;
            string actualVariableName = dataLine.VariableName;
            double[] actualPeriodValueArray = dataLine.PeriodValueArray;

            // Assert
            Assert.Equal(expectedScenarioId, actualScenarioId);
            Assert.Equal(expectedVariableName, actualVariableName);
            Assert.Equal(expectedPeriodValueArray.Length, actualPeriodValueArray.Length);

            for (int n = 0; n < expectedPeriodValueArray.Length; n++)
            {
                Assert.Equal(expectedPeriodValueArray[n], actualPeriodValueArray[n], Precision);
            }
        }

        [Fact]
        public void DataLineIsParsedCorrectly2Test()
        {
            // Arrange
            const string headerLine = "Value000	Value001	Value002	Value003	Value004	Value005	ScenId	VarName";
            DataHeader dataheader = new DataHeader(headerLine);
            Dictionary<string, int> columnMappingsDictionary = dataheader.ColumnMappings;
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            const string line = "0.00	0.04	0.04	0.04	0.04	0.03	1	AvePolLoanYield	";
            const int expectedScenarioId = 1;
            const string expectedVariableName = "AvePolLoanYield";
            double[] expectedPeriodValueArray = { 0.00, 0.04, 0.04, 0.04, 0.04, 0.03 };

            // Act
            DataLine dataLine = new DataLine(line, columnMappingsDictionary, configuration);
            int actualScenarioId = dataLine.ScenarioId;
            string actualVariableName = dataLine.VariableName;
            double[] actualPeriodValueArray = dataLine.PeriodValueArray;

            // Assert
            Assert.Equal(expectedScenarioId, actualScenarioId);
            Assert.Equal(expectedVariableName, actualVariableName);
            Assert.Equal(expectedPeriodValueArray.Length, actualPeriodValueArray.Length);

            for (int n = 0; n < expectedPeriodValueArray.Length; n++)
            {
                Assert.Equal(expectedPeriodValueArray[n], actualPeriodValueArray[n], Precision);
            }
        }

        [Fact]
        public void DataLinePeriodsAggregateCorrectlyTest()
        {
            // Arrange
            const string headerLine = "ScenId	VarName	Value000	Value001	Value002	Value003	Value004	Value005";
            DataHeader dataheader = new DataHeader(headerLine);
            Dictionary<string, int> columnMappingsDictionary = dataheader.ColumnMappings;
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);
            const string line = "1	AvePolLoanYield	0.00	0.04	0.04	0.04	0.04	0.03";
            DataLine dataLine = new DataLine(line, columnMappingsDictionary, configuration);
            const double expectedMinValueAggregation = 0.00;
            const double expectedMaxValueAggregation = 0.04;
            const double expectedFirstValueAggregation = 0.00;
            const double expectedLastValueAggregation = 0.03;

            // Act
            Dictionary<PeriodAggregation, double> periodAggregationDictionary = dataLine.AggregateAll();
            double actualMinValueAggregation = periodAggregationDictionary[PeriodAggregation.Min];
            double actualMaxValueAggregation = periodAggregationDictionary[PeriodAggregation.Max];
            double actualFirstValueAggregation = periodAggregationDictionary[PeriodAggregation.First];
            double actualLastValueAggregation = periodAggregationDictionary[PeriodAggregation.Last];

            // Assert
            Assert.Equal(expectedMinValueAggregation, actualMinValueAggregation, Precision);
            Assert.Equal(expectedMaxValueAggregation, actualMaxValueAggregation, Precision);
            Assert.Equal(expectedFirstValueAggregation, actualFirstValueAggregation, Precision);
            Assert.Equal(expectedLastValueAggregation, actualLastValueAggregation, Precision);
        }
    }
}
