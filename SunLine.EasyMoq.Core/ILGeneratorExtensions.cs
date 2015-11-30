using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SunLine.EasyMoq.Core
{
    internal static class ILGeneratorExtensions
    {
        private const string ExecuteMethodMethodName = "MethodWasExecuted";
        private const string GetReturnedObjectMethodName = "GetReturnObject";

        internal static void CallMethodWasExecuted(this ILGenerator ilGenerator, FieldInfo interceptorFieldInfo, string methodhash)
        {
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldInfo);
            ilGenerator.Emit(OpCodes.Ldstr, methodhash);
            ilGenerator.Emit(OpCodes.Call, typeof(Interceptor).GetMethod(ExecuteMethodMethodName, new[] { typeof(string) }));
        }

        internal static void CallGetReturnObject(this ILGenerator ilGenerator, FieldInfo interceptorFieldInfo, string methodhash)
        {
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldInfo);
            ilGenerator.Emit(OpCodes.Ldstr, methodhash);
            ilGenerator.Emit(OpCodes.Call, typeof(Interceptor).GetMethod(GetReturnedObjectMethodName, new[] { typeof(string) }));
        }

        internal static void UnboxObject(this ILGenerator ilGenerator, Type typeFrom, Type typeTo)
        {
            var valObj = ilGenerator.DeclareLocal(typeFrom);
            ilGenerator.Emit(OpCodes.Box, typeFrom);
            ilGenerator.Emit(OpCodes.Stloc, valObj);
            ilGenerator.Emit(OpCodes.Ldloc, valObj);
            ilGenerator.Emit(OpCodes.Unbox_Any, typeTo);
        }
    }
}