
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class MinAggregation : IAggregation<double>
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            //double minimumValue = (newValue < previousAggregation) ? newValue : previousAggregation;
            //return minimumValue;
            throw new NotImplementedException();
        }

        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return valueSequence.Min();
        }
    }
}