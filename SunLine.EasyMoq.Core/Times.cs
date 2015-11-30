using System;
using System.Globalization;

namespace SunLine.EasyMoq.Core
{
    public struct Times
    {
        private readonly Func<int, bool> _evaluator;
        private readonly string _messageFormat;
        private readonly int _from;
        private readonly int _to;

        private Times(Func<int, bool> evaluator, int from, int to, string messageFormat)
        {
            _evaluator = evaluator;
            _from = from;
            _to = to;
            _messageFormat = messageFormat;
        }

        public static Times AtLeast(int callCount)
        {
            Guard.NotOutOfRangeInclusive(() => callCount, callCount, 1, int.MaxValue);

            return new Times(c => c >= callCount, callCount, int.MaxValue, "Expected invocation on the mock at least once, but was never performed: {1}");
        }

        public static Times AtLeastOnce()
        {
            return new Times(c => c >= 1, 1, int.MaxValue, "Expected invocation on the mock at least once, but was never performed: {1}");
        }

        public static Times AtMost(int callCount)
        {
            Guard.NotOutOfRangeInclusive(() => callCount, callCount, 0, int.MaxValue);

            return new Times(c => c >= 0 && c <= callCount, 0, callCount, "Expected invocation on the mock at most {3} times, but was {4} times: {1}");
        }

        public static Times AtMostOnce()
        {
            return new Times(c => c >= 0 && c <= 1, 0, 1, "Expected invocation on the mock at most once, but was {4} times: {1}");
        }

        public static Times Between(int callCountFrom, int callCountTo, Range rangeKind)
        {
            if (rangeKind == Range.Exclusive)
            {
                Guard.NotOutOfRangeExclusive(() => callCountFrom, callCountFrom, 0, callCountTo);
                if (callCountTo - callCountFrom == 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(callCountTo));
                }

                return new Times(
                    c => c > callCountFrom && c < callCountTo,
                    callCountFrom,
                    callCountTo,
                    "Expected invocation on the mock between {2} and {3} times (Exclusive), but was {4} times: {1}");
            }

            Guard.NotOutOfRangeInclusive(() => callCountFrom, callCountFrom, 0, callCountTo);
            return new Times(
                c => c >= callCountFrom && c <= callCountTo,
                callCountFrom,
                callCountTo,
                "Expected invocation on the mock between {2} and {3} times (Inclusive), but was {4} times: {1}");
        }

        public static Times Exactly(int callCount)
        {
            Guard.NotOutOfRangeInclusive(() => callCount, callCount, 0, int.MaxValue);

            return new Times(c => c == callCount, callCount, callCount, "Expected invocation on the mock exactly {2} times, but was {4} times: {1}");
        }

        public static Times Never()
        {
            return new Times(c => c == 0, 0, 0, "Expected invocation on the mock should never have been performed, but was {4} times: {1}");
        }

        public static Times Once()
        {
            return new Times(c => c == 1, 1, 1, "Expected invocation on the mock once, but was {4} times: {1}");
        }

        public override bool Equals(object obj)
        {
            if (obj is Times)
            {
                var other = (Times)obj;
                return _from == other._from && _to == other._to;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _from.GetHashCode() ^ _to.GetHashCode();
        }

        public static bool operator ==(Times left, Times right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Times left, Times right)
        {
            return !left.Equals(right);
        }

        internal string GetExceptionMessage(string failMessage, string expression, int callCount)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                _messageFormat,
                failMessage,
                expression,
                _from,
                _to,
                callCount);
        }

        public bool Verify(int callCount)
        {
            return _evaluator(callCount);
        }
    }
}