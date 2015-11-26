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
        public void MockShouldSetupEmptyMethod()
        {
            var mock = new Mock<IFakeProxyInterface>();
            
            mock.Setup(x => x.SimpleMethod()).Returns(() => "Some text!");

            Assert.Equal("Some text!", mock.Object.SimpleMethod());
        }
    }
}
