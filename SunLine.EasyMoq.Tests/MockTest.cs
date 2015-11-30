using Xunit;
using System;
using SunLine.EasyMoq.Core;
using SunLine.EasyMoq.TestServices;

namespace SunLine.EasyMoq.Tests
{
    public class SampleTest
    {
        [Fact]
        public void MockObjectMustBeCreated()
        {
            var mock = new Mock<IUserService>();
            
            Assert.NotNull(mock);
        }
    
        [Fact]
        public void MockObjectTypeMustBeCreated()
        {
            var mock = new Mock<IUserService>();
             
            Assert.NotNull(mock.ObjectType);
        }

        [Fact]
        public void MockObjectMustImplementInterface()
        {
            var mock = new Mock<IUserService>();
            
            var proxyObject = mock.Object as IUserService;

            Assert.NotNull(proxyObject);
        }
        
        [Fact]
        public void MockObjectMustCanCallSimplestMethod()
        {
            var mock = new Mock<IUserService>();
            
            mock.Object.SimplestMethod();
        }
                
        [Fact]
        public void MockObjectMustCanCallMethodReturningInt()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsInt());

            mock.Object.MethodReturnsInt();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedInt()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsInt()).Returns(666);

            int number = mock.Object.MethodReturnsInt();

            Assert.Equal(666, number);
        }

        [Fact]
        public void MockObjectMustReturnExpectedIntWhenWeUsesItIsAny()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodWithValueParameter(It.IsAny<int>())).Returns(24);
            
            int value = mock.Object.MethodWithValueParameter(1);
            
            Assert.Equal(24, value);
        }

        [Fact]
        public void MockObjectMustCanCallMethodReturningString()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsString());

            mock.Object.MethodReturnsString();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedString()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsString()).Returns("text");

            string text = mock.Object.MethodReturnsString();
            
            Assert.Equal("text", text);
        }
        
        [Fact]
        public void MockObjectMustCanCallMethodReturningEnum()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsEnum());
            
            mock.Object.MethodReturnsEnum();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedEnum()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsEnum()).Returns(UserStatusEnum.Active);
            
            UserStatusEnum value = mock.Object.MethodReturnsEnum();
            
            Assert.Equal(UserStatusEnum.Active, value);
        }
        
        [Fact]
        public void MockObjectMustCanCallMethodReturningObject()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsSimpleObject());
            
            mock.Object.MethodReturnsSimpleObject();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedObject()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodReturnsSimpleObject()).Returns(new User { Number = 123 });
            
            var simpleObject = mock.Object.MethodReturnsSimpleObject();
            
            Assert.Equal(123, simpleObject.Number);
        }
        
        [Fact]
        public void MockObjectMustCanCallMethodWithValueParameter()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.MethodWithValueParameter(12));
            
            mock.Object.MethodWithValueParameter(1);
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedValueForProperty()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(x => x.IntegerProperty).Returns(10);
            
            int value = mock.Object.IntegerProperty;
            
            Assert.Equal(10, value);
        }
        
		[Fact]
		public void ThrowsIfExpectationThrows()
		{
			var mock = new Mock<IUserService>();

			mock.Setup(x => x.SimplestMethod()).Throws(new FormatException());

			Assert.Throws<FormatException>(() => mock.Object.SimplestMethod());
		}
        
		[Fact]
		public void ThrowsIfExpectationThrowsWithGenericsExceptionType()
		{
			var mock = new Mock<IUserService>();

			mock.Setup(x => x.SimplestMethod()).Throws<FormatException>();

			Assert.Throws<FormatException>(() => mock.Object.SimplestMethod());
		}
        
        [Fact]
        public void MockMethodWithoutParametersMustBeCalledExpectedNumberOfTimes()
        {
            var mock = new Mock<IUserService>();
            
            mock.Object.SimplestMethod();
            
            mock.Verify(m => m.SimplestMethod(), Times.Once());
        }
        
        [Fact]
        public void MockMethodWithParametersMustBeCalledExpectedNumberOfTimes()
        {
            var mock = new Mock<IUserService>();
            
            mock.Object.MethodWithValueParameter(1);
            mock.Object.MethodWithValueParameter(2);
            mock.Object.MethodWithValueParameter(3);
            
            mock.Verify(m => m.MethodWithValueParameter(It.IsAny<int>()), Times.Exactly(3));
        }
        
        [Fact]
        public void MockMethodMustBeCalledExpectedNumberOfTimesEvenIfOtherMethodsAreCalled()
        {
            var mock = new Mock<IUserService>();
            
            mock.Object.SimplestMethod();
            mock.Object.MethodWithValueParameter(1);
            mock.Object.MethodWithValueParameter(2);
            mock.Object.MethodWithValueParameter(3);
            mock.Object.MethodReturnsString();
            
            mock.Verify(m => m.MethodWithValueParameter(It.IsAny<int>()), Times.Exactly(3));
        }
        
        [Fact]
        public void MockMustThrowExceptionWhenMethodShouldnBeCalled()
        {
            var mock = new Mock<IUserService>();
            
            mock.Object.MethodWithValueParameter(1);
            
            Assert.Throws<MockException>(() => mock.Verify(m => m.MethodWithValueParameter(It.IsAny<int>()), Times.Never));
        }
        
        [Fact]
        public void MockMustThrowExceptionWhenMethodWasntCalled()
        {
            var mock = new Mock<IUserService>();
            
            var obj = mock.Object;
            
            Assert.Throws<MockException>(() => mock.Verify(m => m.MethodWithValueParameter(It.IsAny<int>()), Times.AtLeastOnce));
        }
        
        [Fact]
        public void MockPropertyMustBeCalledExpectedNumberOfTimes()
        {
            var mock = new Mock<IUserService>();
            
            var integerProperty = mock.Object.IntegerProperty;
            
            mock.Verify(x => x.IntegerProperty, Times.Once());
        }
        
        [Fact]
        public void TwoMocksShouldWorkAsExpected()
        {
            var firstMock = new Mock<IUserService>();
            var secondMock = new Mock<IAccessService>();
            firstMock.Setup(x => x.MethodReturnsInt()).Returns(11);
            
            var integer = firstMock.Object.MethodReturnsInt();
            
            Assert.Equal(11, integer);
        }
        
        [Fact]
        public void MockShouldReturnInformationMassegeViaToString()
        {
            var firstMock = new Mock<IUserService>();
            firstMock.Setup(x => x.MethodReturnsInt()).Returns(20);
            
            firstMock.Object.SimplestMethod();
            firstMock.Object.SimplestMethod();
            var information = firstMock.ToString();

            Assert.StartsWith("Object was initialized.", information);
        }
        
        [Fact]
        public void MockShoudReturnInformationThatObjectWasntInitialized()
        {
            var firstMock = new Mock<IUserService>();

            var information = firstMock.ToString();
            
            Assert.StartsWith("Object wasn't initialized.", information);
        }
    }
}
