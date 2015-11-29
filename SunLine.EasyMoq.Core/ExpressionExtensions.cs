using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SunLine.EasyMoq.Core
{
	internal static class ExpressionExtensions
	{
		public static LambdaExpression ToLambda(this Expression expression)
		{
			Guard.NotNull(() => expression, expression);

			LambdaExpression lambda = expression as LambdaExpression;
			if (lambda == null)
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
					"Unsupported expression: {0}", expression));

			var convert = lambda.Body as UnaryExpression;
			if (convert != null && convert.NodeType == ExpressionType.Convert)
				lambda = Expression.Lambda(convert.Operand, lambda.Parameters.ToArray());

			return lambda;
		}

		public static MethodCallExpression ToMethodCall(this LambdaExpression expression)
		{
			Guard.NotNull(() => expression, expression);

			var methodCall = expression.Body as MethodCallExpression;
			if (methodCall == null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					"Expression is not a method invocation: {0}",
					expression.ToString()));
			}

			return methodCall;
		}

		public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
		{
			var prop = expression.Body as MemberExpression;
			if (prop != null)
			{
				var info = prop.Member as PropertyInfo;
				if (info != null)
				{
					return info;
				}
			}

			throw new ArgumentException(string.Format(
				CultureInfo.CurrentCulture,
				"Expression is not a property access: {0}",
				expression.ToString()));
		}

		internal static CallInfo GetCallInfo(this LambdaExpression expression)
		{
			Guard.NotNull(() => expression, expression);

			var methodCall = expression.ToMethodCall();
			return new CallInfo
			{
				Method = methodCall.Method,
				Arguments = methodCall.Arguments,
				Object = methodCall.Object
			};
		}
	}
}