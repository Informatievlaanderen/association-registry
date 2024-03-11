namespace AssociationRegistry.Test.When_Creating_A_VCode;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Integer_Smaller_Than_A_Thousand_And_One
{
    [Theory]
    [InlineData(1000)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Then_it_throws_an_InvalidVCodeException(int intCode)
    {
        var ctor = () => VCode.Create(intCode);
        ctor.Should().Throw<VCodeValtBuitenToegelatenWaardes>();
    }
}
