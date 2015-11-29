using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SunLine.EasyMoq.Core
{
	internal static class ILGeneratorExtensions
	{
		private const string _executeMethodMethodName = "MethodWasExecuted";
		private const string _getReturnedObjectMethodName = "GetReturnObject"; 
		
		internal static void CallMethodWasExecuted(this ILGenerator ilGenerator, FieldInfo interceptorFieldInfo, string methodhash)
		{
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldInfo);
            ilGenerator.Emit(OpCodes.Ldstr, methodhash);
            ilGenerator.Emit(OpCodes.Call, typeof(Interceptor).GetMethod(_executeMethodMethodName, new Type[] { typeof(string) }));  
		}
		
		internal static void CallGetReturnObject(this ILGenerator ilGenerator, FieldInfo interceptorFieldInfo, string methodhash)
		{
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldInfo);
            ilGenerator.Emit(OpCodes.Ldstr, methodhash);
            ilGenerator.Emit(OpCodes.Call, typeof(Interceptor).GetMethod(_getReturnedObjectMethodName, new Type[] { typeof(string) }));
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