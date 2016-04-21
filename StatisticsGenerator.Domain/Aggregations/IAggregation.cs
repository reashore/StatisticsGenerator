
using System.Collections.Generic;

namespace StatisticsGenerator.Domain.Aggregations
{
    public interface IAggregation
    {
        double AggregateIncrementally(double previousAggregation, double newValue);
        double AggregateNonIncrementally(IEnumerable<double> valueSequence);
    }
}