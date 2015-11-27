using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SunLine.EasyMoq.Core
{
    public class Mock<TMock> where TMock : class
    {
        private TMock _object;
        
        private Type _objectType;
            
        private readonly IList<ISetupBuilder> _setupBuilders;
            
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
            _setupBuilders = new List<ISetupBuilder>();
        }
        
        private void GenerateProxyObject()
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
			var setupBuilder = new SetupBuilder<TMock, TResult>(this, expression);
            _setupBuilders.Add(setupBuilder);
            
            return setupBuilder;
		}
        
        private void AddMethodImpl(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);
            
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            EmitInvokeMethod(methodBuilder);
            
            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
        }
            
        private void EmitInvokeMethod(MethodBuilder methodBuilder)
        {
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();

            Type type = methodBuilder.ReturnType;
            if (methodBuilder.ReturnType != typeof(void))
            {
                ilGenerator.DeclareLocal(methodBuilder.ReturnType);
                
                if (methodBuilder.ReturnType.GetTypeInfo().IsValueType)
                {
                    LocalBuilder a = ilGenerator.DeclareLocal(methodBuilder.ReturnType);
                    ilGenerator.Emit(ConvertTypeToOpCode(methodBuilder.ReturnType), 0);
                    ilGenerator.Emit(OpCodes.Stloc, a);
                    ilGenerator.Emit(OpCodes.Ldloc, a);
   
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }
            }

            ilGenerator.Emit(OpCodes.Ret);
        }

		private static OpCode ConvertTypeToOpCode( Type type )
		{
			if (type.GetTypeInfo().IsEnum)
			{
                /*
				System.Enum baseType = (System.Enum) Activator.CreateInstance( type );
				TypeCode code = baseType.GetTypeCode();
				
				switch(code)
				{
					case TypeCode.Byte:
						type = typeof(Byte);
						break;
					case TypeCode.Int16:
						type = typeof(Int16);
						break;
					case TypeCode.Int32:
						type = typeof(Int32);
						break;
					case TypeCode.Int64:
						type = typeof(Int64);
						break;
				}

				return ConvertTypeToOpCode( type );
                */
                
                throw new NotImplementedException("Enums are not supported yet.");
			}

			if ( type.Equals( typeof(Int32) ) )
			{
				return OpCodes.Ldc_I4;
			}
			else if ( type.Equals( typeof(Int16) ) )
			{
				return OpCodes.Ldc_I4;
			}
			else if ( type.Equals( typeof(Int64) ) )
			{
				return OpCodes.Ldc_I8;
			}
			else if ( type.Equals( typeof(Single) ) )
			{
				return OpCodes.Ldc_R4;
			}
			else if ( type.Equals( typeof(Double) ) )
			{
				return OpCodes.Ldc_R8;
			}
			else if ( type.Equals( typeof(UInt16) ) )
			{
				return OpCodes.Ldc_I4;
			}
			else if ( type.Equals( typeof(UInt32) ) )
			{
				return OpCodes.Ldc_I4;
			}
			else if ( type.Equals( typeof(Boolean) ) )
			{
				return OpCodes.Ldc_I4;
			}
			else
			{
				throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
			}
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
