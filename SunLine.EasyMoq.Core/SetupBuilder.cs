using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SunLine.EasyMoq.Core
{
    public class SetupBuilder<TMock>
        where TMock : class
    {
        protected Expression Expression { get; set; }
        protected ProxyTypeBuilder ProxyTypeBuilder { get; }
        protected Exception ThrowsException { get; set; }
        protected string MethodName { get; set; }
        protected Type[] Parameters { get; set; }

        protected SetupBuilder(ProxyTypeBuilder proxyProxyTypeBuilder)
        {
            ProxyTypeBuilder = proxyProxyTypeBuilder;
        }

        public SetupBuilder(ProxyTypeBuilder proxyProxyTypeBuilder, Expression<Action<TMock>> expression)
            : this(proxyProxyTypeBuilder)
        {
            Expression = expression.Body;
        }

        public void Throws(Exception exception)
        {
            ThrowsException = exception;
            MockImplementation();
        }

        public void Throws<TException>() where TException : Exception
        {
            Exception exception = (TException)Activator.CreateInstance(typeof(TException));
            Throws(exception);
        }

        protected virtual void MockImplementation()
        {
            var methodCallExpressions = Expression as MethodCallExpression;
            if (methodCallExpressions != null)
            {
                MethodName = methodCallExpressions.Method.Name;
                Parameters = methodCallExpressions.Method.GetParameters().Select(x => x.ParameterType).ToArray();
            }

            var memberExpression = Expression as MemberExpression;
            var propertyInfo = memberExpression?.Member as PropertyInfo;
            if (propertyInfo != null)
            {
                MethodName = propertyInfo.GetMethod.Name;
                Parameters = new Type[] { };
            }

            if (string.IsNullOrWhiteSpace(MethodName))
            {
                throw new NotSupportedException($"Expression {Expression} is not supported.");
            }

            if (ThrowsException != null)
            {
                ProxyTypeBuilder.MockMethod(MethodName, Parameters, ThrowsException);
            }
        }
    }

    public class SetupBuilder<TMock, TResult> : SetupBuilder<TMock>
        where TMock : class
    {
        protected Func<TResult> ReturnExpression { get; private set; }

        public Type ReturnType
        {
            get; private set;
        }

        public TResult ReturnValue
        {
            get; private set;
        }

        public SetupBuilder(ProxyTypeBuilder proxyProxyTypeBuilder, Expression<Func<TMock, TResult>> expression)
            : base(proxyProxyTypeBuilder)
        {
            Expression = expression.Body;
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

        protected override void MockImplementation()
        {
            base.MockImplementation();

            if (ReturnExpression != null)
            {
                ProxyTypeBuilder.MockMethod(MethodName, Parameters, ReturnValue);
            }
        }

        private void SetReturnDelegate(Func<TResult> returnExpression)
        {
            if (returnExpression == null)
            {
                ReturnExpression = () => default(TResult);
            }
            else
            {
                ReturnExpression = returnExpression;
            }

            var returnValue = ReturnExpression.Invoke();
            ReturnType = returnValue.GetType();
            ReturnValue = returnValue;
        }
    }
}