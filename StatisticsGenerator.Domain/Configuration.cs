
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StatisticsGenerator.Domain
{
    public interface IConfiguration
    {
        List<Operation> Operations { get; }
        List<PeriodAggregation> GetPeriodAggregationsForVariable(string variableName);
        bool IsVariableProcessed(string variableName);
    }

    public class Configuration : IConfiguration
    {
        private List<Operation> _operationList;

        public Configuration(string configurationFile)
        {
            ReadConfigurationFile(configurationFile);
        }

        public List<Operation> Operations => _operationList;

        public List<PeriodAggregation> GetPeriodAggregationsForVariable(string variableName)
        {
            List<PeriodAggregation> periodOperationList = new List<PeriodAggregation>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Operation operation in _operationList)
            {
                if (operation.VariableName == variableName)
                {
                    // duplicates not allowed
                    if (!periodOperationList.Contains(operation.PeriodAggregation))
                    {
                        periodOperationList.Add(operation.PeriodAggregation);
                    }
                }
            }

            return periodOperationList;
        }

        public bool IsVariableProcessed(string variableName)
        {
            return _operationList.Any(operation => operation.VariableName == variableName);
        }

        public List<OuterAggregation> GetOuterAggregationsForVariable(string variableName)
        {
            List<OuterAggregation> outerAggregationList = new List<OuterAggregation>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Operation operation in _operationList)
            {
                if (operation.VariableName == variableName)
                {
                    // duplicates not allowed
                    if (!outerAggregationList.Contains(operation.OuterAggregation))
                    {
                        outerAggregationList.Add(operation.OuterAggregation);
                    }
                }
            }

            return outerAggregationList;
        }

        public List<string> GetVariableNames()
        {
            List<string> variableNames = new List<string>();

            foreach (Operation operation in _operationList)
            {
                // duplicates not allowed
                if (!variableNames.Contains(operation.VariableName))
                {
                    variableNames.Add(operation.VariableName);
                }
            }

            return variableNames;
        }

        #region Private Methods

        private void ReadConfigurationFile(string configurationFile)
        {
            if (configurationFile == null)
            {
                throw new ArgumentNullException(nameof(configurationFile));    
            }

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

        #endregion
    }
}
