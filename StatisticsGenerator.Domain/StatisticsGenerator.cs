
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;

namespace StatisticsGenerator.Domain
{
    public class StatsGenerator
    {
        private List<Operation> _operationList;
        private Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>> _outerAggregationDictionary;

        public StatsGenerator()
        {
        }

        public StatsGenerator(string configurationFile)
        {
            ReadConfigurationFile(configurationFile);
        }

        public List<Operation> Operations => _operationList;

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
            File.WriteAllText(outputDataFile, statisticalResults);

            return statisticalResults;
        }

        #region Private Methods

        private static Operation ParseConfigurationLine(string line)
        {
            string[] arguments = line.Split('\t');

            Operation operation = new Operation {VariableName = arguments[0]};

            OuterAggregation outerAggregation;
            bool parseSucceeded = Enum.TryParse(arguments[1], out outerAggregation);
            if (!parseSucceeded)
            {
                throw new Exception("Invalid OuterAggregation in configuration file");
            }
            operation.OuterAggregation = outerAggregation;

            PeriodAggregation periodAggregation;
            parseSucceeded = Enum.TryParse(arguments[2], out periodAggregation);
            if (!parseSucceeded)
            {
                throw new Exception("Invalid PeriodAggregation in configuration file");
            }
            operation.PeriodAggregation = periodAggregation;

            return operation;
        }

        private void AggregatePeriodData(string inputDataFile)
        {
            _outerAggregationDictionary = new Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>>();

            using (FileStream fileStream = File.OpenRead(inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                int numberPeriods = ReadDataHeader(textReader);
                string line;

                while ((line = textReader.ReadLine()) != null)
                {
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
                    List<PeriodAggregation> periodAggregationList = GetPeriodAggregationsForVariable(variableName);

                    // Aggregate the period data (for a scenarioID and variabe name) into a dictionary
                    Dictionary<PeriodAggregation, double> periodAggregationDictionary = CreatePeriodAggregationsDictionary(periodAggregationList, periodValueArray);

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

        private static bool IsVariableProcessed(string variableName, IEnumerable<Operation> operationList)
        {
            return operationList.Any(operation => operation.VariableName == variableName);
        }

        private List<PeriodAggregation> GetPeriodAggregationsForVariable(string variableName)
        {
            List<PeriodAggregation> periodOperationList = new List<PeriodAggregation>();

            foreach (Operation operation in _operationList)
            {
                if (operation.VariableName == variableName)
                {
                    periodOperationList.Add(operation.PeriodAggregation);
                }
            }

            return periodOperationList;
        }

        private Dictionary<PeriodAggregation, double> CreatePeriodAggregationsDictionary(IEnumerable<PeriodAggregation> periodAggregationList, double[] periodValueArray)
        {
            Dictionary<PeriodAggregation, double> periodAggregationDictionary = new Dictionary<PeriodAggregation, double>();

            foreach (PeriodAggregation periodOperation in periodAggregationList)
            {
                double result = AggregatePeriods(periodValueArray, periodOperation);
                // Save the period aggregation into a dictionary with a key of PeriodAggregation
                periodAggregationDictionary[periodOperation] = result;
            }

            return periodAggregationDictionary;
        }

        private double AggregatePeriods(double[] periodValuesArray, PeriodAggregation periodAggregation)
        {
            double result;
            int numberPeriods = periodValuesArray.Length;

            switch (periodAggregation)
            {
                case PeriodAggregation.FirstValue:
                    result = periodValuesArray[0];
                    break;

                case PeriodAggregation.LastValue:
                    result = periodValuesArray[numberPeriods - 1];
                    break;

                case PeriodAggregation.MinValue:
                    result = periodValuesArray.Min();
                    break;

                case PeriodAggregation.MaxValue:
                    result = periodValuesArray.Max();
                    break;

                default:
                    throw new InvalidOperationException("Invalid period aggregation");
            }

            return result;
        }

        private string CreateStatisticalResults()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Iterate over operation in configuration file
            foreach (Operation operation in _operationList)
            {
                string variableName = operation.VariableName;
                OuterAggregation outerAggregation = operation.OuterAggregation;
                PeriodAggregation periodAggregation = operation.PeriodAggregation;

                // Create List to hold data to be aggregated
                List<double> aggregateList = new List<double>();

                // Iterate over outer aggregation dictionary
                foreach (var keyValuePair in _outerAggregationDictionary)
                {
                    ScenarioVariableKey key = keyValuePair.Key;
                    // key pair value contains the period aggregations
                    Dictionary<PeriodAggregation, double> value = keyValuePair.Value;

                    if (key.VariableName == variableName)
                    {
                        double periodAggregationResult = value[periodAggregation];
                        aggregateList.Add(periodAggregationResult);
                    }
                }

                double variableNameAggregate = AggregateVariableNames(aggregateList, outerAggregation);

                string message = $"({variableName.PadRight(20)},{outerAggregation.ToString().PadRight(15)},{periodAggregation.ToString().PadRight(15)}) = {variableNameAggregate.ToString(CultureInfo.InvariantCulture).PadLeft(20)}";
                stringBuilder.AppendLine(message);
            }

            string statisticalResults = stringBuilder.ToString();
            return statisticalResults;
        }

        private static double AggregateVariableNames(IEnumerable<double> aggregateList, OuterAggregation outerAggregation)
        {
            double result;

            switch (outerAggregation)
            {
                case OuterAggregation.MinValue:
                    result = aggregateList.Min();
                    break;

                case OuterAggregation.MaxValue:
                    result = aggregateList.Max();
                    break;

                case OuterAggregation.Average:
                    result = aggregateList.Average();
                    break;

                default:
                    throw new InvalidOperationException("Invalid outer aggregation");
            }

            return result;
        }

        #endregion
    }
}
