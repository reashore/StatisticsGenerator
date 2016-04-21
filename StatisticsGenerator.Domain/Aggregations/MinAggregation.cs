using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class MinAggregation : IAggregation
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            //double minimumValue = (newValue < previousAggregation) ? newValue : previousAggregation;
            //return minimumValue;
            throw new NotImplementedException();
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            return valueSequence.Min();
        }
    }
}