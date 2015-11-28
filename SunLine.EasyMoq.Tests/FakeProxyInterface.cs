namespace SunLine.EasyMoq.Tests
{
	public enum ValueEnum
	{
		Unknown = 0,
		Number = 1
	}
	
	public interface IFakeProxyInterface
	{
		void SimplestMethod();
		
		int MethodReturnsInt();
		
		string MethodReturnsString();
	}
}