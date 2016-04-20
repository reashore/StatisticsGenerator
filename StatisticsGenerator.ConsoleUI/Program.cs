
using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using CommandLine;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.ConsoleUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //string basePath = ConfigurationManager.AppSettings["BasePath"];
            //string configurationFileName = ConfigurationManager.AppSettings["ConfigurationFileName"];
            //string inputDataFileName = ConfigurationManager.AppSettings["InputDataFileName"];
            //string outputDataFileName = ConfigurationManager.AppSettings["OutputDataFileName"];

            //// defaults
            //string configurationFile = Path.Combine(basePath, configurationFileName);
            //string inputDataFile = Path.Combine(basePath, inputDataFileName);
            //string outputDataFile = Path.Combine(basePath, outputDataFileName);

            //// parse values from command line
            //// todo create function
            //Options options = new Options();
            //var result = Parser.Default.ParseArguments(args, options);

            //var configurationFile = options.ConfigurationFile;
            //var inputDtaFile = options.InputDataFile;
            //var outputDatFile = options.OutputDataFile;


            const string basePath = "../../Data";
            string configurationFile = Path.Combine(basePath, "Configuration.txt");
            string inputDataFile = Path.Combine(basePath, "InputData.txt");
            string outputDataFile = Path.Combine(basePath, "OutputData.txt");

            try
            {
                StatsGenerator statsGenerator = new StatsGenerator(configurationFile);
                string statisticalResults = statsGenerator.GenerateStatistics(inputDataFile, outputDataFile);

                Console.WriteLine($"\nConfiguration file = {configurationFile}");
                Console.WriteLine($"Input Data file    = {inputDataFile}");
                Console.WriteLine($"Output file        = {outputDataFile}");
                Console.WriteLine($"\n{statisticalResults}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
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
}
