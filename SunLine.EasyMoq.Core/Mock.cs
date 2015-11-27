using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Collections.Generic;

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
            
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            EmitInvokeMethod(methodBuilder);
            
            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
        }
        
        private void WriteILForMethod( MethodBuilder builder)
		{
			ILGenerator ilGenerator = builder.GetILGenerator();


			if (builder.ReturnType != typeof(void))
			{
				ilGenerator.DeclareLocal(builder.ReturnType);
			}



			if (builder.ReturnType != typeof(void))
			{
				if (!builder.ReturnType.GetTypeInfo().IsValueType)
				{
					ilGenerator.Emit(OpCodes.Castclass, builder.ReturnType);
				}
				else
				{
					ilGenerator.Emit(OpCodes.Unbox, builder.ReturnType);
					ilGenerator.Emit(ConvertTypeToOpCode(builder.ReturnType));
				}

				ilGenerator.Emit(OpCodes.Stloc, 1);

				Label label = ilGenerator.DefineLabel();
				ilGenerator.Emit(OpCodes.Br_S, label);
				ilGenerator.MarkLabel(label);
				ilGenerator.Emit(OpCodes.Ldloc, 1);
			}
			else
			{
				ilGenerator.Emit(OpCodes.Pop);
			}


			ilGenerator.Emit(OpCodes.Ret);
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
                    ConstructorInfo ci = typeof(Int32).GetConstructor(System.Type.EmptyTypes);
                    ilGenerator.Emit(OpCodes.Newobj, ci); // Store "5" ...
                    ilGenerator.Emit(OpCodes.Stloc, a);  // ... in "a".
                    ilGenerator.Emit(OpCodes.Ldloc, a);  // Load "a" ...
   
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
				return OpCodes.Ldind_I4;
			}
			else if ( type.Equals( typeof(Int16) ) )
			{
				return OpCodes.Ldind_I2;
			}
			else if ( type.Equals( typeof(Int64) ) )
			{
				return OpCodes.Ldind_I8;
			}
			else if ( type.Equals( typeof(Single) ) )
			{
				return OpCodes.Ldind_R4;
			}
			else if ( type.Equals( typeof(Double) ) )
			{
				return OpCodes.Ldind_R8;
			}
			else if ( type.Equals( typeof(UInt16) ) )
			{
				return OpCodes.Ldind_U2;
			}
			else if ( type.Equals( typeof(UInt32) ) )
			{
				return OpCodes.Ldind_U4;
			}
			else if ( type.Equals( typeof(Boolean) ) )
			{
				return OpCodes.Ldind_I4;
			}
			else
			{
				throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
			}
		}
            
        object GetDefaultValue(Type t)
        {
            if (t.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(t);
            }
        
            return null;
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
