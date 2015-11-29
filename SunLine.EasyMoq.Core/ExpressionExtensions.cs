using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace SunLine.EasyMoq.Core
{
	internal static class ExpressionExtensions
	{
		internal static MethodCallExpression ToMethodCall(this LambdaExpression expression)
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

		internal static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
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

			if(expression.Body is MethodCallExpression)
			{
				var methodCall = expression.ToMethodCall();
				return new CallInfo
				{
					Method = methodCall.Method,
					Arguments = methodCall.Arguments,
					Object = methodCall.Object
				};
			}
			
			if(expression.Body is MemberExpression)
			{
				var propertyCall = expression.ToPropertyInfo();
				return new CallInfo
				{
					Method = propertyCall.GetMethod
				};
			}
			
			throw new ArgumentException(string.Format(
				CultureInfo.CurrentCulture,
				"Expression is not supported: {0}",
				expression.ToString()));
		}
	}
}