using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsGenerator.Domain
{
    class DataLine
    {
        private string _line;
        private string _numberColumns;

        // todo pass in configuration
        public DataLine(string line, string numberColumns)
        {
            _line = line;
            _numberColumns = numberColumns;
        }

        public int ScenarioId { get; set; }
        public string VariableName { get; set; }
        public double[] PeriodValueArray { get; set; }
        public bool IsVariableProcessed { get; set; }
        

        // will have Aggregate() operation
        // will have a property to set the AggregationStrategy
        // will create and populate dictionary of aggregated results

        // will require a list of aggregations

        // perhaps reading the header should be part of this class since it will avoid having to pass column mappings

    }
}
