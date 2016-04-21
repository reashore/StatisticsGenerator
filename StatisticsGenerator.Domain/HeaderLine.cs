
using System;
using System.Collections.Generic;

namespace StatisticsGenerator.Domain
{
    public class DataHeader
    {
        private readonly string _headerLine;

        public DataHeader(string headerLine)
        {
            _headerLine = headerLine;
        }

        public Dictionary<string, int> GetColumnMappings()
        {
            if (_headerLine == null)
            {
                // todo log exception using NLog
                throw new Exception(Resources.Error_InputDataFileContainsEmptyFirstRow);
            }

            string[] columnHeaderArray = _headerLine.Split('\t');
            int numberHeaderColumns = columnHeaderArray.Length;
            // The number of periods equals the numberHeaderColumns minus the two columns for ScenarioId and VariableName
            int numberPeriods = numberHeaderColumns - 2;
            List<string> expectedColumnHeaderNamesList = GetExpectedColumnHeaderNames(numberPeriods);
            Dictionary<string, int> columnMappingDictionary = new Dictionary<string, int>();

            foreach (string columnHeaderName in expectedColumnHeaderNamesList)
            {
                bool columnFound = false;

                for (int index = 0; index < columnHeaderArray.Length; index++)
                {
                    if (columnHeaderArray[index] == columnHeaderName)
                    {
                        columnMappingDictionary[columnHeaderName] = index;
                        columnFound = true;
                        break;
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
