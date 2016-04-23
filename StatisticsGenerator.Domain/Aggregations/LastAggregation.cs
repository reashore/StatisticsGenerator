
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class LastAggregation : IAggregation<double>
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            throw new NotImplementedException();
        }

        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return valueSequence.Last();
        }
    }
}