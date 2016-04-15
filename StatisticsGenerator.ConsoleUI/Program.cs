using System;
using System.Collections.Generic;
using System.IO;

namespace StatisticsGenerator.ConsoleUI
{
    public enum Calculation
    {
        MinValue,
        MaxValue,
        Average
    }

    public enum Period
    {
        FirstValue,
        LastValue,
        MinValue,
        MaxValue
    }

    public class Operation
    {
        public string Variable { get; set; }
        public Calculation Calculation { get; set; }
        public Period Period { get; set; }
    }

    public class Program
    {
        // todo move data files into Data directory
        public static void Main(string[] args)
        {
            List<Operation> operationList = ReadConfigurationFile();

            Operation operation1 = new Operation
            {
                Variable = "AvePolLoadYield",
                Calculation = Calculation.Average,
                Period = Period.MaxValue
            };

            ProcessData(operation1);
            CreateProcessedDataFile();

            foreach (Operation operation in operationList)
            {
                string variable = operation.Variable;
                Calculation calculation = operation.Calculation;
                Period period = operation.Period;

                string message = $"{variable}, {calculation}, {period}";
                Console.WriteLine(message);
            }

            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
        }

        // add config file as parameter
        // return list of operations
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

                operation.Variable = arguments[0];

                Calculation calculation;
                bool parseSucceeded = Enum.TryParse(arguments[1], out calculation);
                if (!parseSucceeded)
                {
                    throw new Exception("Invalid calculation in configuration file");
                }
                operation.Calculation = calculation;

                Period period;
                parseSucceeded = Enum.TryParse(arguments[2], out period);
                if (!parseSucceeded)
                {
                    throw new Exception("Invalid period in configuration file");
                }
                operation.Period = period;

                operationList.Add(operation);
            }

            return operationList;
        }


        private static void CreateProcessedDataFile()
        {

        }

        private static void ProcessData(Operation operation)
        {
            const string inputDataFile = "../../InputData.txt";
            using (FileStream fileStream = File.OpenRead(inputDataFile))
            using (TextReader textReader = new StreamReader(fileStream))
            {
                // todo handle first line separately
                string firstLine = textReader.ReadLine();

                if (firstLine == null)
                {
                    throw new Exception("Input data file contains empty first row");
                }

                string[] columnHeaderArray = firstLine.Split('\t');
                // the number of periods is the length of the column header array 
                // minus the first two column headers (ScenarioId and VariableName)
                int numberPeriods = columnHeaderArray.Length - 2;
                List<double> periodsList = new List<double>();

                while (true)
                {
                    string line = textReader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    string[] segments = line.Split('\t');

                    int scenarioId;
                    bool parseSucceeded = int.TryParse(segments[0], out scenarioId);
                    if (!parseSucceeded)
                    {
                        throw new Exception("Invalid ScenarioId in data file");
                    }

                    string variableName = segments[1];

                    // todo move these values into an array
                    double value;
                    parseSucceeded = double.TryParse(segments[2], out value);
                    if (!parseSucceeded)
                    {
                        throw new Exception("Invalid data value in data file");
                    }


                    Console.WriteLine(line);
                }
            }
        }

    }
}
