namespace AssociationRegistry.Test.Admin.Api.CommandMetadata.IfMatchHeaderParser;

using AssociationRegistry.Admin.Api.Infrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_PositiveNumber_Weak_Etag
{
    [Theory]
    [InlineData("W/\"100\"", 100)]
    [InlineData("W/\"1\"", 1)]
    [InlineData("W/\"99999999\"", 99999999)]
    public void Then_it_returns_the_number(string etag, long expected)
    {
        IfMatchParser.ParseIfMatch(etag).Should().Be(expected);
    }
}

[UnitTest]
public class Given_Not_A_PositiveNumber_Weak_Etag
{
    [Theory]
    [InlineData("W/\"-1\"")]
    [InlineData("W/\"-1100\"")]
    [InlineData("W/\"ABCD\"")]
    [InlineData("W/\"-ABCD\"")]
    public void Then_it_throws_a_EtagHeaderIsInvalidException(string etag)
    {
        Assert.Throws<IfMatchParser.EtagHeaderIsInvalidException>(() => IfMatchParser.ParseIfMatch(etag));
    }
}

[UnitTest]
public class Given_Not_A_Weak_Etag
{
    [Theory]
    [InlineData("\"1\"")]
    [InlineData("\"1100\"")]
    [InlineData("\"99999999\"")]
    public void Then_it_throws_a_EtagHeaderIsInvalidException(string etag)
    {
        Assert.Throws<IfMatchParser.EtagHeaderIsInvalidException>(() => IfMatchParser.ParseIfMatch(etag));
    }
}
