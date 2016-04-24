
using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;
using StatisticsGenerator.Domain;
using NLog;

using Configuration = StatisticsGenerator.Domain.Configuration;

namespace StatisticsGenerator.ConsoleUI
{
    // Command line usage examples:
    //      >StatisticsGenerator.ConsoleUI --ConfigurationFile C:\Data\Configuration.txt --InputDataFile C:\Data\InputData.txt --OutputDataFile C:\Data\OutputDataFile.txt
    //      >StatisticsGenerator.ConsoleUI --ConfigurationFile C:\Data\Configuration.txt 
    //      >StatisticsGenerator.ConsoleUI --InputDataFile C:\Data\InputData.txt
    //      >StatisticsGenerator.ConsoleUI -c C:\Data\Configuration.txt -i C:\Data\InputData.txt -o C:\Data\OutputDataFile.txt
    //      >StatisticsGenerator.ConsoleUI -c C:\Data\Configuration.txt 
    //      >StatisticsGenerator.ConsoleUI -i C:\Data\InputData.txt
    //      >StatisticsGenerator.ConsoleUI -o C:\Data\OutputDataFile.txt
    //      >StatisticsGenerator.ConsoleUI 
    //      >StatisticsGenerator.ConsoleUI --Help

    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] commandLineArguments)
        {
            try
            {
                string configurationFile;
                string inputDataFile;
                string outputDataFile;

                if (!ParseCommandLineArguments(commandLineArguments, out configurationFile, out inputDataFile, out outputDataFile))
                {
                    return;
                }

                CreateStatistics(configurationFile, inputDataFile, outputDataFile);

            }
            catch (Exception exception)
            {
                Environment.ExitCode = 1;
                Logger.Error(exception);
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine(Properties.Resources.Info_PressAnyKeyToExit);
            Console.ReadKey();

            Environment.ExitCode = 0;
        }

        private static bool ParseCommandLineArguments(string[] commandLineArguments, out string configurationFile, out string inputDataFile, out string outputDataFile)
        {
            GetFileDefaultsFromAppSettings(out configurationFile, out inputDataFile, out outputDataFile);

            Options options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(commandLineArguments, options))
            {
                if (options.ConfigurationFile != null)
                {
                    configurationFile = options.ConfigurationFile;
                }

                if (options.InputDataFile != null)
                {
                    inputDataFile = options.InputDataFile;
                }

                if (options.OutputDataFile != null)
                {
                    outputDataFile = options.OutputDataFile;
                }
            }
            else
            {
                Console.WriteLine(Options.GetUsage());
                return false;
            }

            return true;
        }

        private static void GetFileDefaultsFromAppSettings(out string defaultConfigurationFile, out string defaultInputDataFile, out string defaultOutputDataFile)
        {
            string basePath = ConfigurationManager.AppSettings["BasePath"];

            string defaultConfigurationFileName = ConfigurationManager.AppSettings["ConfigurationFileName"];
            string defaultInputDataFileName = ConfigurationManager.AppSettings["InputDataFileName"];
            string defaultOutputDataFileName = ConfigurationManager.AppSettings["OutputDataFileName"];

            defaultConfigurationFile = Path.Combine(basePath, defaultConfigurationFileName);
            defaultInputDataFile = Path.Combine(basePath, defaultInputDataFileName);
            defaultOutputDataFile = Path.Combine(basePath, defaultOutputDataFileName);
        }

        private static void CreateStatistics(string configurationFile, string inputDataFile, string outputDataFile)
        {
            Configuration configuration = new Configuration(configurationFile);
            InputData inputData = new InputData(inputDataFile, configuration);
            Dictionary<Operation, double> resultsDictionary = inputData.CreateStatistics();

            string statisticalResults = FormatResults(resultsDictionary);
            File.WriteAllText(outputDataFile, statisticalResults);

            Console.WriteLine("\nStatistics Generator\n");
            Console.WriteLine($"Configuration file = {configurationFile}");
            Console.WriteLine($"Input Data file    = {inputDataFile}");
            Console.WriteLine($"Output file        = {outputDataFile}");
            Console.WriteLine($"\n{statisticalResults}");
            Console.WriteLine(ShowEnvironment());
        }

        private static string FormatResults(Dictionary<Operation, double> resultsDictionary)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var keyValuePair in resultsDictionary)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;

                string variableName = key.VariableName;
                OuterAggregation outerAggregation = key.OuterAggregation;
                PeriodAggregation periodAggregation = key.PeriodAggregation;

                double outerAggregationValue = value;

                string keyFormat = $"({variableName.PadRight(17)},{outerAggregation.ToString().PadRight(10)},{periodAggregation.ToString().PadRight(6)})";
                string valueFormat = $"{outerAggregationValue.ToString("F4", CultureInfo.InvariantCulture).PadLeft(14)}";
                string message = $"{keyFormat} = {valueFormat}";
                stringBuilder.AppendLine(message);
            }
            
            return stringBuilder.ToString();
        }

        // ReSharper disable once UnusedMember.Local
        private static void TestNlogLogging()
        {
            Logger.Trace("Sample trace message");
            Logger.Debug("Sample debug message");
            Logger.Info("Sample informational message");
            Logger.Warn("Sample warning message");
            Logger.Error("Sample error message");
            Logger.Fatal("Sample fatal error message");

            Logger.Log(LogLevel.Info, "Sample informational message");
        }

        private static string ShowEnvironment()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string machineName = Environment.MachineName;
            OperatingSystem osVersion = Environment.OSVersion;
            int processorCount = Environment.ProcessorCount;
            Version dotnetVersion = Environment.Version;
            string currentDirectory = Environment.CurrentDirectory;

            Version buildVersion = Assembly.GetExecutingAssembly().GetName().Version;

            stringBuilder.AppendLine($"Machine Name         = {machineName}");
            stringBuilder.AppendLine($"OS Version           = {osVersion}");
            stringBuilder.AppendLine($"Processor Count      = {processorCount}");
            stringBuilder.AppendLine($"Version              = {dotnetVersion}");
            stringBuilder.AppendLine($"Current Directory    = {currentDirectory}");
            stringBuilder.AppendLine($"Build Version        = {buildVersion}");

            return stringBuilder.ToString();
        }
    }
}
