using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;

namespace SunLine.EasyMoq.Core
{
    public class Mock<TMock>  
        where TMock : class
    {
        private TMock _object;
        private Type _objectType;
        private readonly ProxyTypeBuilder _proxyTypeBuilder;
        private readonly Interceptor _interceptor;
            
        public TMock Object {
            get {
                if(_object == null)
                {
                    GenerateProxyObject();
                }
                
                return _object;
            }
        }
        
        public Type ObjectType
        {
            get {
                if(_objectType == null)
                {
                    GenerateProxyObject();
                }
                
                return _objectType;
            }
        }
        
        public Mock()
        {
            _interceptor = new Interceptor();
            _proxyTypeBuilder = new ProxyTypeBuilder(typeof(TMock), _interceptor);
        }
    
        public SetupBuilder<TMock, TResult> Setup<TResult>(Expression<Func<TMock, TResult>> expression)
		{                    
			var setupBuilder = new SetupBuilder<TMock, TResult>(_proxyTypeBuilder, expression);            
            return setupBuilder;
		}
    
        public SetupBuilder<TMock> Setup(Expression<Action<TMock>> expression)
        {
            return new SetupBuilder<TMock>(_proxyTypeBuilder, expression);
        }
    
		public void Verify(Expression<Action<TMock>> expression)
		{
			Verify(expression, Times.AtLeastOnce(), null);
		}

		public void Verify(Expression<Action<TMock>> expression, Times times)
		{
			Verify(expression, times, null);
		}

		public void Verify(Expression<Action<TMock>> expression, Func<Times> times)
		{
			Verify(expression, times());
		}

		public void Verify(Expression<Action<TMock>> expression, string failMessage)
		{
			Verify(expression, Times.AtLeastOnce(), failMessage);
		}

		public void Verify(Expression<Action<TMock>> expression, Func<Times> times, string failMessage)
		{
			Verify(expression, times(), failMessage);
		}

		public void Verify<TResult>(Expression<Func<TMock, TResult>> expression)
		{
			Verify(expression, Times.AtLeastOnce(), null);
		}

		public void Verify<TResult>(Expression<Func<TMock, TResult>> expression, Times times)
		{
			Verify(expression, times, null);
		}

		public void Verify<TResult>(Expression<Func<TMock, TResult>> expression, Func<Times> times)
		{
			Verify(expression, times(), null);
		}

		public void Verify<TResult>(Expression<Func<TMock, TResult>> expression, string failMessage)
		{
			Verify(expression, Times.AtLeastOnce(), failMessage);
		}

		public void Verify<TResult>(Expression<Func<TMock, TResult>> expression, Times times, string failMessage)
		{
			Verify(expression, times, failMessage);
		}
    
		private void Verify(Expression<Action<TMock>> expression, Times times, string failMessage)
		{
			Guard.NotNull(() => times, times);

			var callInfo = expression.GetCallInfo();
			ThrowIfVerifyNonVirtual(expression, callInfo.Method);
			
			VerifyCalls(callInfo, expression, times);
		}

		internal void VerifyGet<T, TProperty>(Mock<T> mock, Expression<Func<T, TProperty>> expression, Times times, string failMessage)
			where T : class
		{
			var method = expression.ToPropertyInfo().GetGetMethod(true);
			ThrowIfVerifyNonVirtual(expression, method);

			var callInfo = new CallInfo { Method = method };
			VerifyCalls(callInfo, expression, times);
		}
		
		private void VerifyCalls(CallInfo callInfo, Expression expression, Times times)
		{
            var methodInformation = _interceptor.GetMethodInformation(callInfo);
            if (!times.Verify(methodInformation.NumberOfCalls))
            {
                ThrowVerifyException(expression, times, methodInformation.NumberOfCalls);
			}
		}
		
		private static void ThrowVerifyException(Expression expression, Times times, int callCount)
		{
			var message = times.GetExceptionMessage("!!!Expected exception!!!", expression.ToString(), callCount);
			throw new MockException(MockException.ExceptionReason.VerificationFailed, message);
		}
		
		private static void ThrowIfVerifyNonVirtual(Expression verify, MethodInfo method)
		{
			if (!CanOverride(method))
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					"Invalid verify on a non-virtual (overridable in VB) member: {0}",
					verify.ToString()));
			}
		}
		
		public static bool CanOverride(MethodInfo method)
		{
			return method.IsVirtual && !method.IsFinal && !method.IsPrivate;
		}
	
        private void GenerateProxyObject()
        {          
            _proxyTypeBuilder.MockNotImplementedMethods();
            _objectType = _proxyTypeBuilder.CreateTypeInfo().AsType();
            _object = (TMock) Activator.CreateInstance(_objectType, _interceptor);
        }
    }
}
