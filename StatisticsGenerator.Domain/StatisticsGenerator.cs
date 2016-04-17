
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;

namespace StatisticsGenerator.Domain
{
    public enum AggregateOperation
    {
        MinValue,
        MaxValue,
        Average
    }

    public enum PeriodOperation
    {
        FirstValue,
        LastValue,
        MinValue,
        MaxValue
    }

    public class StatsGenerator
    {
        private List<Operation> _operationList;
        private Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>> _outerAggregationDictionary;

        public StatsGenerator()
        {
        }

        public StatsGenerator(string configurationFile)
        {
            ReadConfigurationFile(configurationFile);
        }

        public void ReadConfigurationFile(string configurationFile)
        {
            string[] lines = File.ReadAllLines(configurationFile);
            _operationList = new List<Operation>();

            foreach (string line in lines)
            {
                // Skip blank lines in configuration file
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                Operation operation = ParseConfigurationLine(line);
                _operationList.Add(operation);
            }
        }

        public string GenerateStatistics(string inputDataFile, string outputDataFile)
        {
            AggregatePeriodData(inputDataFile);
            string statisticalResults = CreateStatisticalResults();
            CreateOutputDataFile(outputDataFile, statisticalResults);

            return statisticalResults;
        }

        #region Private Methods

        private static Operation ParseConfigurationLine(string line)
        {
            string[] arguments = line.Split('\t');
            Operation operation = new Operation();

            operation.VariableName = arguments[0];

            AggregateOperation aggregateOperation;
            bool parseSucceeded = Enum.TryParse(arguments[1], out aggregateOperation);
            if (!parseSucceeded)
            {
                throw new Exception("Invalid AggregateOperation in configuration file");
            }
            operation.AggregateOperation = aggregateOperation;

            PeriodOperation periodOperation;
            parseSucceeded = Enum.TryParse(arguments[2], out periodOperation);
            if (!parseSucceeded)
            {
                throw new Exception("Invalid PeriodOperation in configuration file");
            }
            operation.PeriodOperation = periodOperation;

            return operation;
        }

        private void AggregatePeriodData(string inputDataFile)
        {
            _outerAggregationDictionary = new Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>>();

            using (FileStream fileStream = File.OpenRead(inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                int numberPeriods = ReadDataHeader(textReader);

                while (true)
                {
                    string line = textReader.ReadLine();

                    // Check for end of file
                    if (line == null)
                    {
                        break;
                    }

                    int scenarioId;
                    string variableName;
                    double[] periodValueArray;

                    ParseDataLine(line, numberPeriods, out scenarioId, out variableName, out periodValueArray);
                    bool isVariableProcessed = IsVariableProcessed(variableName, _operationList);

                    // Skip data line if variable is not configured to be processed
                    if (!isVariableProcessed)
                    {
                        continue;
                    }

                    // Get the list of period aggregation operations for a variable name from the configuration data (i.e. operationsList)
                    List<PeriodOperation> periodAggregationList = GetPeriodOperationsForVariable(variableName);

                    // Aggregate the period data (for a scenarioID and variabe name) into a dictionary
                    Dictionary<PeriodOperation, double> periodAggregationDictionary = CreatePeriodAggregationsDictionary(periodAggregationList, periodValueArray);

                    // Create composite key for outer aggregation dictionary
                    ScenarioVariableKey scenarioVariableKey = new ScenarioVariableKey
                    {
                        ScenarioId = scenarioId,
                        VariableName = variableName
                    };

                    // Save period aggregation dictionary into outer aggregation dictionary with composite key (scenarioID, variablename)
                    _outerAggregationDictionary[scenarioVariableKey] = periodAggregationDictionary;
                }
            }
        }

        private static int ReadDataHeader(TextReader textReader)
        {
            string firstLine = textReader.ReadLine();

            if (firstLine == null)
            {
                throw new Exception("Input data file contains empty first row");
            }

            string[] columnHeaderArray = firstLine.Split('\t');

            // The number of periods is the length of the column header array minus the first two column headers (ScenarioId and VariableName)
            int numberPeriods = columnHeaderArray.Length - 2;

            return numberPeriods;
        }

        private static void ParseDataLine(string line, int numberPeriods, out int scenarioId, out string variableName, out double[] periodValueArray)
        {
            string[] segments = line.Split('\t');

            bool parseSucceeded = int.TryParse(segments[0], out scenarioId);

            if (!parseSucceeded)
            {
                throw new Exception("Invalid ScenarioId in data file");
            }

            variableName = segments[1];

            // Dynamically allocate array to hold number of period values
            periodValueArray = new double[numberPeriods];

            for (int n = 2; n < numberPeriods; n++)
            {
                double value;
                parseSucceeded = double.TryParse(segments[n], out value);
                if (!parseSucceeded)
                {
                    throw new Exception("Invalid data value in data file");
                }
                periodValueArray[n] = value;
            }
        }

        private static bool IsVariableProcessed(string variableName, List<Operation> operationList)
        {
            return operationList.Any(operation => operation.VariableName == variableName);
        }

        private List<PeriodOperation> GetPeriodOperationsForVariable(string variableName)
        {
            List<PeriodOperation> periodOperationList = new List<PeriodOperation>();

            foreach (Operation operation in _operationList)
            {
                if (operation.VariableName == variableName)
                {
                    periodOperationList.Add(operation.PeriodOperation);
                }
            }

            return periodOperationList;
        }

        private Dictionary<PeriodOperation, double> CreatePeriodAggregationsDictionary(List<PeriodOperation> periodAggregationList, double[] periodValueArray)
        {
            Dictionary<PeriodOperation, double> periodAggregationDictionary = new Dictionary<PeriodOperation, double>();

            foreach (PeriodOperation periodOperation in periodAggregationList)
            {
                double result = AggregatePeriods(periodValueArray, periodOperation);
                // Save the period aggregation into a dictionary with a key of periodOperation
                periodAggregationDictionary[periodOperation] = result;
            }

            return periodAggregationDictionary;
        }

        private double AggregatePeriods(double[] periodValuesArray, PeriodOperation periodOperation)
        {
            double result;
            int numberPeriods = periodValuesArray.Length;

            switch (periodOperation)
            {
                case PeriodOperation.FirstValue:
                    result = periodValuesArray[0];
                    break;

                case PeriodOperation.LastValue:
                    result = periodValuesArray[numberPeriods - 1];
                    break;

                case PeriodOperation.MinValue:
                    result = periodValuesArray.Min();
                    break;

                case PeriodOperation.MaxValue:
                    result = periodValuesArray.Max();
                    break;

                default:
                    throw new InvalidOperationException("Invalid period operation");
            }

            return result;
        }

        private string CreateStatisticalResults()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Operation operation in _operationList)
            {
                string variableName = operation.VariableName;
                AggregateOperation aggregateOperation = operation.AggregateOperation;
                PeriodOperation periodOperation = operation.PeriodOperation;
                List<double> aggregateList = new List<double>();

                foreach (KeyValuePair<ScenarioVariableKey, Dictionary<PeriodOperation, double>> keyValuePair in _outerAggregationDictionary)
                {
                    ScenarioVariableKey key = keyValuePair.Key;
                    Dictionary<PeriodOperation, double> value = keyValuePair.Value;

                    if (key.VariableName == variableName)
                    {
                        double periodAggregationResult = value[periodOperation];
                        aggregateList.Add(periodAggregationResult);
                    }
                }

                double variableNameAggregate = AggregateVariableNames(aggregateList, aggregateOperation);

                string message = $"({variableName.PadRight(20)},{aggregateOperation.ToString().PadRight(15)},{periodOperation.ToString().PadRight(15)}) ={variableNameAggregate.ToString(CultureInfo.InvariantCulture).PadLeft(20)}";
                stringBuilder.AppendLine(message);
            }

            string statisticalResults = stringBuilder.ToString();
            return statisticalResults;
        }

        private double AggregateVariableNames(List<double> aggregateList, AggregateOperation aggregateOperation)
        {
            double result;

            switch (aggregateOperation)
            {
                case AggregateOperation.MinValue:
                    result = aggregateList.Min();
                    break;

                case AggregateOperation.MaxValue:
                    result = aggregateList.Max();
                    break;

                case AggregateOperation.Average:
                    result = aggregateList.Average();
                    break;

                default:
                    throw new InvalidOperationException("Invalid aggregate operation");
            }

            return result;
        }

        private void CreateOutputDataFile(string outputDataFile, string fileContents)
        {
            File.Delete(outputDataFile);
            File.AppendAllText(outputDataFile, fileContents);

        }

        #endregion
    }
}
