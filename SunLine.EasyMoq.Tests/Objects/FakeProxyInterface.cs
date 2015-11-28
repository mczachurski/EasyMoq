namespace SunLine.EasyMoq.Tests.Objects
{	
	public interface IFakeProxyInterface
	{
		void SimplestMethod();
		
		int MethodReturnsInt();
		
		string MethodReturnsString();
		
		ValueEnum MethodReturnsEnum();
		
		SimpleObject MethodReturnsSimpleObject();
		
		int MethodWithValueParameter(int number);
	}
}