
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class AverageAggregation : IAggregation<double>
    {
        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return valueSequence.Average();
        }
    }
}