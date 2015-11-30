using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SunLine.EasyMoq.Core
{
    public static class It
    {
        public static TValue IsAny<TValue>()
        {
            return default(TValue);
        }

        public static TValue IsNotNull<TValue>()
        {
            return default(TValue);
        }

        public static TValue Is<TValue>(Expression<Func<TValue, bool>> match)
        {
            return default(TValue);
        }

        public static TValue IsInRange<TValue>(TValue from, TValue to, Range rangeKind) where TValue : IComparable
        {
            return default(TValue);
        }

        public static TValue IsIn<TValue>(IEnumerable<TValue> items)
        {
            return default(TValue);
        }

        public static TValue IsIn<TValue>(params TValue[] items)
        {
            return default(TValue);
        }

        public static TValue IsNotIn<TValue>(IEnumerable<TValue> items)
        {
            return default(TValue);
        }

        public static TValue IsNotIn<TValue>(params TValue[] items)
        {
            return default(TValue);
        }
    }
}