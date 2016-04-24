
using System;

namespace StatisticsGenerator.Domain
{
    public static class DoubleExtensions
    {
        private const double Digits2 = 0.01;
        private const double Digits3 = 0.001;
        private const double Digits4 = 0.0001;
        private const double Digits5 = 0.00001;
        private const double Digits6 = 0.000001;
        private const double Digits7 = 0.0000001;
        private const double Digits8 = 0.00000001;

        // ReSharper disable UnusedMember.Global
        public static bool EqualTo2Digits(this double left, double right)
        {
            return Math.Abs(left - right) < Digits2;
        }

        public static bool EqualTo3Digits(this double left, double right)
        {
            return Math.Abs(left - right) < Digits3;
        }

        public static bool EqualTo4Digits(this double left, double right)
        {
            return Math.Abs(left - right) < Digits4;
        }

        public static bool EqualTo5Digits(this double left, double right)
        {
            return Math.Abs(left - right) < Digits5;
        }

        public static bool EqualTo6Digits(this double left, double right)
        {
            return Math.Abs(left - right) < Digits6;
        }

        public static bool EqualTo7Digits(this double left, double right)
        {
            return Math.Abs(left - right) < Digits7;
        }

        public static bool EqualTo8Digits(this double left, double right)
        {
            return Math.Abs(left - right) < Digits8;
        }
        // ReSharper restore UnusedMember.Global
    }
}