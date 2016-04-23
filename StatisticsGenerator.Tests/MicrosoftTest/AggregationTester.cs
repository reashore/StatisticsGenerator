
using System.Collections.Generic;
using StatisticsGenerator.Domain.Aggregations;

namespace StatisticsGenerator.Tests.MicrosoftTest
{
    public class AggregationTester
    {
        private List<double> _data;

        public IAggregation<double> AggregationStrategy { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double First { get; set; }
        public double Last { get; set; }
        public double Average { get; set; }
        public double StandardDeviation { get; set; }

        public void CreateData()
        {
            _data = new List<double>();

            for (int n = 0; n < 1000000; n++)
            {
                _data[n] =(n % 2 == 0) ? 2 : 1;
            }
        }

        public void TestAggregation()
        {
            CreateData();

            AggregationStrategy = new MinAggregation();
            Min = Aggregate(_data);

            AggregationStrategy = new MaxAggregation();
            Max = Aggregate(_data);

            AggregationStrategy = new FirstAggregation();
            First = Aggregate(_data);

            AggregationStrategy = new LastAggregation();
            Last = Aggregate(_data);

            AggregationStrategy = new AverageAggregation();
            Average = Aggregate(_data);

            AggregationStrategy = new StandardDeviationAggregation();
            StandardDeviation = Aggregate(_data);
        }

        public double Aggregate(IEnumerable<double> sequence)
        {
            return AggregationStrategy.Aggregate(sequence);
        }
    }
}
