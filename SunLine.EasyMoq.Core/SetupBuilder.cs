using System;
using System.Linq.Expressions;

namespace SunLine.EasyMoq.Core
{
    public interface ISetupBuilder
	{
		string FullMethodName { get; }
		string MethodName { get; }
	}
	
    public class SetupBuilder<TMock, TResult> : ISetupBuilder where TMock : class
	{
        private readonly Mock<TMock> _mock;
        private readonly Expression<Func<TMock, TResult>> _expression;
        private Func<TResult> _returnExpression;
		
		public string MethodName
		{
			get; private set;
		}
		
		public string FullMethodName
		{
			get; private set;
		}
		
        public SetupBuilder(Mock<TMock> mock, Expression<Func<TMock, TResult>> expression)
        {
            _mock = mock;
            _expression = expression;
			
			var call = (MethodCallExpression)_expression.Body;
			FullMethodName = call.Method.ToString();
			MethodName = call.Method.Name;
        }
        
        public void Returns(Func<TResult> returnExpression)
		{
			SetReturnDelegate(returnExpression);
		}

		public void Returns(TResult value)
		{
			Returns(() => value);
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
        }
	}
}