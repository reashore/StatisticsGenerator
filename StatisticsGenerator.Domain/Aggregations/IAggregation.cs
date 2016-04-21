using System.Collections.Generic;

namespace StatisticsGenerator.Domain.Aggregations
{
    public interface IAggregation
    {
        // todo add UseConcurrency?
        double AggregateIncrementally(double previousAggregation, double newValue);
        double AggregateNonIncrementally(IEnumerable<double> valueSequence);
    }
}