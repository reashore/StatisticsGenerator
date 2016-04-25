
using System.Collections.Generic;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class StandardDeviationAggregation : IAggregation<double>
    {
        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return valueSequence.StandardDeviation();
        }
    }
}
