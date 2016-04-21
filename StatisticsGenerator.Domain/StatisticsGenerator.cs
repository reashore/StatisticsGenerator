
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
        private readonly List<Operation> _operationList;
        private Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>> _outerAggregationDictionary;
        // todo expose?
        private readonly Configuration _configuration;

        public StatsGenerator(string configurationFile)
        {
            _configuration = new Configuration(configurationFile);
            // todo remove this line and just use configuration
            _operationList = _configuration.Operations;
        }

        public string GenerateStatistics(string inputDataFile, string outputDataFile)
        {
            AggregatePeriodData(inputDataFile);
            string statisticalResults = CreateStatisticalResults();
            File.WriteAllText(outputDataFile, statisticalResults);

            return statisticalResults;
        }

        private static int ReadDataHeader(TextReader textReader)
        {
            string firstLine = textReader.ReadLine();

            if (firstLine == null)
            {
                // todo log exception
                throw new Exception(Resources.Error_InputDataFileContainsEmptyFirstRow);
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
                // todo log error
                throw new Exception("Invalid ScenarioId in data file");
            }

            variableName = segments[1];

            // Dynamically allocate array to hold period values
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

        private void AggregatePeriodData(string inputDataFile)
        {
            _outerAggregationDictionary = new Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>>();

            using (FileStream fileStream = File.OpenRead(inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                int numberPeriods = ReadDataHeader(textReader);
                //int numberPeriods = 0;
                string line;

                //// new code
                //// todo caution reading header twice!!!!!
                //string headerLine = textReader.ReadLine();
                //DataHeader dataHeader = new DataHeader(headerLine);
                //Dictionary<string, int> columnMappingDictionary = dataHeader.GetColumnMappings();

                while ((line = textReader.ReadLine()) != null)
                {
                    // new code *********************************
                    //DataLine dataLine = new DataLine(line, columnMappingDictionary, _configuration);
                    //dataLine.ParseLine();
                    ////int temp1 = dataLine.ScenarioId;
                    ////string temp2 = dataLine.VariableName;
                    ////double[] periodValueArrayTemp = dataLine.PeriodValueArray;
                    //dataLine.AggregationStrategy = new MinAggregation();
                    //dataLine.Aggregate();

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

                    // Get the list of period aggregation operations for a variable name from the configuration data (i.e. _operationList)
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

        private double AggregatePeriods(double[] periodValuesArray, PeriodAggregation periodAggregation)
        {
            double result;
            int numberPeriods = periodValuesArray.Length;

            // todo add standard deviation
            switch (periodAggregation)
            {
                case PeriodAggregation.FirstValue:
                    result = periodValuesArray[0];
                    break;

                case PeriodAggregation.LastValue:
                    result = periodValuesArray[numberPeriods - 1];
                    break;

                case PeriodAggregation.MinValue:
                    result = periodValuesArray.AsParallel().Min();
                    break;

                case PeriodAggregation.MaxValue:
                    result = periodValuesArray.AsParallel().Max();
                    break;

                default:
                    throw new InvalidOperationException("Invalid period aggregation");
            }

            return result;
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

                string message = $"({variableName.PadRight(17)},{outerAggregation.ToString().PadRight(10)},{periodAggregation.ToString().PadRight(11)}) = {variableNameAggregate.ToString("F2", CultureInfo.InvariantCulture).PadLeft(18)}";
                stringBuilder.AppendLine(message);
            }

            string statisticalResults = stringBuilder.ToString();
            return statisticalResults;
        }

        private static double AggregateVariableNames(IEnumerable<double> aggregateList, OuterAggregation outerAggregation)
        {
            double result;

            // todo add standard deviation
            switch (outerAggregation)
            {
                case OuterAggregation.MinValue:
                    result = aggregateList.AsParallel().Min();
                    break;

                case OuterAggregation.MaxValue:
                    result = aggregateList.AsParallel().Max();
                    break;

                case OuterAggregation.Average:
                    result = aggregateList.AsParallel().Average();
                    break;

                default:
                    throw new InvalidOperationException("Invalid outer aggregation");
            }

            return result;
        }
    }

    #region Saved Code

    public class StatsGenerator2
    {
        private readonly List<Operation> _operationList;
        private Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>> _outerAggregationDictionary;

        public StatsGenerator2()
        {
        }

        public StatsGenerator2(string configurationFile)
        {
            Configuration configuration = new Configuration(configurationFile);
            _operationList = configuration.Operations;
        }

        public string GenerateStatistics(string inputDataFile, string outputDataFile)
        {
            AggregatePeriodData(inputDataFile);
            string statisticalResults = CreateStatisticalResults();
            File.WriteAllText(outputDataFile, statisticalResults);

            return statisticalResults;
        }

        // Private Methods

        // Data Methods

        private static int ReadDataHeader(TextReader textReader)
        {
            string firstLine = textReader.ReadLine();

            if (firstLine == null)
            {
                // todo replace with resource
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

            // Dynamically allocate array to hold period values
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

                    // Get the list of period aggregation operations for a variable name from the configuration data (i.e. _operationList)
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
                    result = periodValuesArray.AsParallel().Min();
                    break;

                case PeriodAggregation.MaxValue:
                    result = periodValuesArray.AsParallel().Max();
                    break;

                default:
                    throw new InvalidOperationException("Invalid period aggregation");
            }

            return result;
        }

        // Other Methods

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

                string message = $"({variableName.PadRight(17)},{outerAggregation.ToString().PadRight(10)},{periodAggregation.ToString().PadRight(11)}) = {variableNameAggregate.ToString("F2", CultureInfo.InvariantCulture).PadLeft(18)}";
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
                    result = aggregateList.AsParallel().Min();
                    break;

                case OuterAggregation.MaxValue:
                    result = aggregateList.AsParallel().Max();
                    break;

                case OuterAggregation.Average:
                    result = aggregateList.AsParallel().Average();
                    break;

                default:
                    throw new InvalidOperationException("Invalid outer aggregation");
            }

            return result;
        }
    }

    #endregion
}
