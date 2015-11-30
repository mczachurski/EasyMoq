namespace SunLine.EasyMoq.TestServices
{	
	public interface IUserService
	{
		int IntegerProperty { get; set; }
		
		void SimplestMethod();
		
		int MethodReturnsInt();
		
		string MethodReturnsString();
		
		UserStatusEnum MethodReturnsEnum();
		
		User MethodReturnsSimpleObject();
		
		int MethodWithValueParameter(int number);
		
		User MethodReturnsSimpleObjectWithParameter(int number);
	}
}