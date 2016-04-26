
namespace StatisticsGenerator.Domain
{
    // Operation is a struct rather than a class since it fascilitates comparing dictionay keys
    public struct Operation
    {
        public string VariableName { get; set; }
        public OuterAggregation OuterAggregation { get; set; }
        public PeriodAggregation PeriodAggregation { get; set; }
    }
}