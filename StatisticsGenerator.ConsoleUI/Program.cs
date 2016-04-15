
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StatisticsGenerator.ConsoleUI
{
    // todo move to domain assembly eventually
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

    public class Operation
    {
        public string VariableName { get; set; }
        public AggregateOperation AggregateOperation { get; set; }
        public PeriodOperation PeriodOperation { get; set; }
    }

    // easier to compare if they are structs
    public struct ScenarioVariableKey
    {
        public int ScenarioId { get; set; }
        public string VariableName { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // for simplicity just hard-code these file locations
            const string configurationFile = "../../Data/Configuration.txt";
            const string inputDataFile = "../../Data/InputData.txt";

            List<Operation> operationList = ReadConfigurationFile(configurationFile);

            Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>> masterDictionary = ProcessData(inputDataFile, operationList);

            CreateProcessedDataFile();

            DisplayOperationList(operationList);

            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
        }

        private static void DisplayOperationList(List<Operation> operationList)
        {
            foreach (Operation operation in operationList)
            {
                string variable = operation.VariableName;
                AggregateOperation aggregateOperation = operation.AggregateOperation;
                PeriodOperation periodOperation = operation.PeriodOperation;

                string message = $"{variable}, {aggregateOperation}, {periodOperation}";
                Console.WriteLine(message);
            }
        }

        private static List<Operation> ReadConfigurationFile(string configurationFile)
        {
            string[] lines = File.ReadAllLines(configurationFile);
            List<Operation> operationList = new List<Operation>();

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

                operationList.Add(operation);
            }

            return operationList;
        }

        private static void CreateProcessedDataFile()
        {
            // create output file from dictionary with key (ScenarioId, Variablename)

        }

        private static Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>> ProcessData(string inputDataFile, List<Operation> operationList)
        {
            Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>> masterDictionary = new Dictionary<ScenarioVariableKey, Dictionary<PeriodOperation, double>>();

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
                        // todo calculate scenario aggregates
                        break;
                    }

                    int scenarioId;
                    string variableName;
                    double[] periodValueArray;
                    
                    ParseDataLine(line, numberPeriods, out scenarioId, out variableName, out periodValueArray);

                    bool isVariableProcessed = IsVariableProcessed(variableName, operationList);

                    // skip line if variable is not configured to be processed
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

                    List<PeriodOperation> periodAggregationList = GetPeriodOperationsForVariable(variableName, operationList);
                    Dictionary<PeriodOperation, double> periodAggregationDictionary = CreatePeriodAggregationsDictionary(periodAggregationList, periodValueArray);

                    masterDictionary[scenarioVariableKey] = periodAggregationDictionary;
                }
            }

            return masterDictionary;
        }

        private static Dictionary<PeriodOperation, double> CreatePeriodAggregationsDictionary(List<PeriodOperation> periodAggregationList, double[] periodValueArray)
        {
            Dictionary<PeriodOperation, double> periodAggregationDictionary = new Dictionary<PeriodOperation, double>();

            // todo rename to PeriodAggregation
            foreach (PeriodOperation periodOperation in periodAggregationList)
            {
                double result = AggregatePeriods(periodValueArray, periodOperation);
                periodAggregationDictionary[periodOperation] = result;
            }

            return periodAggregationDictionary;
        }

        //private static void ProcessData2(string inputDataFile, List<Operation> operationList)
        //{
        //    using (FileStream fileStream = File.OpenRead(inputDataFile))
        //    using (TextReader textReader = new StreamReader(fileStream))
        //    {
        //        int numberPeriods = ReadDataHeader(textReader);
        //        int previousScenarioId = -1;
        //        Dictionary<string, double> aggregatesByVariableDictionary = new Dictionary<string, double>();
        //        Dictionary<string, double> periodAggregationDictionary;
        //        Dictionary<string, double> variableAggregationDictionary = new Dictionary<string, double>();

        //        while (true)
        //        {
        //            string line = textReader.ReadLine();

        //            // check for end of file
        //            if (line == null)
        //            {
        //                // todo calculate scenario aggregates
        //                break;
        //            }

        //            int scenarioId;
        //            string variableName;
        //            double[] periodValueArray;
                    
        //            ParseDataLine(line, numberPeriods, out scenarioId, out variableName, out periodValueArray);

        //            ScenarioVariableKey ScenarioVariableKey = new ScenarioVariableKey
        //            {
        //                ScenarioId = scenarioId,
        //                VariableName = variableName
        //            };

        //            // todo rename
        //            Dictionary< ScenarioVariableKey, double> foo = new Dictionary<ScenarioVariableKey, double>();

        //            // todo everything beow here should be in a function
        //            bool isNewScenario = scenarioId != previousScenarioId;

        //            if (isNewScenario)
        //            {
        //                periodAggregationDictionary = new Dictionary<string, double>();


        //                // todo calculate scenario aggregates
        //                // save aggregates dictionary from previous scenario
        //                // todo rename to Dictionary
        //                Dictionary<string, double> previousAggregatesByVariable = aggregatesByVariableDictionary;
        //                aggregatesByVariableDictionary = new Dictionary<string, double>();


        //                CalculateAggregates(previousAggregatesByVariable);

        //                // save the current scenarioId
        //                previousScenarioId = scenarioId;
        //            }

        //            // add new variable name to aggregation dictionary
        //            if (!aggregatesByVariableDictionary.ContainsKey(variableName))
        //            {
        //                // todo set to 0 for now
        //                aggregatesByVariableDictionary[variableName] = 0;
        //            }
                    
        //            bool isVariableProcessed = IsVariableProcessed(variableName, operationList);

        //            // skip line if variable is not configured to be processed
        //            if (!isVariableProcessed)
        //            {
        //                continue;
        //            }

        //            List<PeriodOperation> periodOperationsForVariableList = GetPeriodOperationsForVariable(variableName, operationList);
        //            List<AggregateOperation> aggregateOperationsForVariableList = GetAggregateOperationsForVariable(variableName, operationList);


        //            foreach (PeriodOperation periodOperation in periodOperationsForVariableList)
        //            {
        //                // todo where to save these values
        //                double aggregatedPeriodValue = AggregatePeriods(periodValueArray, periodOperation);
        //                periodAggregationDictionary[variableName] = aggregatedPeriodValue;

        //            }



        //        }

        //        // calculate aggregate
        //        // generate output file
        //    }
        //}

        private static void CalculateAggregates(Dictionary<string, double> previousAggregatesByVariable)
        {
            
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

        private static double AggregatePeriods(double[] periodValuesArray, PeriodOperation periodOperation)
        {
            double result;
            int numberPeriods = periodValuesArray.Length;

            switch (periodOperation)
            {
                case PeriodOperation.FirstValue:
                    result = periodValuesArray[0];
                    break;

                case PeriodOperation.LastValue:
                    result = periodValuesArray[numberPeriods -1];
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

        private static bool IsVariableProcessed(string variableName, List<Operation> operationList)
        {
            return operationList.Any(operation => operation.VariableName == variableName);
        }

        private static List<PeriodOperation> GetPeriodOperationsForVariable(string variableName, List<Operation> operationList)
        {
            List<PeriodOperation> periodOperationList = new List<PeriodOperation>();

            foreach (Operation operation in operationList)
            {
                if (operation.VariableName == variableName)
                {
                    periodOperationList.Add(operation.PeriodOperation);
                }
            }

            return periodOperationList;
        }

        private static List<AggregateOperation> GetAggregateOperationsForVariable(string variableName, List<Operation> operationList)
        {
            List<AggregateOperation> aggregateOperationList = new List<AggregateOperation>();

            foreach (Operation operation in operationList)
            {
                if (operation.VariableName == variableName)
                {
                    aggregateOperationList.Add(operation.AggregateOperation);
                }
            }

            return aggregateOperationList;
        }

        private static Dictionary<PeriodOperation, double> CreatePeriodAggregationDictionary(List<PeriodOperation> periodOperationList)
        {
            Dictionary<PeriodOperation, double> periodAggregationDictionary = new Dictionary<PeriodOperation, double>();

            foreach (PeriodOperation periodOperation in periodOperationList)
            {
                periodAggregationDictionary[periodOperation] = 0;
            }

            return periodAggregationDictionary;
        }
    }
}
