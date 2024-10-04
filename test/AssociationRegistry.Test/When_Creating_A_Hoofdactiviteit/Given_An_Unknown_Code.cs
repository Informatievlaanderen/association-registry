namespace AssociationRegistry.Test.When_Creating_A_Hoofdactiviteit;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Unknown_Code
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("RANDOM")]
    public void Then_it_throws_an_UnknownHoofdactiviteitCodeException(string code)
    {
        var ctor = () => HoofdactiviteitVerenigingsloket.Create(code);
        ctor.Should().Throw<HoofdactiviteitCodeIsNietGekend>();
    }
}
