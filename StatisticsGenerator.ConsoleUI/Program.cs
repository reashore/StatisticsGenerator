
using System;
using System.IO;
using System.Configuration;
using StatisticsGenerator.Domain;

using Configuration = StatisticsGenerator.Domain.Configuration;

// todo add NLog logging
// todo sign assembly

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

                Console.WriteLine(Properties.Resources.Info_PressAnyKeyToExit);
                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
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
    }
}
