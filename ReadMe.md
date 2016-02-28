# EasyMoq
This is a short introduction to EasyMoc framework.

## What is EasyMoq
Easy Moq is a simple framework which provide possiblities to mock objects. It can be used in projects created in .NET Core. 
Framework don't have any dependencies to other libraries.

## Usage scenarios.
Below are described most common usage scenarios.

### Simple mock.

We can create an object based on interface. All methods will return default .NET values.

```csharp
    var mock = new Mock<IUserService>();
```

### Mock with expected returns.

We can create object based on interface and we can specify values/objetcs which selected methods should returns.

```csharp
    var mock = new Mock<IUserService>();
    mock.Setup(x => x.MethodReturnsInt()).Returns(666);

    int number = mock.Object.MethodReturnsInt();

    Assert.Equal(666, number);
```

Method whith parameters.

```csharp
    var mock = new Mock<IUserService>();
    mock.Setup(x => x.MethodWithValueParameter(It.IsAny<int>())).Returns(24);

    int value = mock.Object.MethodWithValueParameter(1);

    Assert.Equal(24, value);
```

### Mock with expected exceptions.

We can also create object based on interface and specify that some methods should throw an exception.

```csharp
    var mock = new Mock<IUserService>();

    mock.Setup(x => x.SimplestMethod()).Throws(new FormatException());

    Assert.Throws<FormatException>(() => mock.Object.SimplestMethod());
```

### Count numbers of method executions.

Also framework provide a possiblities to count number of method executions. After running test we can check how many times specific method was executed.
We can check exact amount.

``` csharp
    var mock = new Mock<IUserService>();
            
    mock.Object.MethodWithValueParameter(1);
    mock.Object.MethodWithValueParameter(2);
    mock.Object.MethodWithValueParameter(3);
            
    mock.Verify(m => m.MethodWithValueParameter(It.IsAny<int>()), Times.Exactly(3));
```
Or we can use methods like: "AtLeast", "AtLeastOnce', "AtMost", "AtMostOnce", "Between", "Never", "Once", for example:

```csharp
    var mock = new Mock<IUserService>();
            
    mock.Object.MethodWithValueParameter(1);
            
    Assert.Throws<MockException>(() => mock.Verify(m => m.MethodWithValueParameter(It.IsAny<int>()), Times.Never));
```

## Missing features.
Still there are missing features, like:
- matching method arguments,
- stub properties,
- stub events,
- callbacks.