
using System;
using System.Collections.Generic;

namespace StatisticsGenerator.Domain
{
    public class DataHeader
    {
        private readonly string _headerLine;

        public DataHeader(string headerLine)
        {
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                throw new Exception("Header line is missing in input data file");
            }

            _headerLine = headerLine;
            ColumnMappings = GetColumnMappings();
        }

        public Dictionary<string, int> ColumnMappings { get; }

        private Dictionary<string, int> GetColumnMappings()
        {
            string[] headerColumnArray = _headerLine.Split('\t');
            int numberHeaderColumns = headerColumnArray.Length;
            // The number of periods equals the numberHeaderColumns minus the two columns for ScenarioId and VariableName
            int numberPeriods = numberHeaderColumns - 2;
            List<string> expectedColumnHeaderNamesList = GetExpectedColumnHeaderNames(numberPeriods);
            Dictionary<string, int> columnMappingDictionary = new Dictionary<string, int>();

            foreach (string columnHeaderName in expectedColumnHeaderNamesList)
            {
                bool columnFound = false;

                for (int index = 0; index < headerColumnArray.Length; index++)
                {
                    if (headerColumnArray[index] == columnHeaderName)
                    {
                        columnMappingDictionary[columnHeaderName] = index;
                        columnFound = true;
                    }
                }

                if (!columnFound)
                {
                    throw new Exception($"{columnHeaderName} not found in input data file.");
                }
            }

            return columnMappingDictionary;
        }

        private static List<string> GetExpectedColumnHeaderNames(int numberPeriods)
        {
            const string scenarioIdColumnName = "ScenId";
            const string variableNameColumnName = "VarName";

            List<string> expectedColumnHeaderNamesList = new List<string> { scenarioIdColumnName, variableNameColumnName };

            for (int n = 0; n < numberPeriods; n++)
            {
                string columnName = $"Value{n.ToString().PadLeft(3, '0')}";
                expectedColumnHeaderNamesList.Add(columnName);
            }

            return expectedColumnHeaderNamesList;
        }
    }
}
