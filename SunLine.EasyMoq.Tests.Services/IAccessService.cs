using SunLine.EasyMoq.Tests.Objects;

namespace SunLine.EasyMoq.Tests.Services
{	
	public interface IAccessService
	{	
		Access MethodReturnsSimpleObjectWithParameter(int number);
		
		Access MethodReturnsSimpleObjectWithParameter(Access access);
	}
}