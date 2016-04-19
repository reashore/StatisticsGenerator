
using System;
using System.Collections.Generic;
using System.IO;

namespace StatisticsGenerator.Domain
{
    public interface IConfiguration
    {
        int ReadConfigurationFile(string configurationFile);
        List<Operation> Operations { get; set; }
    }

    public class Configuration : IConfiguration
    {
        private List<Operation> _operationList;

        public Configuration()
        {
        }

        public Configuration(string configurationFile)
        {
            ReadConfigurationFile(configurationFile);
        }

        //public List<Operation> Operations => _operationList;
        // todo do not allow public set
        public List<Operation> Operations
        {
            get
            {
                return _operationList;
            }

            set
            {
                _operationList = value;
            }
        }

        public int ReadConfigurationFile(string configurationFile)
        {
            string[] lines = File.ReadAllLines(configurationFile);
            _operationList = new List<Operation>();

            foreach (string line in lines)
            {
                // Skip blank lines in configuration file
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                Operation operation = ParseConfigurationLine(line);
                _operationList.Add(operation);
            }

            return _operationList?.Count ?? 0;
        }

        private static Operation ParseConfigurationLine(string line)
        {
            string[] arguments = line.Split('\t');
            Operation operation = new Operation { VariableName = arguments[0] };

            OuterAggregation outerAggregation;
            bool parseSucceeded = Enum.TryParse(arguments[1], out outerAggregation);
            if (!parseSucceeded)
            {
                throw new Exception("Invalid OuterAggregation in configuration file");
            }
            operation.OuterAggregation = outerAggregation;

            PeriodAggregation periodAggregation;
            parseSucceeded = Enum.TryParse(arguments[2], out periodAggregation);
            if (!parseSucceeded)
            {
                throw new Exception("Invalid PeriodAggregation in configuration file");
            }
            operation.PeriodAggregation = periodAggregation;

            return operation;
        }
    }
}
