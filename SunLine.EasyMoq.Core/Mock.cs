using System;
using System.Linq.Expressions;

namespace SunLine.EasyMoq.Core
{
    public class Mock<TMock> where TMock : class
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
    
        private void GenerateProxyObject()
        {          
            _proxyTypeBuilder.MockNotImplementedMethods();
            _objectType = _proxyTypeBuilder.CreateTypeInfo().AsType();
            _object = (TMock) Activator.CreateInstance(_objectType, _interceptor);
        }
    }
}
