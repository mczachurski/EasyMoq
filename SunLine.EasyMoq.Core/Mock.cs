using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SunLine.EasyMoq.Core
{
    public class Mock<T>
    {
        private T _object;
        
        private Type _objectType;
        
        public T Object {
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
            Type mockInterface = typeof(T);
                 
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
            _object = (T) Activator.CreateInstance(_objectType);
        }
        
        private void AddMethodImpl(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] paramTypes = ParamTypes(parameters, false);
            
            MethodBuilder mdb = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, paramTypes);
            
            ILGenerator il = mdb.GetILGenerator();
            il.EmitWriteLine("This is line of text in method mody.");
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
