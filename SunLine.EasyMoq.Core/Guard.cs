using System;
using System.Linq.Expressions;

namespace SunLine.EasyMoq.Core
{
	internal static class Guard
	{
		public static void NotNull<T>(Expression<Func<T>> reference, T value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(GetParameterName(reference));
			}
		}

		public static void NotNullOrEmpty(Expression<Func<string>> reference, string value)
		{
			NotNull<string>(reference, value);
			if (value.Length == 0)
			{
				throw new ArgumentException("Value cannot be an empty string.", GetParameterName(reference));
			}
		}

		public static void NotOutOfRangeInclusive<T>(Expression<Func<T>> reference, T value, T from, T to)
				where T : IComparable
		{
			if (value != null && (value.CompareTo(from) < 0 || value.CompareTo(to) > 0))
			{
				throw new ArgumentOutOfRangeException(GetParameterName(reference));
			}
		}

		public static void NotOutOfRangeExclusive<T>(Expression<Func<T>> reference, T value, T from, T to)
				where T : IComparable
		{
			if (value != null && (value.CompareTo(from) <= 0 || value.CompareTo(to) >= 0))
			{
				throw new ArgumentOutOfRangeException(GetParameterName(reference));
			}
		}

		private static string GetParameterName(LambdaExpression reference)
		{
			var member = (MemberExpression)reference.Body;
			return member.Member.Name;
		}
	}
}