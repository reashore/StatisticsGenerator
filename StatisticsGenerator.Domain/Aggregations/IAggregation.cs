
using System.Collections.Generic;

namespace StatisticsGenerator.Domain.Aggregations
{
    public interface IAggregation<T> where T : struct   // value-type constraint
    {
        T Aggregate(IEnumerable<T> valueSequence);
    }
}