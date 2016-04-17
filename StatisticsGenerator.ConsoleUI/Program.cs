
using System;
using System.IO;
using StatisticsGenerator.Domain;

namespace StatisticsGenerator.ConsoleUI
{
    public class Program
    {
        public static void Main()
        {
            // for simplicity in this demo, just hard-code these file locations
            const string basePath = @"..\..\Data";
            string configurationFile = Path.Combine(basePath, "Configuration.txt");
            string inputDataFile = Path.Combine(basePath, "InputData.txt");
            string outputDataFile = Path.Combine(basePath, "OutputData.txt");

            try
            {
                StatsGenerator statsGenerator = new StatsGenerator(configurationFile);
                string statisticalResults = statsGenerator.GenerateStatistics(inputDataFile, outputDataFile);

                Console.WriteLine($"\nConfiguration file = {configurationFile}");
                Console.WriteLine($"Input Data file    = {configurationFile}");
                Console.WriteLine($"Output file        = {configurationFile}");
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
}
