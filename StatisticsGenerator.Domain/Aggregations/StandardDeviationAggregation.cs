
using System;
using System.Collections.Generic;

namespace StatisticsGenerator.Domain.Aggregations
{
    public class StandardDeviationAggregation : IAggregation<double>
    {
        private readonly bool _useConcurrency;

        public StandardDeviationAggregation(bool useConcurrency)
        {
            _useConcurrency = useConcurrency;
        }

        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            throw new NotImplementedException();
        }

        public double Aggregate(IEnumerable<double> valueSequence)
        {
            return _useConcurrency ? Utilities.ComputeStandardDeviationConcurrently(valueSequence) : Utilities.ComputeStandardDeviation(valueSequence);
        }
    }
}
