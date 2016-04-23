
using System.Text;
using CommandLine;

namespace StatisticsGenerator.ConsoleUI
{
    public class Options
    {
        [Option('c', "ConfigurationFile" ,HelpText = "Configuration File")]
        public string ConfigurationFile { get; set; }

        [Option('i', "InputDataFile", HelpText = "Input Data File")]
        public string InputDataFile { get; set; }

        [Option('o', "OutputDataFile", HelpText = "Output Data File")]
        public string OutputDataFile { get; set; }

        [HelpOption]
        public static string GetUsage()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("CreateStatistics Application 1.0");
            stringBuilder.AppendLine("See user manual for usage instructions.");

            return stringBuilder.ToString();
        }
    }
}