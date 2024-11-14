namespace AssociationRegistry.Test.When_Creating_A_Werkingsgebied;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Known_Code
{
    [Theory]
    [InlineData("BE2")]
    [InlineData("BE25")]
    [InlineData("BE25535002")]
    public void Then_identical_codes_with_correct_casing_return_a_werkingsgebied(string code)
    {
        var ctor = () => Werkingsgebied.Create(code);

        ctor.Should().NotThrow<WerkingsgebiedCodeIsNietGekend>();

        Werkingsgebied.Create(code).Should().NotBeNull();
    }

    [Theory]
    [InlineData("be2")]
    [InlineData("be25")]
    public void Then_identical_codes_with_incorrect_casing_return_a_werkingsgebied(string code)
    {
        var ctor = () => Werkingsgebied.Create(code);

        ctor.Should().NotThrow<WerkingsgebiedCodeIsNietGekend>();

        Werkingsgebied.Create(code).Should().NotBeNull();
    }
}
