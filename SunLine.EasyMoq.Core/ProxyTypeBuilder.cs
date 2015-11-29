using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace SunLine.EasyMoq.Core
{
    public class ProxyTypeBuilder
    {
        private readonly TypeBuilder _typeBuilder;
        private readonly Type _mockInterface;
        private readonly IList<MethodInfo> _implementedMethods;
        private readonly Interceptor _interceptor;
        private const string _interceptorFieldName = "_interceptor";
        
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
            AddMethodImplementation(methodInfo, (mi, mb) => { EmitInvokeMethod<TResult>(mi, mb, returnValue); });
        }
                
        public void MockMethod(string name, Type[] parameters, Exception exception)
        {
            MethodInfo methodInfo = GetMethodInfo(name, parameters);
            AddMethodImplementation(methodInfo, (mi, mb) => { EmitInvokeMethodThowException(mi, mb, exception); });
        }
                
        public void MockNotImplementedMethods()
        {
            foreach (MethodInfo methodInfo in _mockInterface.GetRuntimeMethods()) 
            { 
                if(!_implementedMethods.Contains(methodInfo))
                {
                    AddMethodImplementation(methodInfo, (mi, mb) => { EmitInvokeMethod(mi, mb); });
                } 
            } 
        }

        private void AddConstructor()
        {
            ConstructorBuilder constructor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, 
                new Type[] { typeof(Interceptor) });
            
            EmitConstructor(constructor);
        }

        private void AddMethodImplementation(MethodInfo methodInfo, Action<MethodInfo, MethodBuilder> emitInvokeMethodAction)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);
            
            MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodInfo.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            emitInvokeMethodAction.Invoke(methodInfo, methodBuilder);
            
            _typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            _implementedMethods.Add(methodInfo);
        }
        
        private void EmitInvokeMethod(MethodInfo methodInfo, MethodBuilder methodBuilder)
        {
            var methodInformation = AddMethodInformationToInterceptor(methodInfo);
            FieldInfo interceptorFieldInfo = _typeBuilder.GetDeclaredField(_interceptorFieldName);
            
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.CallMethodWasExecuted(interceptorFieldInfo, methodInformation.Hash);
            
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
                
        private void EmitInvokeMethodThowException(MethodInfo methodInfo, MethodBuilder methodBuilder, Exception exception)
        {            
            var methodInformation = AddMethodInformationToInterceptor(methodInfo, exception);
            FieldInfo interceptorFieldInfo = _typeBuilder.GetDeclaredField(_interceptorFieldName);
            
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.CallMethodWasExecuted(interceptorFieldInfo, methodInformation.Hash);
            ilGenerator.CallGetReturnObject(interceptorFieldInfo, methodInformation.Hash);
            ilGenerator.Emit(OpCodes.Throw);
        }
        
        private void EmitInvokeMethod<TResult>(MethodInfo methodInfo, MethodBuilder methodBuilder, TResult returnValue)
        {
            var methodInformation = AddMethodInformationToInterceptor(methodInfo, returnValue);
            FieldInfo interceptorFieldInfo = _typeBuilder.GetDeclaredField(_interceptorFieldName);
            
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.CallMethodWasExecuted(interceptorFieldInfo, methodInformation.Hash);
            
            Type type = methodBuilder.ReturnType;
            if (methodBuilder.ReturnType != typeof(void))
            {
                if (methodBuilder.ReturnType.GetTypeInfo().IsValueType)
                {
                    ilGenerator.CallGetReturnObject(interceptorFieldInfo, methodInformation.Hash);
                    ilGenerator.UnboxObject(typeof(object), methodBuilder.ReturnType);
                }
                else
                {
                    ilGenerator.CallGetReturnObject(interceptorFieldInfo, methodInformation.Hash);
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

        private MethodInformation AddMethodInformationToInterceptor(MethodInfo methodInfo)
        {
            return AddMethodInformationToInterceptor(methodInfo, null);
        }
        
        private MethodInformation AddMethodInformationToInterceptor(MethodInfo methodInfo, object returnValue)
        {
            Type[] parameters = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            var methodInformation = new MethodInformation(methodInfo.Name, methodInfo.ReturnType, parameters, returnValue);
            _interceptor.AddMethodInformation(methodInformation);
            
            return methodInformation;
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