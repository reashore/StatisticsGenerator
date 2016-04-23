
using System;
using System.IO;
using System.Configuration;
using StatisticsGenerator.Domain;

using Configuration = StatisticsGenerator.Domain.Configuration;

// Although the original program was fully functional and solved the stated problem, 
// the following enhancements were made in order to create an "Enterprise" version.
//
// Enhancements:
// 1) Added unit tests (NUnit and MSTest)
// 2) Added Strategy design pattern
// 3) created finer grained object model to fascilitate unit testing
// 4) Added NLog
// 7) Added missing requirement to allow different column orders
// 12) Added support for standard deviation, which cannot be incrementally aggregated (unlike Min, Max, and Average, which can be incrementally aggregated). Stresses the design to accomodate new aggregations.
// 13) Added support for command line parsing
// 14) Globalization strings

// todo add NLog logging
// todo replace globalizable stings with resources
// todo sign assembly
// todo if variable name is not in configuration then variable is not aggregated
// todo cleanup test project root

namespace StatisticsGenerator.ConsoleUI
{
    // Command line usage:
    //      >StatisticsGenerator.ConsoleUI --ConfigurationFile C:\Data\Configuration.txt --InputDataFile C:\Data\InputData.txt --OutputDataFile C:\Data\OutputDataFile.txt
    //      >StatisticsGenerator.ConsoleUI -c C:\Data\Configuration.txt -i C:\Data\InputData.txt -o C:\Data\OutputDataFile.txt
    //      >StatisticsGenerator.ConsoleUI -c C:\Data\Configuration.txt 
    //      >StatisticsGenerator.ConsoleUI -i C:\Data\InputData.txt
    //      >StatisticsGenerator.ConsoleUI -o C:\Data\OutputDataFile.txt
    //      >StatisticsGenerator.ConsoleUI 
    //      >StatisticsGenerator.ConsoleUI --Help

    public static class Program
    {
        public static void Main(string[] commandLineArguments)
        {
            string configurationFile;
            string inputDataFile;
            string outputDataFile;

            if (ParseCommandLineArguments(commandLineArguments, out configurationFile, out inputDataFile, out outputDataFile))
            {
                CreateStatistics(configurationFile, inputDataFile, outputDataFile);

                Console.WriteLine(Properties.Resources.Info_PressAnyKeyToExit);
                Console.ReadKey();
            }
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
            try
            {
                Configuration configuration = new Configuration(configurationFile);
                InputData inputData = new InputData(inputDataFile, configuration);
                string statisticalResults = inputData.CreateStatistics();
                File.WriteAllText(outputDataFile, statisticalResults);

                Console.WriteLine("\nStatistics Generator\n");
                Console.WriteLine($"Configuration file = {configurationFile}");
                Console.WriteLine($"Input Data file    = {inputDataFile}");
                Console.WriteLine($"Output file        = {outputDataFile}");
                Console.WriteLine($"\n{statisticalResults}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
