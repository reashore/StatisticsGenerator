
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using StatisticsGenerator.Domain.Aggregations;

namespace StatisticsGenerator.Domain
{
    public class InputData
    {
        private readonly string _inputDataFile;
        private readonly IConfiguration _configuration;
        private Dictionary<ScenarioVariableNameKey, Dictionary<PeriodAggregation, double>> _outerAggregationDictionary;

        public InputData(string inputDataFile, IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(inputDataFile))
            {
                throw new Exception("Input data file is null or white space");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _inputDataFile = inputDataFile;
            _configuration = configuration;
        }

        public string CreateStatistics()
        {
            PerformInnerAggregations();
            StringBuilder stringBuilder = new StringBuilder();

            // Note that the current order for the inner and outer loops minimizes memory requirements.
            // The outerAggregationDictionary is iterated once for each operation. This requires a single List<double>
            // to hold the data currently being aggregated.

            foreach (Operation operation in _configuration.Operations)
            {
                string variableName = operation.VariableName;
                OuterAggregation outerAggregation = operation.OuterAggregation;
                PeriodAggregation periodAggregation = operation.PeriodAggregation;

                // Create List to hold data to be aggregated
                List<double> aggregationList = new List<double>();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var keyValuePair in _outerAggregationDictionary)
                {
                    ScenarioVariableNameKey key = keyValuePair.Key;
                    // key pair value contains the inner aggregations
                    Dictionary<PeriodAggregation, double> value = keyValuePair.Value;

                    if (key.VariableName == variableName)
                    {
                        double periodAggregationResult = value[periodAggregation];
                        aggregationList.Add(periodAggregationResult);
                    }
                }

                double outerAggregationValue = PerformOuterAggregation(aggregationList, outerAggregation);

                string keyFormat = $"({variableName.PadRight(17)},{outerAggregation.ToString().PadRight(10)},{periodAggregation.ToString().PadRight(6)})";
                string valueFormat = $"{outerAggregationValue.ToString("F2", CultureInfo.InvariantCulture).PadLeft(14)}";
                string message = $"{keyFormat} = {valueFormat}";
                stringBuilder.AppendLine(message);
            }

            string statisticalResults = stringBuilder.ToString();
            return statisticalResults;
        }

        #region Private Members

        private IAggregation<double> AggregationStrategy { get; set; }

        private void PerformInnerAggregations()
        {
            _outerAggregationDictionary = new Dictionary<ScenarioVariableNameKey, Dictionary<PeriodAggregation, double>>();

            // see CA2202: https://msdn.microsoft.com/en-us/library/ms182334.aspx

            using (FileStream fileStream = File.OpenRead(_inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                string line = textReader.ReadLine();
                DataHeader dataHeader = new DataHeader(line);

                while ((line = textReader.ReadLine()) != null)
                {
                    DataLine dataLine = new DataLine(line, dataHeader.ColumnMappings, _configuration);

                    bool isVariableProcessed = _configuration.IsVariableProcessed(dataLine.VariableName);

                    // Skip data line if variable is not configured to be processed
                    if (!isVariableProcessed)
                    {
                        continue;
                    }

                    // Perform all configured aggregations on the dataLine and return dictionary of aggregations
                    Dictionary<PeriodAggregation, double> periodAggregationDictionary = dataLine.AggregateAll();

                    // Create composite key for outer aggregation dictionary
                    ScenarioVariableNameKey scenarioVariableKey = new ScenarioVariableNameKey
                    {
                        ScenarioId = dataLine.ScenarioId,
                        VariableName = dataLine.VariableName
                    };

                    // Save period aggregation dictionary into outer aggregation dictionary with composite key (scenarioID, variablename)
                    _outerAggregationDictionary[scenarioVariableNameKey] = periodAggregationDictionary;
                }
            }
        }

        private double Aggregate(IEnumerable<double> aggregateList)
        {
            return AggregationStrategy.Aggregate(aggregateList);
        }

        private double PerformOuterAggregation(IEnumerable<double> aggregateList, OuterAggregation outerAggregation)
        {
            double result;

            switch (outerAggregation)
            {
                case OuterAggregation.Min:
                    AggregationStrategy = new MinAggregation();
                    result = Aggregate(aggregateList);
                    break;

                case OuterAggregation.Max:
                    AggregationStrategy = new MaxAggregation();
                    result = Aggregate(aggregateList);
                    break;

                case OuterAggregation.Average:
                    AggregationStrategy = new AverageAggregation();
                    result = Aggregate(aggregateList);
                    break;

                case OuterAggregation.StdDev:
                    AggregationStrategy = new StandardDeviationAggregation();
                    result = Aggregate(aggregateList);
                    break;

                default:
                    throw new Exception("Outer aggregation case missing");
            }

            return result;
        }

        #endregion
    }
}
