using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SunLine.EasyMoq.Core
{
    public class ProxyTypeBuilder
    {
        private readonly TypeBuilder _typeBuilder;
        private readonly Type _mockInterface;
        private readonly IList<MethodInfo> _implementedMethods;
        private readonly Interceptor _interceptor;
        private const string _interceptorFieldName = "_interceptor";
        private const string _getReturnedObjectMethodName = "GetReturnedObject";
        
        public ProxyTypeBuilder(Type mockInterface, Interceptor interceptor)
        {                 
            _implementedMethods = new List<MethodInfo>();
            _mockInterface = mockInterface;
            _interceptor = interceptor;
            
            _typeBuilder = ProxyAssemblyBuilder.Instance.CreateTypeBuilder(_mockInterface);
            _typeBuilder.AddInterfaceImplementation(_mockInterface);
            
            AddConstructor();
        }
        
        public TypeInfo CreateTypeInfo()
        {
            return _typeBuilder.CreateTypeInfo();
        }
        
        public void MockMethod<TResult>(string name, Type[] parameters, TResult returnValue)
        {
            MethodInfo methodInfo = GetMethodInfo(name, parameters);
            AddMethodImplementation(methodInfo, 
                (MethodBuilder methodBuilder) => { EmitInvokeMethod<TResult>(methodBuilder, returnValue); });
        }
                
        public void MockMethod(string name, Type[] parameters, Exception exception)
        {
            MethodInfo methodInfo = GetMethodInfo(name, parameters);
            AddMethodImplementation(methodInfo, 
                (MethodBuilder methodBuilder) => { EmitInvokeMethodThowException(methodBuilder, exception); });
        }
                
        public void MockNotImplementedMethods()
        {
            foreach (MethodInfo methodInfo in _mockInterface.GetRuntimeMethods()) 
            { 
                if(!_implementedMethods.Contains(methodInfo))
                {
                    AddMethodImplementation(methodInfo, 
                        (MethodBuilder methodBuilder) => { EmitInvokeMethod(methodBuilder); });
                } 
            } 
        }

        private void AddConstructor()
        {
            ConstructorBuilder constructor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, 
                new Type[] { typeof(Interceptor) });
            
            EmitConstructor(constructor);
        }

        private void AddMethodImplementation(MethodInfo methodInfo, Action<MethodBuilder> emitInvokeMethodAction)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);
            
            MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodInfo.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            emitInvokeMethodAction.Invoke(methodBuilder);
            
            _typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            _implementedMethods.Add(methodInfo);
        }
        
        private void EmitInvokeMethod(MethodBuilder methodBuilder)
        {
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
        
            Type type = methodBuilder.ReturnType;
            if (methodBuilder.ReturnType != typeof(void))
            {                
                if (methodBuilder.ReturnType.GetTypeInfo().IsValueType)
                {
                    LocalBuilder val = ilGenerator.DeclareLocal(methodBuilder.ReturnType);
                    ilGenerator.Emit(ConvertTypeToOpCode(methodBuilder.ReturnType), 0);
                    ilGenerator.Emit(OpCodes.Stloc, val);
                    ilGenerator.Emit(OpCodes.Ldloc, val);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                }
            }
        
            ilGenerator.Emit(OpCodes.Ret);
        }
                
        private void EmitInvokeMethodThowException(MethodBuilder methodBuilder, Exception exception)
        {
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            var keyGuid = Guid.NewGuid().ToString();
            _interceptor.AddReturnedObject(keyGuid, exception);
            FieldInfo interceptorFieldInfo = _typeBuilder.GetDeclaredField(_interceptorFieldName);
        
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldInfo);
            ilGenerator.Emit(OpCodes.Ldstr, keyGuid);
            ilGenerator.Emit(OpCodes.Call, typeof(Interceptor).GetMethod(_getReturnedObjectMethodName, new Type[] { typeof(string) }));
            ilGenerator.Emit(OpCodes.Throw);
        }
        
        private void EmitInvokeMethod<TResult>(MethodBuilder methodBuilder, TResult returnValue)
        {
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            var keyGuid = Guid.NewGuid().ToString();
            _interceptor.AddReturnedObject(keyGuid, returnValue);
            FieldInfo interceptorFieldInfo = _typeBuilder.GetDeclaredField(_interceptorFieldName);
        
            Type type = methodBuilder.ReturnType;
            if (methodBuilder.ReturnType != typeof(void))
            {
                if (methodBuilder.ReturnType.GetTypeInfo().IsValueType)
                {
                    var valObj = ilGenerator.DeclareLocal(typeof(object));
                    
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldInfo);
                    ilGenerator.Emit(OpCodes.Ldstr, keyGuid);
                    ilGenerator.Emit(OpCodes.Call, typeof(Interceptor).GetMethod(_getReturnedObjectMethodName, new Type[] { typeof(string) }));
                    ilGenerator.Emit(OpCodes.Box, typeof(object));
                    ilGenerator.Emit(OpCodes.Stloc, valObj);
                    ilGenerator.Emit(OpCodes.Ldloc, valObj);
                    ilGenerator.Emit(OpCodes.Unbox_Any, methodBuilder.ReturnType);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldInfo);
                    ilGenerator.Emit(OpCodes.Ldstr, keyGuid);
                    ilGenerator.Emit(OpCodes.Call, typeof(Interceptor).GetMethod(_getReturnedObjectMethodName, new Type[] { typeof(string) }));
                }
            }
        
            ilGenerator.Emit(OpCodes.Ret);
        }
        
        private void EmitConstructor(ConstructorBuilder constructorBuilder)
        {
            FieldBuilder fieldBuilder = _typeBuilder.DefineField(_interceptorFieldName, typeof(Interceptor), FieldAttributes.Private);

            ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);
        }
        
        private MethodInfo GetMethodInfo(string name, Type[] parameters)
        {
            MethodInfo methodInfo = _mockInterface.GetRuntimeMethod(name, parameters);
            return methodInfo;
        }

		private OpCode ConvertTypeToOpCode(Type type)
		{
			if (type.GetTypeInfo().IsEnum)
			{
                return ConvertTypeToOpCode(typeof(Int32));
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
            
        private Type[] ParamTypes(ParameterInfo[] parms, bool noByRef)
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