using System;
using System.Linq;
using System.Linq.Expressions;

namespace SunLine.EasyMoq.Core
{
    public interface ISetupBuilder
	{
		string MethodName { get; }
		Type[] MethodParameters { get; }
		Type ReturnType { get; }
	}
	
    public class SetupBuilder<TMock, TResult> : ISetupBuilder where TMock : class
	{
        private readonly ProxyTypeBuilder _proxyTypeBuilder;
        private readonly Expression<Func<TMock, TResult>> _expression;
        private Func<TResult> _returnExpression; 
		
		public string MethodName
		{
			get; private set;
		}
		
		public Type[] MethodParameters
		{
			get; private set;
		}
		
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
			
			var call = (MethodCallExpression)_expression.Body;
			MethodName = call.Method.Name;
			MethodParameters = call.Method.GetParameters().Select(x => x.GetType()).ToArray();
        }
        
        public void Returns(Func<TResult> returnExpression)
		{
			SetReturnDelegate(returnExpression);
			MockMethod();
		}

		public void Returns(TResult value)
		{
			Returns(() => value);
		}
        
		private void MockMethod()
		{
			_proxyTypeBuilder.MockMethod<TResult>(MethodName, MethodParameters, ReturnValue);
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