using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SunLine.EasyMoq.Core;

namespace SunLine.EasyMoq.Test
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
            
            Console.WriteLine(mock.ObjectType.FullName); 
            Console.WriteLine(mock.Object.ToString());
             
            Assert.NotNull(mock.ObjectType);
        }
    }
}
