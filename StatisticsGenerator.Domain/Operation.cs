
namespace StatisticsGenerator.Domain
{
    public class Operation
    {
        public string VariableName { get; set; }
        public AggregateOperation AggregateOperation { get; set; }
        public PeriodAggregation PeriodAggregation { get; set; }
    }
}