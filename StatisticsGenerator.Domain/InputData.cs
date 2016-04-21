
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace StatisticsGenerator.Domain
{
    public class InputData
    {
        private readonly string _inputDataFile;
        private readonly IConfiguration _configuration;
        private Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>> _outerAggregationDictionary;

        public InputData(string inputDataFile, IConfiguration configuration)
        {
            _inputDataFile = inputDataFile;
            _configuration = configuration;
            UseConcurrency = false;
        }

        public bool UseConcurrency { get; set; }

        public void PerformInnerAggregations()
        {
            _outerAggregationDictionary = new Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>>();

            using (FileStream fileStream = File.OpenRead(_inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                string headerLine = textReader.ReadLine();
                DataHeader dataHeader = new DataHeader(headerLine);
                string line;

                while ((line = textReader.ReadLine()) != null)
                {
                    DataLine dataLine = new DataLine(line, dataHeader.GetColumnMappings(), _configuration);
                    dataLine.ParseLine();

                    bool isVariableProcessed = _configuration.IsVariableProcessed(dataLine.VariableName);

                    // Skip data line if variable is not configured to be processed
                    if (!isVariableProcessed)
                    {
                        continue;
                    }

                    // Get the list of period aggregations for a variable name from the configuration 
                    List<PeriodAggregation> periodAggregationList = _configuration.GetPeriodAggregationsForVariable(dataLine.VariableName);

                    // Perform all aggregations on the dataLine and return dictionary of aggregations
                    Dictionary<PeriodAggregation, double> periodAggregationDictionary = dataLine.AggregateAll(periodAggregationList);

                    // Create composite key for outer aggregation dictionary
                    ScenarioVariableKey scenarioVariableKey = new ScenarioVariableKey
                    {
                        ScenarioId = dataLine.ScenarioId,
                        VariableName = dataLine.VariableName
                    };

                    // Save period aggregation dictionary into outer aggregation dictionary with composite key (scenarioID, variablename)
                    // todo expensive operation
                    _outerAggregationDictionary[scenarioVariableKey] = periodAggregationDictionary;
                }
            }
        }

        public string PerformOuterAggregations()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Operation operation in _configuration.Operations)
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
                    // key pair value contains the inner aggregations
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

        // todo use strategy design pattern
        private double AggregateVariableNames(IEnumerable<double> aggregateList, OuterAggregation outerAggregation)
        {
            double result;

            // todo add standard deviation
            switch (outerAggregation)
            {
                case OuterAggregation.MinValue:
                    result = UseConcurrency ? aggregateList.AsParallel().Min() : aggregateList.Min();
                    break;

                case OuterAggregation.MaxValue:
                    result = UseConcurrency ? aggregateList.AsParallel().Max() : aggregateList.Min();
                    break;

                case OuterAggregation.Average:
                    result = UseConcurrency ? aggregateList.AsParallel().Average() : aggregateList.Average();
                    break;

                default:
                    throw new InvalidOperationException("Invalid outer aggregation");
            }

            return result;
        }
    }
}
