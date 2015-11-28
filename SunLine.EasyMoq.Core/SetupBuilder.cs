using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SunLine.EasyMoq.Core
{
    public class SetupBuilder<TMock, TResult> where TMock : class
	{
        private readonly ProxyTypeBuilder _proxyTypeBuilder;
        private readonly Expression<Func<TMock, TResult>> _expression;
        private Func<TResult> _returnExpression; 
		
		public Type ReturnType
		{
			get; private set;
		}
		
		public TResult ReturnValue
		{
			get; private set;
		}
		
        public SetupBuilder(ProxyTypeBuilder proxyTypeBuilder, Expression<Func<TMock, TResult>> expression)
        {
            _proxyTypeBuilder = proxyTypeBuilder;
            _expression = expression;
        }
        
        public void Returns(Func<TResult> returnExpression)
		{
			SetReturnDelegate(returnExpression);
			MockImplementation();
		}

		public void Returns(TResult value)
		{
			Returns(() => value);
		}
        
		private void MockImplementation()
		{
			var methodCallExpressions = _expression.Body as MethodCallExpression;			
			if(methodCallExpressions != null)
			{
				string methodName = methodCallExpressions.Method.Name;
				Type[] methodParameters = methodCallExpressions.Method.GetParameters().Select(x => x.ParameterType).ToArray();	
				_proxyTypeBuilder.MockMethod<TResult>(methodName, methodParameters, ReturnValue);

			    return;
			}
			
			var memberExpression = _expression.Body as MemberExpression;
		    var propertyInfo = memberExpression?.Member as PropertyInfo;
		    if (propertyInfo != null)
		    {
		        string propertyName = propertyInfo.GetMethod.Name;
                _proxyTypeBuilder.MockMethod<TResult>(propertyName, new Type[] { }, ReturnValue);
                return;
		    }

		    throw new NotSupportedException($"Expression {_expression} is not supported.");
		}
		
        private void SetReturnDelegate(Func<TResult> returnExpression)
        {
            if (returnExpression == null)
			{
				_returnExpression = (Func<TResult>)(() => default(TResult));
			}
			else
			{
				_returnExpression = returnExpression;
			}
			
			var returnValue = _returnExpression.Invoke();
			ReturnType = returnValue.GetType();
			ReturnValue = returnValue;
        }
	}
}