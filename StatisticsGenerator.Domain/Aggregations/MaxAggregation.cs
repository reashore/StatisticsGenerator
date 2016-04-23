
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class MaxAggregation : IAggregation<double>
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            //double maximumValue = (newValue > previousAggregation) ? newValue : previousAggregation;
            //return maximumValue;
            throw new NotImplementedException();
        }

        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return valueSequence.Max();
        }
    }
}