
using System.Collections.Generic;

namespace StatisticsGenerator.Domain.Aggregations
{
    public interface IAggregation
    {
        double AggregateIncrementally(double previousAggregation, double newValue);
        // todo rename to Aggregate()
        double AggregateNonIncrementally(IEnumerable<double> valueSequence);
    }
}