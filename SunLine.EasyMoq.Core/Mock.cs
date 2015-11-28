using System;
using System.Linq.Expressions;

namespace SunLine.EasyMoq.Core
{
    public class Mock<TMock> where TMock : class
    {
        private TMock _object;
        
        private Type _objectType;
            
        private readonly ProxyTypeBuilder _proxyTypeBuilder;
            
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
            _proxyTypeBuilder = new ProxyTypeBuilder(typeof(TMock));
        }
    
        public SetupBuilder<TMock, TResult> Setup<TResult>(Expression<Func<TMock, TResult>> expression)
		{                    
			var setupBuilder = new SetupBuilder<TMock, TResult>(this._proxyTypeBuilder, expression);            
            return setupBuilder;
		}
    
        private void GenerateProxyObject()
        {          
            _proxyTypeBuilder.MockNotImplementedMethods();
            _objectType = _proxyTypeBuilder.CreateTypeInfo().AsType();
            _object = (TMock) Activator.CreateInstance(_objectType);
        }
    }
}
