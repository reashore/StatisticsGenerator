
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class LastAggregation : IAggregation<double>
    {
        private readonly bool _useConcurrency;

        public LastAggregation(bool useConcurrency)
        {
            _useConcurrency = useConcurrency;
        }

        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            throw new NotImplementedException();
        }

        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return _useConcurrency ? valueSequence.AsParallel().Last() : valueSequence.Last();
        }
    }
}