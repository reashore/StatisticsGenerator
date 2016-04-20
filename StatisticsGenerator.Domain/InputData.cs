
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StatisticsGenerator.Domain
{
    public class InputData
    {
        private string _inputDataFile;
        private readonly List<Operation> _operationList;
        private Dictionary<ScenarioVariableKey, Dictionary<PeriodAggregation, double>> _outerAggregationDictionary;

        public InputData()
        {

        }

        // todo implement
        public bool UseConcurrency { get; set; }

        public InputData(string inputDataFile)
        {
            _inputDataFile = inputDataFile;
        }




        // read the data file line by line
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

                    // todo the following should be attached to the configuration class *****

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

        //todo move to configuration file
        private static bool IsVariableProcessed(string variableName, IEnumerable<Operation> operationList)
        {
            return operationList.Any(operation => operation.VariableName == variableName);
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

    }
}
