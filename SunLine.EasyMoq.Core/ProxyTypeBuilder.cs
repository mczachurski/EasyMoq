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
        public static Dictionary<string, object> _returnedObjects = new Dictionary<string, object>();
        
        public ProxyTypeBuilder(Type mockInterface)
        {                 
            _implementedMethods = new List<MethodInfo>();
            _mockInterface = mockInterface;
            
            _typeBuilder = ProxyAssemblyBuilder.Instance.CreateTypeBuilder(mockInterface);
            ConstructorBuilder constructor = _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            
            _typeBuilder.AddInterfaceImplementation(mockInterface);
        }
        
        public TypeInfo CreateTypeInfo()
        {
            return _typeBuilder.CreateTypeInfo();
        }
        
        public void MockMethod<TResult>(string name, Type[] parameters, TResult returnValue)
        {
            MethodInfo methodInfo = GetMethodInfo(name, parameters);
            AddMethodImplementation<TResult>(methodInfo, returnValue);
        }
        
        public void MockNotImplementedMethods()
        {
            foreach (MethodInfo methodInfo in _mockInterface.GetRuntimeMethods()) 
            { 
                if(!_implementedMethods.Contains(methodInfo))
                {
                    AddMethodImplementation(methodInfo);
                } 
            } 
        }

        private void AddMethodImplementation(MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);
            
            MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodInfo.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            EmitInvokeMethod(methodBuilder);
            
            _typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            _implementedMethods.Add(methodInfo);
        }
        
        private void AddMethodImplementation<TResult>(MethodInfo methodInfo, TResult returnValue)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);
            
            MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodInfo.Name, 
                MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            EmitInvokeMethod<TResult>(methodBuilder, returnValue);
            
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
        
        public static object GetReturnedObject(string key)
        {
            if(_returnedObjects.ContainsKey(key))
            {
                return _returnedObjects[key];
            }
            
            return null;
        }
        
        private void EmitInvokeMethod<TResult>(MethodBuilder methodBuilder, TResult returnValue)
        {
            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            var keyGuid = Guid.NewGuid().ToString();
            _returnedObjects.Add(keyGuid, returnValue);
        
            Type type = methodBuilder.ReturnType;
            if (methodBuilder.ReturnType != typeof(void))
            {
                if (methodBuilder.ReturnType.GetTypeInfo().IsValueType)
                {
                    var valObj = ilGenerator.DeclareLocal(typeof(object));
                    
                    ilGenerator.Emit(OpCodes.Ldstr, keyGuid);
                    ilGenerator.Emit(OpCodes.Call, typeof(ProxyTypeBuilder).GetMethod("GetReturnedObject", new Type[] { typeof(string) }));
                    ilGenerator.Emit(OpCodes.Box, typeof(object));
                    ilGenerator.Emit(OpCodes.Stloc, valObj);
                    ilGenerator.Emit(OpCodes.Ldloc, valObj);
                    ilGenerator.Emit(OpCodes.Unbox_Any, methodBuilder.ReturnType);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldstr, keyGuid);
                    ilGenerator.Emit(OpCodes.Call, typeof(ProxyTypeBuilder).GetMethod("GetReturnedObject", new Type[] { typeof(string) }));
                }
            }
        
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