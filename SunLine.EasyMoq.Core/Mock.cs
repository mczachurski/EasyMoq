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
        
        public SetupBuilder<TMock, TResult> Returns(Func<TResult> returnExpression)
		{
			SetReturnDelegate(returnExpression);
            return this;
		}

		public SetupBuilder<TMock, TResult> Returns(TResult value)
		{
			Returns(() => value);
            return this;
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
    
    public class Mock<TMock> where TMock : class
    {
        private TMock _object;
        
        private Type _objectType;
        
        public TMock Object {
            get {
                return _object;
            }
        }
        
        public Type ObjectType
        {
            get {
                return _objectType;
            }
        }
        
        public Mock()
        {
            Type mockInterface = typeof(TMock);
                 
            var assemblyName = new AssemblyName("SunLine.EasyMoq.ProxyAssembly");
            assemblyName.Version = new Version(1, 0, 0);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("SunLine.EasyMoq.ProxyAssembly");
            var typeBuilder = moduleBuilder.DefineType(mockInterface.Name + "Proxy", TypeAttributes.Public);
            
            ConstructorBuilder constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            
            typeBuilder.AddInterfaceImplementation(mockInterface);
            
            foreach (MethodInfo methodInfo in mockInterface.GetRuntimeMethods()) 
            { 
                AddMethodImpl(typeBuilder, methodInfo); 
            } 
          
            _objectType = typeBuilder.CreateTypeInfo().AsType();
            _object = (TMock) Activator.CreateInstance(_objectType);
        }
        
        public SetupBuilder<TMock, TResult> Setup<TResult>(Expression<Func<TMock, TResult>> expression)
		{
			return new SetupBuilder<TMock, TResult>(this, expression);
		}
        
        private void AddMethodImpl(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);
            
            MethodBuilder mdb = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            ILGenerator il = mdb.GetILGenerator();
            il.Emit(OpCodes.Ret);
            
            typeBuilder.DefineMethodOverride(mdb, methodInfo);
        }
            
        private static Type[] ParamTypes(ParameterInfo[] parms, bool noByRef)
        {
            Type[] types = new Type[parms.Length];
            for (int i = 0; i < parms.Length; i++)
            {
                types[i] = parms[i].ParameterType;
                if (noByRef && types[i].IsByRef)
                {
                    types[i] = types[i].GetElementType();
                }
            }
            return types;
        }
    }
}
