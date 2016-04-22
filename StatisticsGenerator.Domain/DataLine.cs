
using System;
using System.Collections.Generic;
using StatisticsGenerator.Domain.Aggregations;

namespace StatisticsGenerator.Domain
{
    public class DataLine
    {
        private readonly string _line;
        private readonly Dictionary<string, int> _columnMappingDictionary;
        //private readonly IConfiguration _configuration;

        public DataLine(string line, Dictionary<string, int> columnMappingDictionary)
        {
            _line = line;
            _columnMappingDictionary = columnMappingDictionary;
            //_configuration = configuration;
            UseConcurrency = false;
        }

        //public DataLine(string line, Dictionary<string, int> columnMappingDictionary, IConfiguration configuration)
        //{
        //    _line = line;
        //    _columnMappingDictionary = columnMappingDictionary;
        //    _configuration = configuration;
        //    UseConcurrency = false;
        //}

        public int ScenarioId { get; private set; }
        public string VariableName { get; private set; }
        public double[] PeriodValueArray { get; private set; }
        public bool IsVariableProcessed { get; set; }
        public IAggregation<double> AggregationStrategy { get; set; }
        public bool UseConcurrency { get; set; }

        public void ParseLine()
        {
            if (string.IsNullOrWhiteSpace(_line))
            {
                throw new Exception("Input data line was empty");
            }

            string[] segments = _line.Split('\t');

            // Parse field value for ScenarioId
            string fieldValue = GetFieldValue(segments, "ScenId");
            int scenarioId;
            bool parseSucceeded = int.TryParse(fieldValue, out scenarioId);
            if (!parseSucceeded)
            {
                // todo log error
                throw new Exception("Invalid ScenarioId in input data file");
            }

            // Parse field value for VariableName
            string variableName = GetFieldValue(segments, "VarName");

            // number periods equals total number columns minus senarioId and VariableName
            int numberPeriods = _columnMappingDictionary.Count - 2;
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

        private string GetFieldValue(string[] segments, string fieldName)
        {
            int fieldIndex = _columnMappingDictionary[fieldName];
            string fieldValue = segments[fieldIndex];
            return fieldValue;
        }

        public double Aggregate()
        {
            return AggregationStrategy.Aggregate(PeriodValueArray);
        }

        public Dictionary<PeriodAggregation, double> AggregateAll(List<PeriodAggregation> periodAggregationList)
        {
            Dictionary<PeriodAggregation, double> periodAggregationDictionary = new Dictionary<PeriodAggregation, double>();

            foreach (PeriodAggregation periodAggregation in periodAggregationList)
            {
                double result = 0;

                switch (periodAggregation)
                {
                    case PeriodAggregation.FirstValue:
                        AggregationStrategy = new FirstAggregation(UseConcurrency);
                        result = Aggregate();
                        break;

                    case PeriodAggregation.LastValue:
                        AggregationStrategy = new LastAggregation(UseConcurrency);
                        result = Aggregate();
                        break;

                    case PeriodAggregation.MinValue:
                        AggregationStrategy = new MinAggregation(UseConcurrency);
                        result = Aggregate();
                        break;

                    case PeriodAggregation.MaxValue:
                        AggregationStrategy = new MaxAggregation(UseConcurrency);
                        result = Aggregate();
                        break;

                    case PeriodAggregation.StandardDeviation:
                        AggregationStrategy = new StandardDeviationAggregation(UseConcurrency);
                        result = Aggregate();
                        break;
                }

                periodAggregationDictionary[periodAggregation] = result;
            }

            return periodAggregationDictionary;
        }
    }
}
