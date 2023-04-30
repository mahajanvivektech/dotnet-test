// https://github.com/Moq/moq4/wiki/Quickstart

using System.Text.RegularExpressions;

namespace Moq.Test;

public class FooTest
{
    [Fact]
    public void Test_Mock_Setup()
    {
        // Arrange
        var mock = new Mock<IFoo>();
        mock.Setup(foo => foo.DoSomething("ping"))
            .Returns(true);
        var foo = mock.Object;

        // Act
        var result = foo.DoSomething("ping");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Test_Mock_Setup_With_Out_Arg()
    {
        // Arrange
        var expectedOutString = "ack";
        var mock = new Mock<IFoo>();
        mock.Setup(foo => foo.TryParse("ping", out expectedOutString))
            .Returns(true);
        var foo = mock.Object;

        // Act
        var result = foo.TryParse("ping", out var outString);

        // Assert
        result.Should().BeTrue();
        expectedOutString.Should().Be(outString);
    }

    [Fact]
    public void Test_Mock_Setup_With_Ref_Arg()
    {
        // Arrange
        var instance = new Bar();
        var mock = new Mock<IFoo>();

        mock.Setup(foo => foo.Submit(ref instance))
            .Returns(true);

        var foo = mock.Object;

        // Act
        var result = foo.Submit(ref instance);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Test_Access_Invocation_Arguments_When_Returning_Value()
    {
        // Arrange
        var mock = new Mock<IFoo>();

        mock.Setup(x => x.DoSomethingStringy(It.IsAny<string>()))
            .Returns((string s) => s.ToLower());

        var foo = mock.Object;

        // Act
        var result = foo.DoSomethingStringy("Vivek");

        // Assert
        result.Should().Be("vivek");
    }

    [Fact]
    public void Test_Throwing_When_Invoked_With_Specific_Params()
    {
        // Arrange
        var mock = new Mock<IFoo>();

        mock.Setup(foo => foo.DoSomething("reset"))
            .Throws<InvalidOperationException>();

        mock.Setup(foo => foo.DoSomething(""))
            .Throws(new ArgumentException("command"));

        var foo = mock.Object;

        // Act
        var act = () => foo.DoSomething("reset");

        // Assert
        act.Should()
            .Throw<InvalidOperationException>();

        // Act
        act = () => foo.DoSomething("");

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("command");

        // Act & Assert
        foo.Invoking(x => x.DoSomething("reset"))
            .Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task Test_Setup_Async_Methods()
    {
        // Arrange
        var mock = new Mock<IFoo>();

        mock.Setup(f => f.DoSomethingAsync())
            .ReturnsAsync(true);

        mock.Setup(f => f.DoSomethingAsync().Result)
            .Returns(true);

        mock.Setup(f => f.DoSomethingAsync())
            .Returns(async () => true);

        var foo = mock.Object;

        // Act
        var result = await foo.DoSomethingAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Test_Matching_Arguments()
    {
        // Arrange
        var mock = new Mock<IFoo>();
        // any value
        mock.Setup(f => f.DoSomething(It.IsAny<string>()))
            .Returns(true);

        var foo = mock.Object;

        // Act
        var result = foo.DoSomething("Vivek");

        // Assert
        result.Should().BeTrue();

        // Arrange
        var bar = new Bar();

        // any value passed in a `ref` parameter
        mock.Setup(f => f.Submit(ref It.Ref<Bar>.IsAny))
            .Returns(true);

        // Act
        result = foo.Submit(ref bar);

        // Assert
        result.Should().BeTrue();

        // Arrange
        // matching Func<int>, lazy evaluated
        mock.Setup(f => f.Add(It.Is<int>(i => i % 2 == 0)))
            .Returns(true);

        // Act
        result = foo.Add(2);

        // Assert
        result.Should().BeTrue();

        // Arrange
        // matching ranges
        mock.Setup(f => f.Add(It.IsInRange<int>(0, 10, Range.Inclusive)))
            .Returns(true);

        // Act
        result = foo.Add(10);

        // Assert
        result.Should().BeTrue();

        // Arrange
        // matching regex
        mock.Setup(x => x.DoSomethingStringy(It.IsRegex("[a-d]+", RegexOptions.IgnoreCase)))
            .Returns("foo");

        // Act
        var res = foo.DoSomethingStringy("a");

        // Assert
        res.Should().Be("foo");
    }

    [Fact]
    public void Test_Callbacks()
    {
        // Arrange
        var expectedArg = string.Empty;
        var mock = new Mock<IFoo>();

        // access invocation arguments
        mock.Setup(f => f.DoSomething(It.IsAny<string>()))
            .Callback((string s) => expectedArg = s)
            .Returns(true);

        var foo = mock.Object;

        // Act
        foo.DoSomething("ping");

        // Assert
        expectedArg.Should().Be("ping");
    }

    [Fact]
    public void Test_Properties()
    {
        // Arrange
        var mock = new Mock<IFoo>();
        mock.Setup(f => f.Name)
            .Returns("bar");

        var foo = mock.Object;

        // Act
        var result = foo.Name;

        // Assert
        result.Should().Be("bar");
    }
}
