namespace AssociationRegistry.Test.When_Creating_A_Hoofdactiviteit;

using Activiteiten;
using Activiteiten.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Unknow_Code
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("RANDOM")]
    public void Then_it_throws_an_UnknownHoofdactiviteitCodeException(string code)
    {
        var ctor = () => Hoofdactiviteit.Create(code);
        ctor.Should().Throw<UnknownHoofdactiviteitCode>();
    }
}
