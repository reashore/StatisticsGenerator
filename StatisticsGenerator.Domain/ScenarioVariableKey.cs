
namespace StatisticsGenerator.Domain
{
    // use struct rather than class since it makes it easier to compare dictionary keys
    public struct ScenarioVariableNameKey
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ScenarioId { get; set; }
        public string VariableName { get; set; }
    }
}