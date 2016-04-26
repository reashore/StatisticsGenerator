
using System;
using Xunit;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.Tests.XUnit
{

    public class DataHeaderTests
    {
        //[Fact]
        //[ExpectedException(typeof(Exception))]
        //public void NullHeaderLineThrowsExceptionTest()
        //{
        //    // Arrange

        //    // Act
        //    // ReSharper disable once UnusedVariable
        //    DataHeader dataHeader = new DataHeader(null);

        //    // Assert
        //}

        //[Fact]
        //[ExpectedException(typeof(Exception))]
        //public void WhitespaceHeaderLineThrowsExceptionTest()
        //{
        //    // Arrange

        //    // Act
        //    // ReSharper disable once UnusedVariable
        //    DataHeader dataHeader = new DataHeader(" ");

        //    // Assert
        //}

        [Fact]
        public void HeaderLineHasCorrectNumberOfHeaders()
        {
            // Arrange
            const string headerLine = "ScenId	VarName	Value000	Value001	Value002	Value003	Value004	Value005";
            int expectedNumberHeaders = headerLine.Split('\t').Length;

            // Act
            DataHeader dataHeader = new DataHeader(headerLine);

            // Assert
            int actualNumberHeaders = dataHeader.ColumnMappings.Count;
            Assert.Equal(expectedNumberHeaders, actualNumberHeaders);
        }

        //[Fact]
        //[ExpectedException(typeof(Exception))]
        //public void ScenarioIdHeaderColumnIsMissingThrowsExceptionTest()
        //{
        //    // Arrange
        //    const string headerLine = "VarName	Value000	Value001	Value002	Value003	Value004	Value005";

        //    // Act
        //    // ReSharper disable once UnusedVariable
        //    DataHeader dataHeader = new DataHeader(headerLine);

        //    // Assert
        //}

        //[Fact]
        //[ExpectedException(typeof(Exception))]
        //public void VariableNameHeaderColumnIsMissingThrowsExceptionTest()
        //{
        //    // Arrange
        //    const string headerLine = "ScenId	Value000	Value001	Value002	Value003	Value004	Value005";

        //    // Act
        //    // ReSharper disable once UnusedVariable
        //    DataHeader dataHeader = new DataHeader(headerLine);

        //    // Assert
        //}

        //[Fact]
        //[ExpectedException(typeof(Exception))]
        //public void ValueHeaderColumnIsMissingThrowsExceptionTest()
        //{
        //    // Arrange
        //    const string headerLine = "ScenId	VarName	Value000	Value002	Value003	Value004	Value005";

        //    // Act
        //    // ReSharper disable once UnusedVariable
        //    DataHeader dataHeader = new DataHeader(headerLine);

        //    // Assert
        //}

        [Fact]
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
            Assert.Equal(expectedValue001ColumnIndex, actualValue001ColumnIndex);
            Assert.Equal(expectedScenarioIdColumnIndex, actualScenarioIdColumnIndex);
            Assert.Equal(expectedVariableNameColumnIndex, actualVariableNameColumnIndex);
        }
    }
}
