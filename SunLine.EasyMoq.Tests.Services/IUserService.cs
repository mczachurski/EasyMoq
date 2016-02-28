using System.Collections.Generic;
using SunLine.EasyMoq.Tests.Objects;

namespace SunLine.EasyMoq.Tests.Services
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
		
		IList<Access> GetUserAccess(int number);
		
		IList<Access> UpdateUserAccess(int number, IList<Access> access);
	}
}