using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StatisticsGenerator.ConsoleUI
{
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

    public class Program
    {
        // todo move data files into Data directory
        public static void Main(string[] args)
        {
            List<Operation> operationList = ReadConfigurationFile();

            //Operation operation1 = new Operation
            //{
            //    Variable = "AvePolLoadYield",
            //    AggregateOperation = AggregateOperation.Average,
            //    PeriodOperation = PeriodOperation.MaxValue
            //};

            ProcessData(operationList);
            CreateProcessedDataFile();

            foreach (Operation operation in operationList)
            {
                string variable = operation.VariableName;
                AggregateOperation aggregateOperation = operation.AggregateOperation;
                PeriodOperation periodOperation = operation.PeriodOperation;

                string message = $"{variable}, {aggregateOperation}, {periodOperation}";
                Console.WriteLine(message);
            }

            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
        }

        // add config file as parameter
        private static List<Operation> ReadConfigurationFile()
        {
            const string configurationFile = "../../Configuration.txt";
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

        }

        // todo replace with list of operations
        private static void ProcessData(List<Operation> operationList)
        {
            const string inputDataFile = "../../InputData.txt";
            using (FileStream fileStream = File.OpenRead(inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                int numberPeriods = ReadDataHeader(textReader);

                while (true)
                {
                    string line = textReader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    int scenarioId;
                    string variableName;
                    double[] periodValueArray;

                    ParseDataLine(line, numberPeriods, out scenarioId, out variableName, out periodValueArray);
                    bool isVariableprocessed = IsVariableProcessed(variableName, operationList);

                    //string[] segments = line.Split('\t');

                    //bool parseSucceeded = int.TryParse(segments[0], out scenarioId);
                    //if (!parseSucceeded)
                    //{
                    //    throw new Exception("Invalid ScenarioId in data file");
                    //}

                    //variableName = segments[1];


                    //for (int n = 2; n < numberPeriods; n++)
                    //{
                    //    double value;
                    //    parseSucceeded = double.TryParse(segments[n], out value);
                    //    if (!parseSucceeded)
                    //    {
                    //        throw new Exception("Invalid data value in data file");
                    //    }
                    //    periodValueArray[n] = value;
                    //}

                    // todo fix later
                    //double processedPeriodValue = ProcessPeriods(periodValuesArray, operation.PeriodOperation);


                    Console.WriteLine(line);
                }
            }
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

        private static double ProcessPeriods(double[] periodValuesArray, PeriodOperation periodOperation)
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
    }
}
