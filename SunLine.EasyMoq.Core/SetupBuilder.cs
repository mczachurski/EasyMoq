using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace SunLine.EasyMoq.Core
{
    public class SetupBuilder<TMock, TResult> where TMock : class
	{
        private readonly Mock<TMock> _mock;
        private readonly Expression<Func<TMock, TResult>> _expression;
        
        private Func<TResult> _returnExpression;
        
        public SetupBuilder(Mock<TMock> mock, Expression<Func<TMock, TResult>> expression)
        {
            _mock = mock;
            _expression = expression;
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