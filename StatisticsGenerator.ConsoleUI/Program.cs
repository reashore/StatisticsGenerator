
using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using CommandLine;
using StatisticsGenerator.Domain;

using Configuration = StatisticsGenerator.Domain.Configuration;

// Although the original program was fully functional and solved the stated problem, the following enhancements were made.
// Enhancements:
// 1) Added unit tests (NUnit and MSTest)
// 2) Added Strategy design pattern
// 3) Added Moq
// 4) Added NLog
// 5) Added concurrency
// 6) Measured performance
// 7) Added missing requirement to allow different column orders
// 8) Created test data (for concurrency testing)
// 9) Created additional domain classes to fascilitate unit testing
// 10) Validated the configuration file
// 11) Validated the data file
// 12) Added support for standard deviation, which cannot be incrementally aggregated (unlike Min, Max, and Average, which can be incrementally aggregated). Stresses the design to accomodate new aggregations.
// 13) Added support for command line parsing
// 14) Added support for globalization
// 15) Ensure that there are zero compiler warnings, zero ReSharper defects, and zero Code Analysis defects
// 16) Use Task Parallel Library (TPL) to calculate standard deviation concurrently via task chaining
// 17) Create AppSettings configuration section in App.config
// 18) Use .Net 4.6.1

// todo add switch to use concurrency
// todo time operations to show that concurrenct improves performance
// todo create large canned data files for testing concurrency
// todo add NLog logging
// todo add support for command line parsing
// todo replace globalizable stings with resources
// todo calculate standard deviation via TPL
// todo check test coverage
// todo derive from class to override the aggregation methods?
// todo sign assembly

namespace StatisticsGenerator.ConsoleUI
{
    public class Program
    {
        // todo show command line examples
        public static void Main(string[] args)
        {
            CreateStatistics(args);

            Console.WriteLine(Properties.Resources.Info_PressAnyKeyToExit);
            Console.ReadKey();
        }

        //public static void CreateStatistics1(string[] args)
        //{
        //    // todo if the command arguments exist use them first

        //    // else use the values in the app.config file



        //    // the command line may contain the config.txt and inputdata.txt files

        //    //string basePath = ConfigurationManager.AppSettings["BasePath"];
        //    //string configurationFileName = ConfigurationManager.AppSettings["ConfigurationFileName"];
        //    //string inputDataFileName = ConfigurationManager.AppSettings["InputDataFileName"];
        //    //string outputDataFileName = ConfigurationManager.AppSettings["OutputDataFileName"];

        //    //// defaults
        //    //string configurationFile = Path.Combine(basePath, configurationFileName);
        //    //string inputDataFile = Path.Combine(basePath, inputDataFileName);
        //    //string outputDataFile = Path.Combine(basePath, outputDataFileName);

        //    //// parse values from command line
        //    //// todo create function
        //    //Options options = new Options();
        //    //var result = Parser.Default.ParseArguments(args, options);

        //    //var configurationFile = options.ConfigurationFile;
        //    //var inputDtaFile = options.InputDataFile;
        //    //var outputDatFile = options.OutputDataFile;


        //    const string basePath = "../../Data";
        //    string configurationFile = Path.Combine(basePath, "Configuration.txt");
        //    string inputDataFile = Path.Combine(basePath, "InputData.txt");
        //    string outputDataFile = Path.Combine(basePath, "OutputData.txt");

        //    try
        //    {
        //        StatsGenerator statsGenerator = new StatsGenerator(configurationFile);
        //        string statisticalResults = statsGenerator.GenerateStatistics(inputDataFile, outputDataFile);

        //        Console.WriteLine($"\nConfiguration file = {configurationFile}");
        //        Console.WriteLine($"Input Data file    = {inputDataFile}");
        //        Console.WriteLine($"Output file        = {outputDataFile}");
        //        Console.WriteLine($"\n{statisticalResults}");
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception.Message);
        //    }
        //}

        public static void CreateStatistics(string[] args)
        {
            try
            {
                const string basePath = "../../Data";
                string configurationFile = Path.Combine(basePath, "Configuration.txt");
                string inputDataFile = Path.Combine(basePath, "InputData.txt");
                string outputDataFile = Path.Combine(basePath, "OutputData.txt");

                Configuration configuration = new Configuration(configurationFile);
                InputData inputData = new InputData(inputDataFile, configuration);

                inputData.PerformInnerAggregations();
                string statisticalResults = inputData.PerformOuterAggregations();

                Console.WriteLine($"\nConfiguration file = {configurationFile}");
                Console.WriteLine($"Input Data file    = {inputDataFile}");
                Console.WriteLine($"Output file        = {outputDataFile}");
                Console.WriteLine($"\n{statisticalResults}");
            }
            catch (Exception exception)
            {
                // todo log exception
                Console.WriteLine(exception.Message);
            }
        }
    }

    public class Options
    {
        //[Option('r', "read", Required = true,
        //  HelpText = "Input files to be processed.")]
        //public IEnumerable<string> InputFiles { get; set; }

        //// Omitting long name, default --verbose
        //[Option(
        //  HelpText = "Prints all messages to standard output.")]
        //public bool Verbose { get; set; }

        [Option(HelpText = "Configuration File")]
        public string ConfigurationFile { get; set; }

        [Option(HelpText = "Configuration File")]
        public string InputDataFile { get; set; }

        [Option(HelpText = "Configuration File")]
        public string OutputDataFile { get; set; }

        //[Value(0, MetaName = "offset",
        //  HelpText = "File offset.")]
        //public long? Offset { get; set; }
    }

    //    var options = new Options();
    //if (CommandLine.Parser.Default.ParseArguments(args, options))
    //{
    //    // consume Options instance properties
    //    if (options.Verbose)
    //    {
    //        Console.WriteLine(options.InputFile);
    //        Console.WriteLine(options.MaximumLength);
    //    }
    //    else
    //        Console.WriteLine("working ...");
    //}
    //else
    //{
    //    // Display the default usage information
    //    Console.WriteLine(options.GetUsage());
    //}
}
