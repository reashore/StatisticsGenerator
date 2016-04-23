
using System;
using System.Collections.Generic;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class StandardDeviationAggregation : IAggregation<double>
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            throw new NotImplementedException();
        }

        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return Utilities.ComputeStandardDeviation(valueSequence);
        }
    }
}
