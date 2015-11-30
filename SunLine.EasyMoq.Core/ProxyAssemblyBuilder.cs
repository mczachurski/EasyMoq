using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SunLine.EasyMoq.Core
{
    internal class ProxyAssemblyBuilder
    {
        private static volatile ProxyAssemblyBuilder _instance;
        private static object _syncRoot = new Object();
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        private ProxyAssemblyBuilder()
        {
            _assemblyBuilder = CreateAssemblyBuilder();
            _moduleBuilder = CreateModuleBuilder();
        }

        internal static ProxyAssemblyBuilder Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new ProxyAssemblyBuilder();
                        }
                    }
                }

                return _instance;
            }
        }

        internal TypeBuilder CreateTypeBuilder(Type mockType)
        {
            var randomName = Guid.NewGuid().ToString();
            var typeBuilder = _moduleBuilder.DefineType($"Proxy{mockType.Name}{randomName}", TypeAttributes.Public);
            return typeBuilder;
        }

        internal AssemblyBuilder CreateAssemblyBuilder()
        {
            var assemblyName = new AssemblyName("SunLine.EasyMoq.ProxyAssembly") { Version = new Version(1, 0, 0) };
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder;
        }

        private ModuleBuilder CreateModuleBuilder()
        {
            var moduleBuilder = _assemblyBuilder.DefineDynamicModule("SunLine.EasyMoq.ProxyAssembly");
            return moduleBuilder;
        }
    }
}