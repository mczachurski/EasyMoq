namespace SunLine.EasyMoq.Tests
{
	public enum ValueEnum
	{
		Unknown = 0,
		Number = 1
	}
	
	public class SimpleObject
	{
		private int _number;
		
		public int Number 
		{
			get { return _number; }
			set { _number = value; }	
		}
	}
	
	public interface IFakeProxyInterface
	{
		void SimplestMethod();
		
		int MethodReturnsInt();
		
		string MethodReturnsString();
		
		ValueEnum MethodReturnsEnum();
		
		SimpleObject MethodReturnsSimpleObject();
	}
}