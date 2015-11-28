using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SunLine.EasyMoq.Core
{
	public class ProxyAssemblyBuilder
	{
		private static volatile ProxyAssemblyBuilder instance;
   		private static object syncRoot = new Object();

		private readonly AssemblyBuilder _assemblyBuilder;
		private readonly ModuleBuilder _moduleBuilder;	   

   		private ProxyAssemblyBuilder() 
		{
            _assemblyBuilder = CreateAssemblyBuilder();
			_moduleBuilder = CreateModuleBuilder();
		}

   		public static ProxyAssemblyBuilder Instance
   		{
      		get 
      		{
         		if (instance == null) 
         		{
            		lock (syncRoot) 
            		{
               			if (instance == null) 
						{
                  			instance = new ProxyAssemblyBuilder();
						}
            		}
         		}

         		return instance;
      		}
   		}
		   
		public TypeBuilder CreateTypeBuilder(Type mockType)
		{
			var randomName = Guid.NewGuid().ToString();
			var typeBuilder = _moduleBuilder.DefineType($"Proxy{mockType.Name}{randomName}", TypeAttributes.Public);
			return typeBuilder;
		}	
		
		private AssemblyBuilder CreateAssemblyBuilder()
		{
            var assemblyName = new AssemblyName("SunLine.EasyMoq.ProxyAssembly");
            assemblyName.Version = new Version(1, 0, 0);
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