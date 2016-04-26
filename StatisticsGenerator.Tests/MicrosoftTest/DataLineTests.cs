
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    [TestClass]
    public class DataLineTests
    {
        private const double Delta = .001;

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void NullDataLineThrowsExceptionTest()
        {
            // Arrange
            // An empty column mapping dictionary is sufficient for this test
            Dictionary<string, int> columnMappingDictionary = new Dictionary<string, int>();
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);

            // Act
            // ReSharper disable once UnusedVariable
            DataLine dataLine = new DataLine(null, columnMappingDictionary, configuration);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void WhitespaceDataLineThrowsExceptionTest()
        {
            // Arrange
            // An empty column mapping dictionary is sufficient for this test
            Dictionary<string, int> columnMappingDictionary = new Dictionary<string, int>();
            const string configurationFile = @"..\..\..\Data\Configuration.txt";
            IConfiguration configuration = new Configuration(configurationFile);

            // Act
            // ReSharper disable once UnusedVariable
            DataLine dataLine = new DataLine("  ", columnMappingDictionary, configuration);

            // Assert
        }

        [TestMethod]
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
            Assert.AreEqual(expectedScenarioId, actualScenarioId);
            Assert.AreEqual(expectedVariableName, actualVariableName);
            Assert.AreEqual(expectedPeriodValueArray.Length, actualPeriodValueArray.Length);

            for (int n = 0; n < expectedPeriodValueArray.Length; n++)
            {
                Assert.AreEqual(expectedPeriodValueArray[n], actualPeriodValueArray[n], Delta);
            }
        }

        [TestMethod]
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
            double[] expectedPeriodValueArray = {0.00, 0.04, 0.04, 0.04, 0.04, 0.03};

            // Act
            DataLine dataLine = new DataLine(line, columnMappingsDictionary, configuration);
            int actualScenarioId = dataLine.ScenarioId;
            string actualVariableName = dataLine.VariableName;
            double[] actualPeriodValueArray = dataLine.PeriodValueArray;

            // Assert
            Assert.AreEqual(expectedScenarioId, actualScenarioId);
            Assert.AreEqual(expectedVariableName, actualVariableName);
            Assert.AreEqual(expectedPeriodValueArray.Length, actualPeriodValueArray.Length);

            for (int n = 0; n < expectedPeriodValueArray.Length; n++)
            {
                Assert.AreEqual(expectedPeriodValueArray[n], actualPeriodValueArray[n], Delta);
            }
        }

        [TestMethod]
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
            Assert.AreEqual(expectedMinValueAggregation, actualMinValueAggregation, Delta);
            Assert.AreEqual(expectedMaxValueAggregation, actualMaxValueAggregation, Delta);
            Assert.AreEqual(expectedFirstValueAggregation, actualFirstValueAggregation, Delta);
            Assert.AreEqual(expectedLastValueAggregation, actualLastValueAggregation, Delta);
        }
    }
}
