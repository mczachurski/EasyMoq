using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace SunLine.EasyMoq.Core
{
    public class Mock<T>
    {
        private object _object;
        
        private Type _objectType;
        
        public object Object {
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
            var assemblyName = new AssemblyName("MyDynamicAssembly");
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var dynamicModule = dynamicAssembly.DefineDynamicModule("MyDynamicModule");
            var dynamicType = dynamicModule.DefineType("MyDynamicType");
            
            ConstructorBuilder constructor = dynamicType.DefineDefaultConstructor(MethodAttributes.Public | 
                MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            
            _objectType = dynamicType.CreateTypeInfo().AsType();
            _object = Activator.CreateInstance(_objectType);
        }
    }
}
