using Xunit;
using System;
using SunLine.EasyMoq.Core;
using SunLine.EasyMoq.Tests.Objects;

namespace SunLine.EasyMoq.Tests
{
    public class SampleTest
    {
        [Fact]
        public void MockObjectMustBeCreated()
        {
            var mock = new Mock<IFakeProxyInterface>();
            
            Assert.NotNull(mock);
        }
    
        [Fact]
        public void MockObjectTypeMustBeCreated()
        {
            var mock = new Mock<IFakeProxyInterface>();
             
            Assert.NotNull(mock.ObjectType);
        }
        
        [Fact]
        public void MockObjectMustImplementInterface()
        {
            var mock = new Mock<IFakeProxyInterface>();
            
            var proxyObject = mock.Object as IFakeProxyInterface;

            Assert.NotNull(proxyObject);
        }
        
        [Fact]
        public void MockObjectMustCanCallSimplestMethod()
        {
            var mock = new Mock<IFakeProxyInterface>();
            
            mock.Object.SimplestMethod();
        }
                
        [Fact]
        public void MockObjectMustCanCallMethodReturningInt()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsInt());

            mock.Object.MethodReturnsInt();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedInt()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsInt()).Returns(666);

            int number = mock.Object.MethodReturnsInt();

            Assert.Equal(666, number);
        }

        [Fact]
        public void MockObjectMustReturnExpectedIntWhenWeUsesItIsAny()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodWithValueParameter(It.IsAny<int>())).Returns(24);
            
            int value = mock.Object.MethodWithValueParameter(1);
            
            Assert.Equal(24, value);
        }

        [Fact]
        public void MockObjectMustCanCallMethodReturningString()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsString());

            mock.Object.MethodReturnsString();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedString()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsString()).Returns("text");

            string text = mock.Object.MethodReturnsString();
            
            Assert.Equal("text", text);
        }
        
        [Fact]
        public void MockObjectMustCanCallMethodReturningEnum()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsEnum());
            
            mock.Object.MethodReturnsEnum();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedEnum()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsEnum()).Returns(ValueEnum.Number);
            
            ValueEnum value = mock.Object.MethodReturnsEnum();
            
            Assert.Equal(ValueEnum.Number, value);
        }
        
        [Fact]
        public void MockObjectMustCanCallMethodReturningObject()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsSimpleObject());
            
            mock.Object.MethodReturnsSimpleObject();
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedObject()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodReturnsSimpleObject()).Returns(new SimpleObject { Number = 123 });
            
            var simpleObject = mock.Object.MethodReturnsSimpleObject();
            
            Assert.Equal(123, simpleObject.Number);
        }
        
        [Fact]
        public void MockObjectMustCanCallMethodWithValueParameter()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.MethodWithValueParameter(12));
            
            mock.Object.MethodWithValueParameter(1);
        }
        
        [Fact]
        public void MockObjectMustReturnExpectedValueForProperty()
        {
            var mock = new Mock<IFakeProxyInterface>();
            mock.Setup(x => x.IntegerProperty).Returns(10);
            
            int value = mock.Object.IntegerProperty;
            
            Assert.Equal(10, value);
        }
        
		[Fact]
		public void ThrowsIfExpectationThrows()
		{
			var mock = new Mock<IFakeProxyInterface>();

			mock.Setup(x => x.SimplestMethod()).Throws(new FormatException());

			Assert.Throws<FormatException>(() => mock.Object.SimplestMethod());
		}
        
		[Fact]
		public void ThrowsIfExpectationThrowsWithGenericsExceptionType()
		{
			var mock = new Mock<IFakeProxyInterface>();

			mock.Setup(x => x.SimplestMethod()).Throws<FormatException>();

			Assert.Throws<FormatException>(() => mock.Object.SimplestMethod());
		}
    }
}
