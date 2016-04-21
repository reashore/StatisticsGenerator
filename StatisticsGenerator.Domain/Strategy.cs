
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsGenerator.Domain
{
    public interface IAggregation
    {
        double AggregateIncrementally(double previousAggregation, double newValue);
        double AggregateNonIncrementally(IEnumerable<double> valueSequence);
    }

    public class MinAggregation : IAggregation
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            double minimumValue = (newValue < previousAggregation) ? newValue : previousAggregation;
            return minimumValue;
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            throw new NotImplementedException();
        }
    }

    public class MaxAggregation : IAggregation
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            double maximumValue = (newValue > previousAggregation) ? newValue : previousAggregation;
            return maximumValue;
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            throw new NotImplementedException();
        }
    }

    public class FirstAggregation : IAggregation
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            throw new NotImplementedException();
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            return valueSequence.First();
        }
    }

    public class LastAggregation : IAggregation
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            throw new NotImplementedException();
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            return valueSequence.Last();
        }
    }

    public class AverageAggregation : IAggregation
    {
        public double AggregateIncrementally(double previousAggregation, double newValue)
        {
            // todo Average must be divided by the count if done incrementally
            throw new NotImplementedException();
        }

        public double AggregateNonIncrementally(IEnumerable<double> valueSequence)
        {
            return valueSequence.Average();
        }
    }


}
