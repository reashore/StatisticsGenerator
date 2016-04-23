
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.MicrosoftTest
{

    [TestClass]
    public class DataHeaderTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void NullHeaderLineThrowsExceptionTest()
        {
            // Arrange

            // Act
            // ReSharper disable once UnusedVariable
            DataHeader dataHeader = new DataHeader(null);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void WhitespaceHeaderLineThrowsExceptionTest()
        {
            // Arrange

            // Act
            // ReSharper disable once UnusedVariable
            DataHeader dataHeader = new DataHeader(" ");

            // Assert
        }

        [TestMethod]
        public void HeaderLineHasCorrectNumberOfHeaders()
        {
            // Arrange
            const string headerLine = "ScenId	VarName	Value000	Value001	Value002	Value003	Value004	Value005";
            int expectedNumberHeaders = headerLine.Split('\t').Length;

            // Act
            DataHeader dataHeader = new DataHeader(headerLine);

            // Assert
            int actualNumberHeaders = dataHeader.ColumnMappings.Count;
            Assert.AreEqual(expectedNumberHeaders, actualNumberHeaders);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ScenarioIdHeaderColumnIsMissingThrowsExceptionTest()
        {
            // Arrange
            const string headerLine = "VarName	Value000	Value001	Value002	Value003	Value004	Value005";

            // Act
            // ReSharper disable once UnusedVariable
            DataHeader dataHeader = new DataHeader(headerLine);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void VariableNameHeaderColumnIsMissingThrowsExceptionTest()
        {
            // Arrange
            const string headerLine = "ScenId	Value000	Value001	Value002	Value003	Value004	Value005";

            // Act
            // ReSharper disable once UnusedVariable
            DataHeader dataHeader = new DataHeader(headerLine);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ValueHeaderColumnIsMissingThrowsExceptionTest()
        {
            // Arrange
            const string headerLine = "ScenId	VarName	Value000	Value002	Value003	Value004	Value005";

            // Act
            // ReSharper disable once UnusedVariable
            DataHeader dataHeader = new DataHeader(headerLine);

            // Assert
        }

        [TestMethod]
        public void HeaderColumnsParseCorrectlyInDifferentOrderTest()
        {
            // Although many variations of the header column order are possible, for this demo only one will be tested

            // Arrange
            const string headerLine = "Value000	Value001	Value002	Value003	Value004	Value005	ScenId	VarName";
            // Indexes are zero-based
            int expectedValue001ColumnIndex = 1;
            int expectedScenarioIdColumnIndex = 6;     
            int expectedVariableNameColumnIndex = 7;     

            // Act
            DataHeader dataHeader = new DataHeader(headerLine);
            int actualScenarioIdColumnIndex = dataHeader.ColumnMappings["ScenId"];
            int actualVariableNameColumnIndex = dataHeader.ColumnMappings["VarName"];
            int actualValue001ColumnIndex = dataHeader.ColumnMappings["Value001"];

            // Assert
            Assert.AreEqual(expectedValue001ColumnIndex, actualValue001ColumnIndex);
            Assert.AreEqual(expectedScenarioIdColumnIndex, actualScenarioIdColumnIndex);
            Assert.AreEqual(expectedVariableNameColumnIndex, actualVariableNameColumnIndex);
        }
    }
}
