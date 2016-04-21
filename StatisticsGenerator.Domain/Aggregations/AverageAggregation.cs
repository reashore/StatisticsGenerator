
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class AverageAggregation : IAggregation
    {
        private readonly bool _useConcurrency;

        public AverageAggregation(bool useConcurrency)
        {
            _useConcurrency = useConcurrency;
        }

        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            throw new NotImplementedException();
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            return _useConcurrency ? valueSequence.AsParallel().Average() : valueSequence.Average();
        }
    }
}