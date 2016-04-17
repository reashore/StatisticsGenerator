
namespace StatisticsGenerator.Domain
{
    public class Operation
    {
        public string VariableName { get; set; }
        public OuterAggregation OuterAggregation { get; set; }
        public PeriodAggregation PeriodAggregation { get; set; }
    }
}