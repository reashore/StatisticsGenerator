
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class MaxAggregation : IAggregation
    {
        private readonly bool _useConcurrency;

        public MaxAggregation(bool useConcurrency)
        {
            _useConcurrency = useConcurrency;
        }

        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            //double maximumValue = (newValue > previousAggregation) ? newValue : previousAggregation;
            //return maximumValue;
            throw new NotImplementedException();
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            return _useConcurrency ? valueSequence.AsParallel().Max() : valueSequence.Max();
        }
    }
}