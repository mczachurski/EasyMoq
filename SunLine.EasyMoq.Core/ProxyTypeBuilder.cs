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
        private FieldBuilder _interceptorFieldBuilder;

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
            AddMethodImplementation(methodInfo, (mi, mb) => { EmitInvokeMethod(mi, mb, returnValue); });
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
                if (!_implementedMethods.Contains(methodInfo))
                {
                    AddMethodImplementation(methodInfo, EmitInvokeMethod);
                }
            }
        }

        private void AddConstructor()
        {
            ConstructorBuilder constructor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                new[] { typeof(Interceptor) });

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

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.CallMethodWasExecuted(_interceptorFieldBuilder, methodInformation.Hash);

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

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.CallMethodWasExecuted(_interceptorFieldBuilder, methodInformation.Hash);
            ilGenerator.CallGetReturnObject(_interceptorFieldBuilder, methodInformation.Hash);
            ilGenerator.Emit(OpCodes.Throw);
        }

        private void EmitInvokeMethod<TResult>(MethodInfo methodInfo, MethodBuilder methodBuilder, TResult returnValue)
        {
            var methodInformation = AddMethodInformationToInterceptor(methodInfo, returnValue);

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.CallMethodWasExecuted(_interceptorFieldBuilder, methodInformation.Hash);

            if (methodBuilder.ReturnType != typeof(void))
            {
                if (methodBuilder.ReturnType.GetTypeInfo().IsValueType)
                {
                    ilGenerator.CallGetReturnObject(_interceptorFieldBuilder, methodInformation.Hash);
                    ilGenerator.UnboxObject(typeof(object), methodBuilder.ReturnType);
                }
                else
                {
                    ilGenerator.CallGetReturnObject(_interceptorFieldBuilder, methodInformation.Hash);
                }
            }

            ilGenerator.Emit(OpCodes.Ret);
        }

        private void EmitConstructor(ConstructorBuilder constructorBuilder)
        {
            _interceptorFieldBuilder = _typeBuilder.DefineField("_interceptor", typeof(Interceptor), FieldAttributes.Private);

            ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, _interceptorFieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);
        }

        private MethodInformation AddMethodInformationToInterceptor(MethodInfo methodInfo, object returnValue = null)
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

            if (type == typeof(Int32))
            {
                return OpCodes.Ldc_I4;
            }

            if (type == typeof(Int16))
            {
                return OpCodes.Ldc_I4;
            }

            if (type == typeof(Int64))
            {
                return OpCodes.Ldc_I8;
            }

            if (type == typeof(Single))
            {
                return OpCodes.Ldc_R4;
            }

            if (type == typeof(Double))
            {
                return OpCodes.Ldc_R8;
            }

            if (type == typeof(UInt16))
            {
                return OpCodes.Ldc_I4;
            }

            if (type == typeof(UInt32))
            {
                return OpCodes.Ldc_I4;
            }

            if (type == typeof(Boolean))
            {
                return OpCodes.Ldc_I4;
            }

            throw new ArgumentException("Type " + type + " could not be converted to a OpCode");
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