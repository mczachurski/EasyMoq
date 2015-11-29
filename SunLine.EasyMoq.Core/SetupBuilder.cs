using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SunLine.EasyMoq.Core
{
	public class SetupBuilder<TMock> 
		where TMock : class
	{
		protected Expression _expression;
		protected readonly ProxyTypeBuilder _proxyTypeBuilder;
		protected Exception _throwsException;
		protected string _methodName;
		protected Type[] _parameters;
		
		protected SetupBuilder(ProxyTypeBuilder proxyTypeBuilder)
		{
			_proxyTypeBuilder = proxyTypeBuilder;
		}
		
		public SetupBuilder(ProxyTypeBuilder proxyTypeBuilder, Expression<Action<TMock>> expression)
			: this(proxyTypeBuilder)
		{
			_expression = expression.Body;
		}
		
		public void Throws(Exception exception)
		{
			_throwsException = exception;
			MockImplementation();
		}
		
		public void Throws<TException>() where TException : Exception
		{
			Exception exception = (TException) Activator.CreateInstance(typeof(TException));
			Throws(exception);
		}
		
		protected virtual void MockImplementation()
		{			
			var methodCallExpressions = _expression as MethodCallExpression;			
			if(methodCallExpressions != null)
			{
				_methodName = methodCallExpressions.Method.Name;
				_parameters = methodCallExpressions.Method.GetParameters().Select(x => x.ParameterType).ToArray();
			}
			
			var memberExpression = _expression as MemberExpression;
		    var propertyInfo = memberExpression?.Member as PropertyInfo;
		    if (propertyInfo != null)
		    {
		        _methodName = propertyInfo.GetMethod.Name;
                _parameters = new Type[] { };
		    }
			
			if(string.IsNullOrWhiteSpace(_methodName))
			{
				throw new NotSupportedException($"Expression {_expression} is not supported.");	
			}
			
			if(_throwsException != null)
			{
				_proxyTypeBuilder.MockMethod(_methodName, _parameters, _throwsException);
			}
		}
	}
	
    public class SetupBuilder<TMock, TResult> : SetupBuilder<TMock> 
		where TMock : class
	{
        private Func<TResult> _returnExpression; 
		private Delegate valueDelegate = (Func<TResult>)(() => default(TResult));
		
		public Type ReturnType
		{
			get; private set;
		}
		
		public TResult ReturnValue
		{
			get; private set;
		}
		
        public SetupBuilder(ProxyTypeBuilder proxyTypeBuilder, Expression<Func<TMock, TResult>> expression)
			: base(proxyTypeBuilder)
        {
            _expression = expression.Body;
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
		
		public void Returns<T1>(Func<T1, TResult> valueFunction)
		{
			
		}
		
		public void Returns<T1, T2>(Func<T1, T2, TResult> valueFunction)
		{
			
		}
		
		public void Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueFunction)
		{
			
		}
		
		public void Returns<T1, T2, T3,T4>(Func<T1, T2, T3, T4, TResult> valueFunction)
		{
			
		}
		
		public void Returns<T1, T2, T3,T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueFunction)
		{
			
		}
		
		public void Returns<T1, T2, T3,T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction)
		{
			
		}
		
		public void Returns<T1, T2, T3,T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction)
		{
			
		}
		
		protected override void MockImplementation()
		{
			base.MockImplementation();
			
			if(_returnExpression != null)
			{
				_proxyTypeBuilder.MockMethod<TResult>(_methodName, _parameters, ReturnValue);
			}
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