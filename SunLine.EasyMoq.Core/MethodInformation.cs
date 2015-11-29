using System;

namespace SunLine.EasyMoq.Core
{
	public class MethodInformation
	{
		public string Hash { get; }
		public string Name { get; }
		public Type[] Parameters { get; }
		public Type ReturnType { get; }
		public object ReturnedObject { get; }
		public int NumberOfCalls { get; private set; }
		
		public MethodInformation(string name, Type returnType, Type[] parameters)
		{
			Hash = Guid.NewGuid().ToString();
			Name = name;
			ReturnType = returnType;
			Parameters = parameters;
		}
		
		public MethodInformation(string name, Type returnType, Type[] parameters, object returnedObject) : this(name, returnType, parameters)
		{
			ReturnedObject = returnedObject;
		}
		
		public void MethodWasExecuted()
		{
			NumberOfCalls++;
		}
	}
}