using DExpressClone.Components.Core;
using FluentAssertions;
using Xunit;

namespace DExpressClone.Components.Tests.Core;

public class CssClassBuilderTests
{
    [Fact]
    public void EmptyBuilder_ReturnsEmptyString()
    {
        var result = CssClassBuilder.New().Build();
        result.Should().BeEmpty();
    }

    [Fact]
    public void SingleClass_ReturnsThatClass()
    {
        var result = CssClassBuilder.New("btn").Build();
        result.Should().Be("btn");
    }

    [Fact]
    public void MultipleClasses_ReturnsSpaceSeparated()
    {
        var result = CssClassBuilder.New("btn")
            .Add("btn-primary")
            .Add("active")
            .Build();
        result.Should().Be("btn btn-primary active");
    }

    [Fact]
    public void AddIf_WhenTrue_AddsClass()
    {
        var result = CssClassBuilder.New("btn")
            .AddIf("active", true)
            .Build();
        result.Should().Be("btn active");
    }

    [Fact]
    public void AddIf_WhenFalse_DoesNotAddClass()
    {
        var result = CssClassBuilder.New("btn")
            .AddIf("active", false)
            .Build();
        result.Should().Be("btn");
    }

    [Fact]
    public void NullClass_IsIgnored()
    {
        var result = CssClassBuilder.New("btn")
            .Add(null)
            .Build();
        result.Should().Be("btn");
    }

    [Fact]
    public void EmptyClass_IsIgnored()
    {
        var result = CssClassBuilder.New("btn")
            .Add("")
            .Add("   ")
            .Build();
        result.Should().Be("btn");
    }

    [Fact]
    public void NullBaseClass_IsIgnored()
    {
        var result = CssClassBuilder.New(null)
            .Add("active")
            .Build();
        result.Should().Be("active");
    }

    [Fact]
    public void WhitespaceIsTrimmed()
    {
        var result = CssClassBuilder.New("  btn  ")
            .Add("  active  ")
            .Build();
        result.Should().Be("btn active");
    }
}
