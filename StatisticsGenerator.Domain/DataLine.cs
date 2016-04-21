
using System;
using System.Collections.Generic;

namespace StatisticsGenerator.Domain
{
    public class DataLine
    {
        private readonly string _line;
        private readonly Dictionary<string, int> _columnMappingDictionary;
        private readonly IConfiguration _configuration;

        public DataLine(string line, Dictionary<string, int> columnMappingDictionary, IConfiguration configuration)
        {
            _line = line;
            _columnMappingDictionary = columnMappingDictionary;
            _configuration = configuration;
        }

        public int ScenarioId { get; set; }
        public string VariableName { get; set; }
        public double[] PeriodValueArray { get; set; }
        public bool IsVariableProcessed { get; set; }
        public IAggregation AggregationStrategy { get; set; }

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

            // todo are these needed?
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

        public void AggregatePeriods()
        {
            List<PeriodAggregation> periodAggregationList = _configuration.GetPeriodAggregationsForVariable(VariableName);

            foreach (PeriodAggregation periodAggregation in periodAggregationList)
            {
                
            }
        }

        public double Aggregate()
        {
            return AggregationStrategy.AggregateNonIncrementally(PeriodValueArray);
        }

        public Dictionary<PeriodAggregation, double> AggregateAll(List<PeriodAggregation> periodAggregationList)
        {
            Dictionary<PeriodAggregation, double> periodAggregationDictionary = new Dictionary<PeriodAggregation, double>();

            foreach (PeriodAggregation periodAggregation in periodAggregationList)
            {
                double result = 0;

                switch (periodAggregation)
                {
                    case PeriodAggregation.MinValue:
                        AggregationStrategy = new MinAggregation();
                        result = Aggregate();
                        break;

                    case PeriodAggregation.MaxValue:
                        AggregationStrategy = new MaxAggregation();
                        result = Aggregate();
                        break;

                        //todo add missing cases
                }

                // todo save to dictionary
                periodAggregationDictionary[periodAggregation] = result;
            }

            return periodAggregationDictionary;
        }

        //private static double AggregatePeriods(double[] periodValuesArray, PeriodAggregation periodAggregation)
        //{
        //    double result;
        //    int numberPeriods = periodValuesArray.Length;

        //    switch (periodAggregation)
        //    {
        //        case PeriodAggregation.FirstValue:
        //            result = periodValuesArray[0];
        //            break;

        //        case PeriodAggregation.LastValue:
        //            result = periodValuesArray[numberPeriods - 1];
        //            break;

        //        case PeriodAggregation.MinValue:
        //            result = periodValuesArray.AsParallel().Min();
        //            break;

        //        case PeriodAggregation.MaxValue:
        //            result = periodValuesArray.AsParallel().Max();
        //            break;

        //        default:
        //            throw new InvalidOperationException("Invalid period aggregation");
        //    }

        //    return result;
        //}

    }
}
