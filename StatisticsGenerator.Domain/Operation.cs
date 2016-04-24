
namespace StatisticsGenerator.Domain
{
    public struct Operation
    {
        public string VariableName { get; set; }
        public OuterAggregation OuterAggregation { get; set; }
        public PeriodAggregation PeriodAggregation { get; set; }
    }
}