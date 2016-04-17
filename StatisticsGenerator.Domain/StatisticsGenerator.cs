
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace StatisticsGenerator.Domain
{
    public enum AggregateOperation
    {
        MinValue,
        MaxValue,
        Average
    }

    // todo rename to PeriodAggregation
    public enum PeriodOperation
    {
        FirstValue,
        LastValue,
        MinValue,
        MaxValue
    }

    public class Operation
    {
        public string VariableName { get; set; }
        public AggregateOperation AggregateOperation { get; set; }
        public PeriodOperation PeriodOperation { get; set; }
    }

    // use struct rather than class since it makes it easier to compare dictionary keys
    public struct ScenarioVariableKey
    {
        public int ScenarioId { get; set; }
        public string VariableName { get; set; }
    }

    ///////////////////////////////

    public class StatsGenerator
    {
        private List<Operation> _operationList;
        private Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>> _masterDictionary;

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
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

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

                _operationList.Add(operation);
            }

            //return operationList;
        }

        public void ProcessData(string inputDataFile, string outputDataFile)
        {
            _masterDictionary = new Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>>();

            using (FileStream fileStream = File.OpenRead(inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                int numberPeriods = ReadDataHeader(textReader);
                while (true)
                {
                    string line = textReader.ReadLine();

                    // check for end of file
                    if (line == null)
                    {
                        break;
                    }

                    int scenarioId;
                    string variableName;
                    double[] periodValueArray;

                    ParseDataLine(line, numberPeriods, out scenarioId, out variableName, out periodValueArray);

                    bool isVariableProcessed = IsVariableProcessed(variableName, _operationList);

                    // skip if variable is not configured to be processed
                    if (!isVariableProcessed)
                    {
                        continue;
                    }

                    // create key
                    ScenarioVariableKey scenarioVariableKey = new ScenarioVariableKey
                    {
                        ScenarioId = scenarioId,
                        VariableName = variableName
                    };

                    // variableName + operationsList => periodAggregations
                    // periodAggregations + periodValueArray => period aggregation dictionary

                    List<PeriodOperation> periodAggregationList = GetPeriodOperationsForVariable(variableName);
                    Dictionary<PeriodOperation, double> periodAggregationDictionary = CreatePeriodAggregationsDictionary(periodAggregationList, periodValueArray);

                    _masterDictionary[scenarioVariableKey] = periodAggregationDictionary;
                }
            }

            CreateProcessedDataFile(outputDataFile);
        }

        private static int ReadDataHeader(TextReader textReader)
        {
            string firstLine = textReader.ReadLine();

            if (firstLine == null)
            {
                throw new Exception("Input data file contains empty first row");
            }

            string[] columnHeaderArray = firstLine.Split('\t');
            // the number of periods is the length of the column header array 
            // minus the first two column headers (ScenarioId and VariableName)
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

        private void CreateProcessedDataFile(string outputDataFile)
        {
            File.Delete(outputDataFile);

            foreach (Operation operation in _operationList)
            {
                string variableName = operation.VariableName;
                AggregateOperation aggregateOperation = operation.AggregateOperation;
                PeriodOperation periodOperation = operation.PeriodOperation;
                List<double> aggregateList = new List<double>();

                foreach (KeyValuePair<ScenarioVariableKey, Dictionary<PeriodOperation, double>> keyValuePair in _masterDictionary)
                {
                    ScenarioVariableKey key = keyValuePair.Key;
                    Dictionary<PeriodOperation, double> value = keyValuePair.Value;

                    if (key.VariableName == variableName)
                    {
                        double periodAggregation = value[periodOperation];
                        aggregateList.Add(periodAggregation);
                    }
                }

                double variableNameAggregate = AggregateVariableNames(aggregateList, aggregateOperation);

                // write to console and output data file
                string message = $"{variableName.PadRight(20)},{aggregateOperation.ToString().PadRight(15)},{periodOperation.ToString().PadRight(15)}, {variableNameAggregate.ToString().PadLeft(20)}";
                Console.WriteLine(message);
                File.AppendAllText(outputDataFile, $"{message}\r\n");
            }
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
    }
}
