
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
            const string basePath = "../../Data";
            string configurationFile = Path.Combine(basePath, "Configuration.txt");
            string inputDataFile = Path.Combine(basePath, "InputData.txt");
            string outputDataFile = Path.Combine(basePath, "OutputData.txt");

            StatsGenerator statsGenerator = new StatsGenerator(configurationFile);
            string statisticalResults = statsGenerator.GenerateStatistics(inputDataFile, outputDataFile);

            Console.WriteLine($"\n{statisticalResults}");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
