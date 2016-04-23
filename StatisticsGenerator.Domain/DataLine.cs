
using System;
using System.Collections.Generic;
using StatisticsGenerator.Domain.Aggregations;

namespace StatisticsGenerator.Domain
{
    public class DataLine
    {
        private readonly IConfiguration _configuration;
        private IAggregation<double> AggregationStrategy { get; set; }

        public DataLine(string line, Dictionary<string, int> columnMappingDictionary, IConfiguration configuration)
        {
            ColumnMappings = columnMappingDictionary;
            _configuration = configuration;
            ParseLine(line);
        }

        public int ScenarioId { get; private set; }
        public string VariableName { get; private set; }
        public double[] PeriodValueArray { get; private set; }
        private Dictionary<string, int> ColumnMappings { get; }

        public Dictionary<PeriodAggregation, double> AggregateAll()
        {
            Dictionary<PeriodAggregation, double> periodAggregationDictionary = new Dictionary<PeriodAggregation, double>();
            List<PeriodAggregation> periodAggregationList = _configuration.GetPeriodAggregationsForVariable(VariableName);

            foreach (PeriodAggregation periodAggregation in periodAggregationList)
            {
                double result;

                switch (periodAggregation)
                {
                    case PeriodAggregation.First:
                        AggregationStrategy = new FirstAggregation();
                        result = Aggregate();
                        break;

                    case PeriodAggregation.Last:
                        AggregationStrategy = new LastAggregation();
                        result = Aggregate();
                        break;

                    case PeriodAggregation.Min:
                        AggregationStrategy = new MinAggregation();
                        result = Aggregate();
                        break;

                    case PeriodAggregation.Max:
                        AggregationStrategy = new MaxAggregation();
                        result = Aggregate();
                        break;

                    case PeriodAggregation.StdDev:
                        AggregationStrategy = new StandardDeviationAggregation();
                        result = Aggregate();
                        break;

                    default:
                        throw new Exception("Period aggregation case missing");
                }

                periodAggregationDictionary[periodAggregation] = result;
            }

            return periodAggregationDictionary;
        }

        #region Private Methods

        private void ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new Exception("Input data line was empty");
            }

            string[] segments = line.Split('\t');

            // Parse field value for ScenarioId
            string fieldValue = GetFieldValue(segments, "ScenId");
            int scenarioId;
            bool parseSucceeded = int.TryParse(fieldValue, out scenarioId);
            if (!parseSucceeded)
            {
                throw new Exception("Invalid ScenarioId in input data file");
            }

            // Parse field value for VariableName
            string variableName = GetFieldValue(segments, "VarName");

            // number periods equals total number columns minus senarioId and VariableName
            int numberPeriods = ColumnMappings.Count - 2;
            // Dynamically allocate array to hold period values
            double[] periodValueArray = new double[numberPeriods];

            // Parse field values for period values
            for (int n = 0; n < numberPeriods; n++)
            {
                string fieldName = $"Value{n.ToString().PadLeft(3, '0')}";
                fieldValue = GetFieldValue(segments, fieldName);
                double value;
                parseSucceeded = double.TryParse(fieldValue, out value);
                if (!parseSucceeded)
                {
                    throw new Exception("Invalid data value in data file");
                }
                periodValueArray[n] = value;
            }

            ScenarioId = scenarioId;
            VariableName = variableName;
            PeriodValueArray = periodValueArray;
        }

        private double Aggregate()
        {
            return AggregationStrategy.Aggregate(PeriodValueArray);
        }

        private string GetFieldValue(string[] segments, string fieldName)
        {
            int fieldIndex = ColumnMappings[fieldName];
            string fieldValue = segments[fieldIndex];
            return fieldValue;
        }

        #endregion
    }
}
