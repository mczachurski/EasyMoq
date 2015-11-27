using Xunit;
using SunLine.EasyMoq.Core;

namespace SunLine.EasyMoq.Tests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dnx.html
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
            mock.Setup(x => x.MethodReturnsInt()).Returns(() => 666);

            int number = mock.Object.MethodReturnsInt();

            Assert.Equal(666, number);
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
            mock.Setup(x => x.MethodReturnsString()).Returns(() => "text");

            string text = mock.Object.MethodReturnsString();
            
            Assert.Equal("text", text);
        }
    }
}
