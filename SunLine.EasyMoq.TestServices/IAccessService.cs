namespace SunLine.EasyMoq.TestServices
{	
	public interface IAccessService
	{	
		Access MethodReturnsSimpleObjectWithParameter(int number);
		
		Access MethodReturnsSimpleObjectWithParameter(Access access);
	}
}